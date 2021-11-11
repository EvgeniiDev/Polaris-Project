using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using TradeBot.Admin;
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
            ;
            DataExport.WriteJson(candles, accumulations, "data.json");
            Console.WriteLine("Data has been saved to file");
            //AdminConnector.Server();
            Console.ReadKey();
        }

    }
}
