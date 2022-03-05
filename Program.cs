using Binance.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeBot.Data;

namespace TradeBot
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var dataParser = new BinanceConnector();
            var candles = await dataParser.GetCandles("ETHUSDT", KlineInterval.OneDay,
                                                new DateTime(2014, 8, 24), new DateTime(2022, 2, 15));
            //var candless = new List<Candle>();
            //var zigzag = ZigZag.CalculatePriceStructLight(candles, 1);
            var zigzag = ZigZag.CalculateZigZag(candles, 5m);
            var accumulations = SliceAlgorithm.FindBoxes(candles).ToList();

            var dataParser = new DataParser();
            var candles = await dataParser.GetCandles("ETHUSDT", Binance.Net.Enums.KlineInterval.OneDay,
                                                new DateTime(2020, 5, 19), new DateTime(2021, 10, 1));


            //JoinBoxes(accumulations);
            //var allowedPair =  new [] { "BTCUSDT", "ETHUSDT", "LTCUSDT", "XMRUSDT", "XMRBUSD" };
            //var allowedTimeFrame = new[] { KlineInterval.FourHour, KlineInterval.OneDay, KlineInterval.OneWeek };
            //foreach(var a in allowedPair)
            //    foreach(var b in allowedTimeFrame)
            //        new DataWorker { Pair = a, TimeFrame = b }.Run();


            //var a = new DataWorker { Pair ="BTCUSDT", TimeFrame= KlineInterval.FiveMinutes };
            //a.Run();
            //Thread.Sleep(15000);
            //a.Exit();

            var segments = new TrendDetector().TrendDetect(zigzag);
            //var zigzag = ZigZag.CalculatePriceStructLight(candles,1);
            var zigzag = ZigZag.GetZigZagDot(ZigZag.CalculateZigZag(candles,5), candles);

            ;
            Export.WriteJson(candles, accumulations, zigzag, null,segments, @"C:\Users\user\Desktop\tvjs-xp-main\src\apps", "data.json");
            Console.WriteLine("Data has been saved to file");
            //AdminConnector.Server();
            Console.ReadKey();
        }
    }
}
