using ExchangeConnectors;
using System;
using System.Threading.Tasks;
using Binance.Net.Enums;
using ExchangeConnectors.Connectors;
using TradeBot.Algorithms;
using TradeBot.Data;
using static DataTypes.TimeFrames;
using System.Threading;
using Log;

namespace TradeBot;

static class Program
{
    static async Task Main()
    {
        var key = "70boOKYkASoJPBAjbjkMWU6UdMCy9T3NserZaaIkSiBpSFFRnL8bzJsxBaLZMWey";
        var secret = "MrA8gCZzQUZaW7mdvGhnErlgoYw8OTr7Ge0l4Z8OlYX5oGcrbOgaLodCT9nQ1iR7";
        var bConnector = new BinanceConnector(key, secret);


       // var dw = new DataWorker(bConnector, "BTCUSDT", TimeFrame.D1, new DateTime(2021, 12, 11));



       // dw.CheckNewData(10);



       //;

        // var exch = new Exchange(bConnector);
        // //var account = new Account();
        // var candles = await bConnector.GetCandles("ETHUSDT", TimeFrame.D1,
        //                                    new DateTime(2014, 8, 24), new DateTime(2022, 2, 15));
       // //var candless = new List<Candle>();
       //var zigzag = ZigZag.CalculatePriceStructLight(candles, 1);

       // //var dataParser = new DataParser();
       // var candles = await bConnector.GetCandles("ETHUSDT", KlineInterval.OneDay,
       //                                     new DateTime(2021, 8, 24), new DateTime(2021, 12, 11));

        // var zigzag = ZigZag.CalculateZigZag(candles, 5m);
        // var accumulations = SliceAlgorithm.FindBoxes(candles);

        // //JoinBoxes(accumulations);
        var allowedPair =  new [] { "ETHUSDT","BTCUSDT", "ETHBUSD", "BTCBUSD","LTCUSDT","LTCBUSD" };
        var allowedTimeFrame = new[] { KlineInterval.ThreeMinutes, KlineInterval.OneHour, KlineInterval.ThirtyMinutes,KlineInterval.OneMinute};
        var dc = new DataConcentrator(new DataDistributor());
        foreach (var a in allowedPair)
            foreach (var b in allowedTimeFrame)
            {
                dc.RegisterDataSource(bConnector, typeof(BinanceConnector), a, b.GetTimeFrame(), new DateTime(2020, 5, 11));
                //dc.RunDataSource(typeof(BinanceConnector), a, b.GetTimeFrame());
            }


        dc.StartConcentrator();

        dc.TotalRps = 20;
        Thread.Sleep(1000*1000);



        // var a = new DataWorker(bConnector,"BTCUSDT", TimeFrame.m1);
        // a.Run();
        //Thread.Sleep(15000);
        //a.Exit();
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
