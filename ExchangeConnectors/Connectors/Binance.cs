using Binance.Net;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using ExchangeConnectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ExchangeConnectors.TimeFrames;

namespace TradeBot
{
    public class BinanceConnector : IExchange
    {
        private BinanceClient client;

        public BinanceConnector()
        {
            client = new BinanceClient(new BinanceClientOptions());
        }
        public async Task<List<Candle>> GetCandles(string pair, TimeFrame timeFrame, DateTime start, DateTime end)
        {
            var candles = new List<Candle>();
            while ((candles.Count > 0 ? candles.Last().TimeStamp : 0) < end.ToMilliseconds())
            {
                var time = candles.Count > 0 ? candles.Last().TimeStamp : start.ToMilliseconds();
                start = time.ToDateTime();

                var convertedTimeFrame = ConvertTimeFrame(timeFrame);
                var callResult = await client.SpotApi.ExchangeData.GetKlinesAsync(pair, convertedTimeFrame, start, end);

                if (callResult == null)
                    throw new Exception("No internet!");

                foreach (var candle in callResult.Data)
                {
                    var timeOfCandle = candle.OpenTime.ToMilliseconds();
                    var newCandle = new Candle(timeOfCandle, candle.OpenPrice, candle.HighPrice,
                                                             candle.LowPrice, candle.ClosePrice);
                    candles.Add(newCandle);//yield return
                }
            }
            return candles;
        }

        public async Task<IEnumerable<BinanceOrder>> GetCurrentDeals(string ticker)//нужно добавить какую-то свою структуру - обертку
        {
            var userTradesResult = await client.SpotApi.Trading.GetOrdersAsync(ticker);
            if (userTradesResult.Success)
            {
                return userTradesResult.Data;
            }
            else
            {
                throw new Exception($"Произошла ошибка при получении списка ордеров {userTradesResult.Error}");
            }

        }
        public Dictionary<string, decimal> GetPrices()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Candle>> GetTrades(string pair, TimeFrame timeFrame, DateTime start, DateTime end)
        {
            var client = new BinanceClient(new BinanceClientOptions());
            var candles = new List<Candle>();
            var exit = false;
            while ((candles.Count > 0 ? candles.Last().TimeStamp : 0) < end.ToMilliseconds() && !exit)
            {
                var time = candles.Count > 0 ? candles.Last().TimeStamp : start.ToMilliseconds();
                start = time.ToDateTime();
                var callResult = await client.SpotApi.ExchangeData.GetTradeHistoryAsync(pair, 1000, start.ToMilliseconds());

                foreach (var candle in callResult.Data)
                {
                    if (candle.TradeTime < end)
                    {
                        var q = candle.Price;
                        var w = candle.QuoteQuantity;
                        //candles.Add(newCandle);
                    }
                    else
                    {
                        exit = true;
                        break;
                    }
                }
            }
            return candles;
        }

        private static TimeFrame GetSeconds(KlineInterval timeFrame)
        {
            return timeFrame switch
            {
                KlineInterval.OneMinute => TimeFrame.m1,
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

        private static KlineInterval ConvertTimeFrame(TimeFrame timeFrame)
        {
            return timeFrame switch
            {
                TimeFrame.m1 => KlineInterval.OneMinute,
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

        Task<IEnumerable<ExchangeOrder>> IExchange.GetCurrentDeals(string ticker)
        {
            throw new NotImplementedException();
        }

        public Task<ExchangeOrder> GetOrderInfo(string id)
        {
            throw new NotImplementedException();
        }
    }
}
