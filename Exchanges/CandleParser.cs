using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinanceApiDataPArser
{
    class DataParser
    {
        public async Task<List<CandleOHLC>> GetCandles(string symbol, Binance.Net.Enums.KlineInterval interval, DateTime start, DateTime end)
        {

            var client = new BinanceClient(new BinanceClientOptions() { });
            var callResult = await client.Spot.Market.GetKlinesAsync(symbol, interval, start, end,1000);
            var candles = new List<CandleOHLC>();

            foreach (var t in callResult.Data)
            {
                var candle = new CandleOHLC(t.OpenTime.Ticks / 1000, t.Open, t.High, t.Low, t.Close);
                candles.Add(candle);
            }
            return candles;
        }
   
    }
}

