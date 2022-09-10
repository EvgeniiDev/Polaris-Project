using System;
using ExchangeConnectors.Connectors;
using Core.Data;
using TradeBot.Strategies;
using static DataTypes.TimeFrames;

namespace TradeBot;

public static class Program
{
    public static void Main()
    {
        const string key = "UEmwbCVtgeRDa3By0d0lsyeh1l1vwSs15TPfLHH7pRmkymWAcOVEAHPxBCBiOYum";
        const string secret = "OZFArrDVZK8y9XpT90RnO8UEUgBDteiobjKzFH4FvoGQusrypvvTnTsryGKZ7MjL";


        var bConnector = new BinanceConnector(key, secret);

        var strategy = new SimpleStrategy();

        var id = StrategysManager.AddStrategy(strategy, bConnector);
        StrategysManager.RunStrategy(id,
            new[] { "BTCUSDT", "LTCUSDT" },
            new[] { TimeFrame.h1 },
            new DateTime(2018, 1, 11));

        var dataSaver = new DataSaver();

        Console.ReadKey();
    }
}