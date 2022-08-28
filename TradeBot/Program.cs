using System;
using System.Threading.Tasks;
using Binance.Net.Enums;
using ExchangeConnectors.Connectors;
using TradeBot.Strategy.Strategies;
using Core.Data;
using TradeBot.Strategy;
using static DataTypes.TimeFrames;

namespace TradeBot;

static class Program
{
    public static void Main()
    {
        var key = "";
        var secret = "";
        var bConnector = new BinanceConnector(key, secret);

        //  var dc = new DataConcentrator();


        var strategy = new SimpleStrategy();

        var id = StrategysManager.AddStrategy(strategy, bConnector);
        StrategysManager.RunStrategy(id, new[] { "BTCUSDT" }, new[] { TimeFrame.h1 }, new DateTime(2018, 1, 11));

        var aa = new DataSaver();

        Console.ReadKey();
        Console.ReadKey();
        Console.ReadKey();
        Console.ReadKey();
        Console.ReadKey();
    }
}