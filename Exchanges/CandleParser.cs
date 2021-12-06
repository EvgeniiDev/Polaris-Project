using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradeBot
{
    class DataParser
    {
        public async Task<List<Candle>> GetCandles(string pair, Binance.Net.Enums.KlineInterval timeFrame, DateTime start, DateTime end)
        {

            var client = new BinanceClient(new BinanceClientOptions() { });
            var callResult = await client.Spot.Market.GetKlinesAsync(pair, timeFrame, start, end);
            var candles = new List<Candle>();

            foreach (var t in callResult.Data)
            {
                DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var timeOfCandle = (t.OpenTime-Jan1St1970).Ticks/TimeSpan.TicksPerMillisecond;
                var candle = new Candle(timeOfCandle, t.Open, t.High, t.Low, t.Close);
                candles.Add(candle);
            }
            return candles;
        }


    }
}

