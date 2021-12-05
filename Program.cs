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
            var dataParser = new DataParser();
            var candles = await dataParser.GetCandles("ETHUSDT", Binance.Net.Enums.KlineInterval.OneDay,
                                                new DateTime(2021, 2, 1), new DateTime(2021, 11, 9));


            var candless = new List<Candle>();
            //var zigzag = ZigZag.CalculatePriceStructLight(candles, 1);
            //var zigzag = ZigZag.GetZigZagDot(ZigZag.CalculateZigZag(candles,50), candles);
            var zigzag = new List<Dot>();
            ;
            var accumulations = SliceAlgorithm.FindBoxes(candles).ToList();
            ;
            DataExport.WriteJson(candles, accumulations, zigzag, @"C:\Users\user\Desktop\tvjs-xp-main\src\apps\data.json");
            Console.WriteLine("Data has been saved to file");
            //AdminConnector.Server();
            Console.ReadKey();

        }
    }
}
