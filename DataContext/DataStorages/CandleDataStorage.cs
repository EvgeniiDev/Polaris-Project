using AutoMapper;
using DataStorage.Cache;
using DataStorage.DatabaseObjects;
using DataTypes;
using MethodTimer;
using System.Collections.Concurrent;
using static DataTypes.TimeFrames;

namespace DataStorage.DataStorages;

public class CandleDataStorage : IDataStorage<CandleDBO>
{
    private ApplicationContext db;
    private ConcurrentDictionary<(string, string, TimeFrame), QueueCache<CandleDBO>> _cache = new();
    private Func<QueueCache<CandleDBO>> createQueueCache = () => new QueueCache<CandleDBO>(1000);

    public CandleDataStorage() : base()
    {
        OpenApplicationContext();
        db.Candles.RemoveRange(db.Candles);
        db.SaveChanges();
    }

    public void OpenApplicationContext()
    {
        db = new ApplicationContext();
    }

    public void CloseApplicationContext()
    {
        db.Dispose();
    }

    [Time]
    public void Write((string ExchangeName, string Pair, TimeFrame) key, CandleDBO candle)
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            _cache.GetOrAdd(key, _ => createQueueCache()).Add(candle);

            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.Candles.Add(candle);
            db.SaveChanges();
        }
    }

    [Time]
    public void WriteWithOutCloseContext((string ExchangeName, string Pair, TimeFrame) key, CandleDBO data)
    {
        _cache.GetOrAdd(key, _ => createQueueCache()).Add(data);
        lock (db)
        {
            db.Candles.Add(data);
            db.SaveChanges();
        }
    }

    [Time]
    public CandleDBO Get((string ExchangeName, string Pair, TimeFrame) key, long timeStamp)
    {
        if (_cache.ContainsKey(key))
        {
            var candle = _cache[key].GetElement(timeStamp);
            if (candle != null)
                return candle;
        }

        using (ApplicationContext db = new ApplicationContext())
        {
            var candle = db.Candles
                .Where(x => x.Pair == key.Item2 &&
                x.ExchangeName == key.Item1 &&
                x.TimeFrame == key.Item3 &&
                x.TimeStamp == timeStamp);
            if (candle.Count() > 0)
                return candle.First();
        }
        return null;
    }

    [Time]
    public List<CandleDBO> GetRange((string ExchangeName, string Pair, TimeFrame) key, long start, long end)
    {
        if (start > end)
            throw new ArgumentException();

        if (_cache.ContainsKey(key))
        {
            var cachedCandles = _cache[key].GetElementsRange(start, end);
            if (cachedCandles.Count != 0)
                return cachedCandles.ToList();
        }

        using (ApplicationContext dbb = new())
        {
            return dbb.Candles
                .Where(x => x.TimeStamp >= start)
                .Where(x => x.TimeStamp <= end)
                .ToList();
        }
    }
}