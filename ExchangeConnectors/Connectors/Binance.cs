using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using DataTypes;
using Log;
using MethodTimer;
using Polly;
using static DataTypes.TimeFrames;
using Kline = DataTypes.Kline;
using Order = DataTypes.Order;
using Trade = DataTypes.Trade;

namespace ExchangeConnectors.Connectors;

public class BinanceConnector : IExchange
{
    //todo добавить поддержку прокси
    private readonly BinanceClient _restClient;
    private readonly BinanceSocketClient _socketClient;
    private ConcurrentDictionary<(string, TimeFrame), List<Action<Kline>>> events = new();
    private readonly ResponseQueue responseQueue = new();
    /*
    BinanceConnector
    Получение любого количества свечей из истории
    Подписка на получение свечей в реальном времени
    Получение торговых объемов и их распределение по ценам на торговой паре за любой период времени
    Получение текущих цен по всем торговым парам
    Получение своих ордеров по указанной паре

    */

    public BinanceConnector(string key, string secret)
    {
        var credentials = key is null && secret is null ? null : new ApiCredentials(key, secret);

        _restClient = new BinanceClient(new BinanceClientOptions()
        {
            ApiCredentials = credentials,
        });

        _socketClient = new BinanceSocketClient(new BinanceSocketClientOptions()
        {
            ApiCredentials = credentials,
        });
    }

    #region Candles and klines

    [Time]
    public async Task<List<Candle>> GetCandles(string pair, TimeFrame timeFrame, DateTime start, DateTime end)
    {
        var candles = new List<Candle>();
        var maxTime = start;
        while ((end - maxTime).TotalSeconds > 1)
        {
            var convertedTimeFrame = timeFrame.GetKlineInterval();
            var callResult = await responseQueue.Send(() => _restClient.SpotApi.ExchangeData.GetKlinesAsync(pair, convertedTimeFrame, maxTime, end, 1000)
                                                                                            .WithRetries());
            if (callResult.Data.Count() == 0)
            {
                Logger.GetCounter("ZeroCandlesFromExchange").WithLabels(new[] { "method" }).Inc(1);
                break;
            }
            foreach (var candle in callResult.Data)
            {
                maxTime = maxTime < candle.CloseTime ? candle.CloseTime : maxTime;
                var timeOfCandle = candle.OpenTime.ToMilliseconds();
                var newCandle = new Candle(timeOfCandle, candle.OpenPrice, candle.HighPrice,
                                            candle.LowPrice, candle.ClosePrice);
                candles.Add(newCandle);
            }
        }
        return candles;
    }

    [Time]
    public async void SubscribeOnNewKlines(string ticker, TimeFrame tf, Action<Kline> deleg)
    {
        var timeFrame = tf.GetKlineInterval();
        var subscriptionResult =
            await _socketClient.SpotStreams.SubscribeToKlineUpdatesAsync(ticker, timeFrame, NewKlineHandler);

        AddSubscription(events, ticker, tf, deleg);

        if (!subscriptionResult.Success)
        {
            Console.WriteLine("Failed to connect: " + subscriptionResult.Error);
            return;
        }

        subscriptionResult.Data.ConnectionLost += () => { Console.WriteLine("Connection lost"); };
        subscriptionResult.Data.ConnectionRestored += (time) => { Console.WriteLine("Connection restored"); };

    }

    [Time]
    public void UnsubscribeOnNewKlines(string ticker, TimeFrame tf, Action<Kline> deleg)
    {
        RemoveSubscription(events, ticker, tf, deleg);
    }

    [Time]
    private void AddSubscription(ConcurrentDictionary<(string, TimeFrame), List<Action<Kline>>> dict, string ticker,
        TimeFrame tf, Action<Kline> deleg)
    {
        var dictByTicker = dict.GetOrAdd((ticker, tf), new List<Action<Kline>>());
        dictByTicker.Add(deleg);
    }

    [Time]
    private void RemoveSubscription(ConcurrentDictionary<(string, TimeFrame), List<Action<Kline>>> dict, string ticker,
    TimeFrame tf, Action<Kline> deleg)
    {
        List<Action<Kline>> listDeleg;
        if (dict.TryGetValue((ticker, tf), out listDeleg))
            listDeleg.Remove(deleg);
    }

    [Time]
    private void NewKlineHandler(DataEvent<IBinanceStreamKlineData> obj)
    {
        if (!obj.Data.Data.Final) return;

        var timeFrame = obj.Data.Data.Interval.GetTimeFrame();
        if (events.ContainsKey((obj.Data.Symbol, timeFrame)))
        {
            foreach (var del in events[(obj.Data.Symbol, timeFrame)].ToList())
            {
                del(new Kline(
                    obj.Timestamp.ToMilliseconds(),
                    timeFrame,
                    obj.Data.Data.OpenPrice,
                    obj.Data.Data.HighPrice,
                    obj.Data.Data.LowPrice,
                    obj.Data.Data.ClosePrice
                ));
            }
        }
    }
    #endregion

    [Time]
    public async Task<List<Order>> GetCurrentOrdersPerPair(string ticker)
    {
        var userTradesResult = await responseQueue.Send(() => _restClient.SpotApi.Trading.GetOrdersAsync(ticker).WithRetries());
        var trades = new List<Order>();

        foreach (var trade in userTradesResult.Data)
        {
            OrderType orderType;
            if (trade.Type is SpotOrderType.Limit or SpotOrderType.StopLossLimit)
                orderType = ((CommonOrderSide)trade.Side).GetOrderType();
            else
                throw new Exception("я не знаю ,Что делать с таким типом ордеров");

            var status = ((CommonOrderStatus)trade.Status).GetOrderStatus();

            var order = new Order(trade.Price, trade.Quantity, trade.QuantityFilled, orderType, status);
            trades.Add(order);
        }
        return trades;
    }

    [Time]
    public async Task<Dictionary<string, decimal>> GetPrices()
    {
        var tickersResult = await responseQueue.Send(() => _restClient.SpotApi.ExchangeData.GetTickersAsync().WithRetries());
        return tickersResult.Data.ToDictionary(x => x.Symbol, x => x.LastPrice);
    }

    [Time]
    public async Task<List<Trade>> GetTrades(string pair, TimeFrame timeFrame, DateTime startTime, DateTime endTime)
    {
        var trades = new List<Trade>();
        while (startTime < endTime)
        {
            var callResult = await responseQueue.Send(() => _restClient.SpotApi.ExchangeData
                .GetAggregatedTradeHistoryAsync(pair, null, startTime, startTime + TimeSpan.FromHours(1), 1000).WithRetries());

            foreach (var trade in callResult.Data)
            {
                startTime = trade.TradeTime;
                if (trade.TradeTime < endTime)
                    trades.Add(new Trade(trade.Price, trade.Quantity));
                else
                    return trades;
            }
        }

        return trades;
    }

    //private async Task<List<Order>> GetTradesHistory(string pair)
    //{
    //    var spotTradeHistoryData = await restClient.SpotApi.ExchangeData.GetTradeHistoryAsync(pair);

    //    if (spotTradeHistoryData.Success)
    //    {
    //        var trades = new List<Order>();
    //        foreach (var trade in spotTradeHistoryData.Data)
    //        {
    //            var status = trade.
    //            var order = new Order(trade.Price, trade.QuoteQuantity, status);
    //            trades.Add(order)
    //        }
    //        return trades;
    //    }
    //    else
    //    {
    //        throw new Exception("Чет данных не пришло");
    //    }
    //}

    //Create Edit Del order
    [Time]
    public async Task<string> CreateOrder(string pair, OrderType orderType, decimal price, decimal amount)
    {
        var type = CommonOrderType.Limit;
        var orderSide = orderType.GetCommonOrderSide();

        // Placing a buy limit order for 0.001 BTC at a price of 50000USDT each
        var orderData = await responseQueue.Send(() => _restClient.SpotApi.CommonSpotClient.PlaceOrderAsync(
            pair,
            orderSide,
            type,
            amount,
            price).WithRetries());

        return orderData.Data.Id;
        //return ((CryptoExchange.Net.Objects.WebCallResult<Binance.Net.Objects.Models.Spot.BinancePlacedOrder>)orderData.Data.SourceObject).Data.ClientOrderId;
        throw new Exception($"не вышло ордер разместить( {orderData.Error}");
    }

    [Time]
    public async Task<Order> GetOrderInfo(string pair, string id)
    {
        var orderData = await responseQueue.Send(() => _restClient.SpotApi.CommonSpotClient.GetOrderAsync(pair, id).WithRetries());

        var orderType = orderData.Data.Side.GetOrderType();
        var orderStatus = orderData.Data.Status.GetOrderStatus();

        return new Order(orderData.Data.Price.Value, orderData.Data.Quantity.Value, orderData.Data.QuantityFilled.Value,
            orderType, orderStatus);
    }

    [Time]
    public async Task CancelOrder(string pair, string orderId)
    {
        var orderData = await responseQueue.Send(() => _restClient.SpotApi.CommonSpotClient.CancelOrderAsync(orderId, pair).WithRetries());
    }
}

public static class Retry
{
    [Time]
    public static async Task<WebCallResult<T>> WithRetries<T>(this Task<WebCallResult<T>> task, int count = 3)
    {
        var retry = Policy
          .HandleResult<WebCallResult<T>>(x => !x.Success)
          .OrInner<TimeoutException>()
          .WaitAndRetryForeverAsync(
            retryAttempt => TimeSpan.FromMilliseconds(100 * retryAttempt),
            (exception, timespan) =>
            {
                Logger.SendTimerData("RetryAttemptNumber", timespan.TotalMilliseconds / 100);
                Console.WriteLine($"RetryAttempt {timespan.TotalMilliseconds / 100}");
            });

        return await retry.ExecuteAsync(async () => await task);
    }
}

public static class StatusConverter
{
    public static OrderType GetOrderType(this CommonOrderSide side)
    {
        return side switch
        {
            CommonOrderSide.Buy => OrderType.BuyLimit,
            CommonOrderSide.Sell => OrderType.SellLimit,
            _ => throw new Exception("ну и зачем ты сюда такой тип ордеров отправил?"),
        };
    }

    public static CommonOrderSide GetCommonOrderSide(this OrderType type)
    {
        return type switch
        {
            OrderType.BuyLimit => CommonOrderSide.Buy,
            OrderType.SellLimit => CommonOrderSide.Sell,
            _ => throw new Exception("ну и зачем ты сюда такой тип ордеров отправил?"),
        };
    }

    public static Status GetOrderStatus(this CommonOrderStatus status)
    {
        return status switch
        {
            CommonOrderStatus.Filled => Status.Close,
            CommonOrderStatus.Active => Status.Open,
            CommonOrderStatus.Canceled => Status.Open,
            _ => throw new Exception("ну и зачем ты сюда такой тип ордеров отправил?"),
        };
    }
}

public static class TimeFrameExtensions
{
    public static TimeFrame GetTimeFrame(this KlineInterval timeFrame)
    {
        return timeFrame switch
        {
            KlineInterval.OneMinute => TimeFrame.m1,


            KlineInterval.ThreeMinutes => TimeFrame.m3,
            KlineInterval.FiveMinutes => TimeFrame.m5,
            KlineInterval.FifteenMinutes => TimeFrame.m15,
            KlineInterval.ThirtyMinutes => TimeFrame.m30,
            KlineInterval.OneHour => TimeFrame.h1,
            KlineInterval.FourHour => TimeFrame.h4,
            KlineInterval.TwelveHour => TimeFrame.h12,
            KlineInterval.OneDay => TimeFrame.D1,
            KlineInterval.OneWeek => TimeFrame.W1,
            KlineInterval.OneMonth => TimeFrame.M1,
            _ => throw new Exception("Unknown timeframe!"),
        };
    }

    public static KlineInterval GetKlineInterval(this TimeFrame timeFrame)
    {
        return timeFrame switch
        {
            TimeFrame.m1 => KlineInterval.OneMinute,
            TimeFrame.m3 => KlineInterval.ThreeMinutes,
            TimeFrame.m5 => KlineInterval.FiveMinutes,
            TimeFrame.m15 => KlineInterval.FifteenMinutes,
            TimeFrame.m30 => KlineInterval.ThirtyMinutes,
            TimeFrame.h1 => KlineInterval.OneHour,
            TimeFrame.h4 => KlineInterval.FourHour,
            TimeFrame.h12 => KlineInterval.TwelveHour,
            TimeFrame.D1 => KlineInterval.OneDay,
            TimeFrame.D3 => KlineInterval.ThreeDay,
            TimeFrame.W1 => KlineInterval.OneWeek,
            TimeFrame.M1 => KlineInterval.OneMonth,
            _ => throw new Exception("Unknown timeframe!"),
        };
    }
}