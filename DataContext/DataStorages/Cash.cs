using DataTypes;
using ExchangeConnectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.DataStorages;

public class Cash : IExchange
{
    private readonly IExchange _connector;
    private readonly CandleDataStorage _candleDataStorage;
    public Cash(IExchange connector, CandleDataStorage candleDataStorage)
    {
        _connector = connector;
        _candleDataStorage = candleDataStorage;
    }

    public Task<Dictionary<string, decimal>> GetPrices()
    {
        return _connector.GetPrices();
    }

    public async Task<List<Candle>> GetCandles(string pair, TimeFrames.TimeFrame timeFrame, DateTime start, DateTime end)
    {
        var convertedStart = DateTimeExtension.ToMilliseconds(start);
        var convertedEnd = DateTimeExtension.ToMilliseconds(end);
        var keys = _candleDataStorage._cache.Keys
            .Where(key => key.Item2 == pair && key.Item3 == timeFrame);
        var finalListCandles = new List<Candle>();
        foreach (var key in keys)
        {
            var listCandles = _candleDataStorage.GetRange(key, convertedStart, convertedEnd)
                .Select(candleDBO => new Candle(
                    timeStamp, candleDBO.Open, candleDBO.High, candleDBO.Low, candleDBO.Close)
                );

            finalListCandles.AddRange(listCandles);
        }


        if (finalListCandles.Count > 0)
            return finalListCandles;

        return await _connector.GetCandles(pair, timeFrame, start, end);
    }

    public void SubscribeOnNewKlines(string pair, TimeFrames.TimeFrame tf, Action<Kline> deleg)
    {
        _connector.SubscribeOnNewKlines(pair, tf, deleg);
    }

    public void UnsubscribeOnNewKlines(string pair, TimeFrames.TimeFrame tf, Action<Kline> deleg)
    {
        _connector.UnsubscribeOnNewKlines(pair, tf, deleg);
    }

    public Task<List<Order>> GetCurrentOrdersPerPair(string pair)
    {
        return _connector.GetCurrentOrdersPerPair(pair);
    }

    public Task<List<Trade>> GetTrades(string pair, TimeFrames.TimeFrame timeFrame, DateTime startTime, DateTime endTime)
    {
        return _connector.GetTrades(pair, timeFrame, startTime, endTime);
    }

    public Task<string> CreateOrder(string pair, OrderType orderType, decimal price, decimal amount)
    {
        return _connector.CreateOrder(pair, orderType, price, amount);
    }

    public Task<Order> GetOrderInfo(string pair, string id)
    {
        return _connector.GetOrderInfo(pair, id);
    }

    public Task CancelOrder(string pair, string orderId)
    {
        return _connector.CancelOrder(pair, orderId);
    }
}
