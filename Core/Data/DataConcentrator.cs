using ExchangeConnectors;
using System.Collections.Concurrent;
using static DataTypes.TimeFrames;

namespace Core.Data;

public class DataConcentrator
{
    private static ConcurrentDictionary<(Type, string, TimeFrame), DataWorker> dataSources = new();
    public static bool IsStarted;

    public static void RegisterDataSource(IExchange connector, Type exchangeType, string pair, TimeFrame timeFrame,
        DateTime? startDate = null)
    {
        var dataWorker = new DataWorker(connector, exchangeType, pair, timeFrame, startDate);
        dataSources.TryAdd((exchangeType, pair, timeFrame), dataWorker);
    }

    public static void RunDataSource(Type exchangeType, string pair, TimeFrame timeFrame)
    {
        var key = (exchangeType, pair, timeFrame);
        if (dataSources.ContainsKey(key) && !dataSources[key].IsStarted)
            dataSources[key].Start();
        else
            throw new Exception("Нет такого источника инфы");
    }

    public static void StopDataSource(Type exchangeType, string pair, TimeFrame timeFrame)
    {
        var key = (exchangeType, pair, timeFrame);
        if (dataSources.ContainsKey(key) && dataSources[key].IsStarted)
            dataSources[key].Stop();
        else
            throw new Exception("Нет такого источника инфы");
    }

    public static void StartConcentrator()
    {
        foreach (var source in dataSources)
        {
            var (type, pair, timeFrame) = source.Key;
            RunDataSource(type, pair, timeFrame);
        }
        IsStarted = true;
    }

    public static void StopConcentrator()
    {
        foreach (var source in dataSources)
        {
            var (type, pair, timeFrame) = source.Key;
            StopDataSource(type, pair, timeFrame);
        }
        IsStarted = false;
    }
}