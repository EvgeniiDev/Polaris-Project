using System;
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
                                                new DateTime(2021, 5, 19), new DateTime(2021, 10, 1));

            var accumulations = BoxDetectionAlgoritm.FindBoxes(candles).Take(10).ToList();

            //var zigzag = ZigZag.CalculatePriceStructLight(candles,1);
            var zigzag = ZigZag.GetZigZagDot(ZigZag.CalculateZigZag(candles,50), candles);
            ;
            DataExport.WriteJson(candles, accumulations,zigzag, "data.json");
            Console.WriteLine("Data has been saved to file");
            //AdminConnector.Server();
            Console.ReadKey();
        }

    }
}
