using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradeBot
{
    public static class DateTimeExtension
    {
        private static DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long ToMilliseconds(this DateTime dt) => (dt - Jan1St1970).Ticks / TimeSpan.TicksPerMillisecond;
        public static DateTime ToDateTime(this long milliseconds) => Jan1St1970.AddMilliseconds(milliseconds);
    }

    class BinanceConnector
    {
        public async Task<List<Candle>> GetCandles(string pair, KlineInterval timeFrame, DateTime start, DateTime end)
        {
            var client = new BinanceClient(new BinanceClientOptions());
            var candles = new List<Candle>();
            while ((candles.Count > 0 ? candles.Last().TimeStamp : 0) < end.ToMilliseconds())
            {
                var a = candles.Count > 0 ? candles.Last().TimeStamp : start.ToMilliseconds();
                start = a.ToDateTime();
                var callResult = await client.Spot.Market.GetKlinesAsync(pair, timeFrame, start, end);
                foreach (var t in callResult.Data)
                {
                    var timeOfCandle = t.OpenTime.ToMilliseconds();
                    var newCandle = new Candle(timeOfCandle, t.Open, t.High, t.Low, t.Close);
                    candles.Add(newCandle);
                }
            }
            return candles;
        }
        public async Task<List<Candle>> GetTrades(string pair, KlineInterval timeFrame, DateTime start, DateTime end)
        {
            var client = new BinanceClient(new BinanceClientOptions());
            var candles = new List<Candle>();
            var exit = false;
            while ((candles.Count > 0 ? candles.Last().TimeStamp : 0) < end.ToMilliseconds() && !exit)
            {
                var a = candles.Count > 0 ? candles.Last().TimeStamp : start.ToMilliseconds();
                start = a.ToDateTime();
                var callResult = await client.Spot.Market.GetTradeHistoryAsync(pair, 1000, start.ToMilliseconds());
                foreach (var t in callResult.Data)
                {
                    if (t.TradeTime < end)
                    {
                        var q = t.CommonPrice;
                        var w = t.QuoteQuantity;
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

    }
}

