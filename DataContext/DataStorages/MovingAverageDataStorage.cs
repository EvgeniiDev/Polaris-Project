using DataStorage.Cache;
using DataStorage.DatabaseObjects;
using MethodTimer;
using System.Collections.Concurrent;
using static DataTypes.TimeFrames;

namespace DataStorage.DataStorages;

public class MovingAverageDataStorage : IDataStorage<MovingAverageDBO>
{
    private ApplicationContext db;
    private ConcurrentDictionary<(string, string, TimeFrame), QueueCache<MovingAverageDBO>> _cache = new();
    private Func<QueueCache<MovingAverageDBO>> createQueueCache = () => new QueueCache<MovingAverageDBO>(1000);

    public MovingAverageDataStorage() : base()
    {
        OpenApplicationContext();
        db.MovingAverage.RemoveRange(db.MovingAverage);
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
    public void Write((string, string, TimeFrame) key, MovingAverageDBO data)
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            _cache.GetOrAdd(key, _ => createQueueCache()).Add(data);
            db.MovingAverage.Add(data);
            db.SaveChanges();
        }
    }

    [Time]
    public void WriteWithOutCloseContext((string, string, TimeFrame) key, MovingAverageDBO data)
    {
        _cache.GetOrAdd(key, _ => createQueueCache()).Add(data);
        lock (db)
        {
            db.MovingAverage.Add(data);
            db.SaveChanges();
        }
    }

    [Time]
    public MovingAverageDBO Get((string, string, TimeFrame) key, long timeStamp)
    {
        if (_cache.ContainsKey(key))
        {
            var zigZag = _cache[key].GetElement(timeStamp);
            if (zigZag != null)
                return zigZag;
        }

        using (ApplicationContext db = new ApplicationContext())
        {
            var zigZag = db.MovingAverage
                .Where(x => x.Pair == key.Item2 &&
                x.ExchangeName == key.Item1 &&
                x.TimeFrame == key.Item3 &&
                x.TimeStamp == timeStamp);
            if (zigZag.Count() > 0)
                return zigZag.First();
        }
        return null;
    }

    [Time]
    public List<MovingAverageDBO> GetRange((string, string, TimeFrame) key, long start, long end)
    {
        if (start > end)
            throw new ArgumentException();

        if (_cache.ContainsKey(key))
        {
            var cachedzigZag = _cache[key].GetElementsRange(start, end);
            if (cachedzigZag.Count != 0)
                return cachedzigZag.ToList();
        }

        using (ApplicationContext dbb = new())
        {
            return dbb.MovingAverage
                .Where(x => x.TimeStamp >= start)
                .Where(x => x.TimeStamp <= end)
                .ToList();
        }
    }
}