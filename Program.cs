using System;
using TradeBot.Data;
using TrueRealExchange;

namespace TradeBot
{
    class Program
    {
        static void Main()
        {

           // var bConnector = new BinanceConnector();
           // var exch = new Exchange(bConnector);
           // //var account = new Account();
           //// var candles = await dataParser.GetCandles("ETHUSDT", KlineInterval.OneDay,
           ////                                     new DateTime(2014, 8, 24), new DateTime(2022, 2, 15));
           // //var candless = new List<Candle>();
           // //var zigzag = ZigZag.CalculatePriceStructLight(candles, 1);

           // //var dataParser = new DataParser();
           // var candles = await bConnector.GetCandles("ETHUSDT", KlineInterval.OneDay,
           //                                     new DateTime(2021, 8, 24), new DateTime(2021, 12, 11));

           // //var zigzag = ZigZag.CalculateZigZag(candles, 5m);
           // var accumulations = SliceAlgorithm.FindBoxes(candles).ToList();
            
           // //JoinBoxes(accumulations);
           // var allowedPair =  new [] { "BTCUSDT", "ETHUSDT" };
           // var allowedTimeFrame = new[] { KlineInterval.FourHour, KlineInterval.OneDay, KlineInterval.OneWeek };
            
           // foreach(var a in allowedPair)
           //     foreach(var b in allowedTimeFrame)
           //         new DataWorker { Pair = a, TimeFrame = b }.Run();


           // //var a = new DataWorker { Pair ="BTCUSDT", TimeFrame= KlineInterval.FiveMinutes };
           // //a.Run();
           // //Thread.Sleep(15000);
           // //a.Exit();
           //// var zigzag = ZigZag.CalculateZigZag(candles, 5);
           // //var segments = new TrendDetector().TrendDetect(zigzag);
           // //var zigzag = ZigZag.CalculatePriceStructLight(candles,1);

           // ;
           // Export.WriteJson(candles, accumulations, zigzag, null,segments, @"C:\Users\user\Desktop\tvjs-xp-main\src\apps", "data.json");
           // Console.WriteLine("Data has been saved to file");
            //AdminConnector.Server();
            Console.ReadKey();
        }
    }
}
