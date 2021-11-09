using Binance.Net;
using Binance.Net.Objects;
using BinanceApiDataPArser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BinanceApiDataParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dataParser = new DataParser();
            var candles = await dataParser.GetCandles("ETHUSDT" , Binance.Net.Enums.KlineInterval.OneDay,
                                                new DateTime(2021, 5, 19),new DateTime(2021, 10, 1));

            var accumulations = new List<Accumulation>(){ new Accumulation(candles[0].TimeStamp, candles[0].Low, candles[5].TimeStamp
                                                                , candles[5].High, AccumulationType.Rectangle),
                                          new Accumulation(candles[0].TimeStamp, candles[0].Low, candles[5].TimeStamp
                                                                , candles[5].High, AccumulationType.Rectangle)};

            dataParser.WriteJson(candles, accumulations, "data.json");
            Console.WriteLine("Data has been saved to file");
            Console.ReadKey();
        }
    }
}
