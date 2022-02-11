﻿using Binance.Net.Enums;
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
                                                new DateTime(2021, 8, 24), new DateTime(2021, 12, 5));
            ;
            //var candless = new List<Candle>();
            //var zigzag = ZigZag.CalculatePriceStructLight(candles, 1);
            var zigzag = ZigZag.CalculateZigZag(candles, 4.5m);
            var accumulations = SliceAlgorithm.FindBoxes(candles).ToList();

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
            ;
            Export.WriteJson(candles, null, zigzag, null,segments, @"C:\Users\user\Desktop\tvjs-xp-main\src\apps", "data.json");

            Console.WriteLine("Data has been saved to file");
            //AdminConnector.Server();
            Console.ReadKey();

        }
    }
}
