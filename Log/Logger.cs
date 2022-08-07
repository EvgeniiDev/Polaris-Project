using System.Collections.Concurrent;
using Prometheus;
using Prometheus.DotNetRuntime;

namespace Log;

public static class Logger
{
    private static ConcurrentDictionary<string, Gauge> _timers = new();
    private static ConcurrentDictionary<(string,string), Counter> _counters = new();
    private static readonly DotNetRuntimeStatsBuilder.Builder Collector = DotNetRuntimeStatsBuilder.Default();

    static Logger()
    {
        var server = new KestrelMetricServer("localhost", 9184);
        server.Start();
        Collector.StartCollecting();
    }

    public static void SendTimerData(string name, double measure, string discr = null)
    { 
        //Console.WriteLine(name);
        _timers.GetOrAdd(name, x => Metrics.CreateGauge(x, discr))
            .Set(measure);
    }

    public static Counter GetCounter(string name, string discr = null)
    {
        //Console.WriteLine(name);
        return _counters.GetOrAdd((name,discr), x => Metrics.CreateCounter(x.Item1, x.Item2));
    }
}