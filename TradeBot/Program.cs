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
        var key = "d1kZLyychsfHPhcFTv2ylHrIkuwTEIv6iqR3ID0Pz26VtTYl1xN4ltGTr6T5p7Rh";
        var secret = "nGPQZKaTFQbgKXPzx0xQLIzAyI2EBm5vg8mJPaJtKmx0BQ16jmwX4ge0Rz6lzINJ";
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