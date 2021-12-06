using Binance.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TradeBot.Data;

namespace TradeBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var dataParser = new DataParser();
            //var candles = await dataParser.GetCandles("ETHUSDT", Binance.Net.Enums.KlineInterval.OneDay,
            //                                    new DateTime(2021, 1, 1), new DateTime(2021, 1, 1));

            //var candlefdss = await dataParser.GetCandles("ETHUSDT", Binance.Net.Enums.KlineInterval.OneDay,
            //                        new DateTime(2021, 1, 1), new DateTime(2021, 1, 1).AddDays(1));

            //var candcxvles = await dataParser.GetCandles("ETHUSDT", Binance.Net.Enums.KlineInterval.OneDay,
            //                        new DateTime(2021, 1, 1).AddSeconds(-1), new DateTime(2021, 1, 1).AddSeconds(1));


            //var candsdfles = await dataParser.GetCandles("ETHUSDT", Binance.Net.Enums.KlineInterval.OneDay,
            //                        new DateTime(2021, 1, 1), new DateTime(2021, 1, 9));



            ////var cansdfdles = await dataParser.GetCandles("ETHUSDT", Binance.Net.Enums.KlineInterval.OneDay,
            ////                        new DateTime(2020, 0, 0), new DateTime(2021, 0, 9));
            //var candsfsddles = await dataParser.GetCandles("ETHUSDT", Binance.Net.Enums.KlineInterval.OneDay,
            //                        new DateTime(2020, 1, 1), new DateTime(2021, 1, 10));
            ;
            //var candless = new List<Candle>();
            //var zigzag = ZigZag.CalculatePriceStructLight(candles, 1);
            //var zigzag = ZigZag.CalculateZigZag(candles,5);
            //var accumulations = SliceAlgorithm.FindBoxes(candles).ToList();
            ;
            //JoinBoxes(accumulations);
            var allowedPair =  new [] { "BTCUSDT", "ETHUSDT", "LTCUSDT", "XMRUSDT", "XMRBUSD" };
            var allowedTimeFrame = new[] { KlineInterval.FourHour, KlineInterval.OneDay, KlineInterval.OneWeek };
            foreach(var a in allowedPair)
                foreach(var b in allowedTimeFrame)
                    new DataWorker { Pair = a, TimeFrame = b }.Run();

            //DataExport.WriteJson(candles, accumulations, zigzag, @"C:\Users\user\Desktop\tvjs-xp-main\src\apps\data.json");
            Console.WriteLine("Data has been saved to file");
            //AdminConnector.Server();
            Console.ReadKey();

        }



        private static void JoinBoxes(List<Accumulation> accumulations)
        {
            ;
            foreach (var a in accumulations.ToList())
            {
                foreach (var t in accumulations.ToList())
                {
                    if (a.StartTimeStamp < t.StartTimeStamp && t.EndTimeStamp < a.EndTimeStamp)
                    {
                        accumulations.Remove(t);
                    }
                }
            }

        }
    }
}
