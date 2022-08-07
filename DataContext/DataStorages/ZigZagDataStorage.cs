using DataStorage.Cache;
using DataStorage.DatabaseObjects;
using MethodTimer;
using System.Collections.Concurrent;
using static DataTypes.TimeFrames;

namespace DataStorage.DataStorages;

public class ZigZagDataStorage : IDataStorage<ZigZagDBO>
{
    private ApplicationContext db;
    private ConcurrentDictionary<(string, string, TimeFrame), QueueCache<ZigZagDBO>> _cache = new();
    private Func<QueueCache<ZigZagDBO>> createQueueCache = () => new QueueCache<ZigZagDBO>(1000);

    public ZigZagDataStorage() : base()
    {
        OpenApplicationContext();
        db.ZigZag.RemoveRange(db.ZigZag);
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
    public void Write((string, string, TimeFrame) key, ZigZagDBO data)
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            _cache.GetOrAdd(key, _ => createQueueCache()).Add(data);

            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.ZigZag.Add(data);
            db.SaveChanges();
        }
    }

    [Time]
    public void WriteWithOutCloseContext((string, string, TimeFrame) key, ZigZagDBO data)
    {
        _cache.GetOrAdd(key, _ => createQueueCache()).Add(data);
        lock (db)
        {
            db.ZigZag.Add(data);
            db.SaveChanges();
        }
    }

    [Time]
    public ZigZagDBO Get((string, string, TimeFrame) key, long timeStamp)
    {
        if (_cache.ContainsKey(key))
        {
            var zigZag = _cache[key].GetElement(timeStamp);
            if (zigZag != null)
                return zigZag;
        }

        using (ApplicationContext db = new ApplicationContext())
        {
            var zigZag = db.ZigZag
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
    public List<ZigZagDBO> GetRange((string, string, TimeFrame) key, long start, long end)
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
            return dbb.ZigZag
                .Where(x => x.TimeStamp >= start)
                .Where(x => x.TimeStamp <= end)
                .ToList();
        }
    }
}