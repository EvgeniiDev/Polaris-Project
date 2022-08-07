using DataStorage.DatabaseObjects;
using DataTypes;

namespace DataStorage.Cache;

public class QueueCache<T> where T : MainDatabaseObject
{
    private LimitedSizeQueue<T> objects;

    public QueueCache(int size)
    {
        objects = new LimitedSizeQueue<T>(size);
    }

    public List<T> GetElementsRange(long start, long end)
    {
        if (objects.GetFirst().TimeStamp <= start && objects.GetLast().TimeStamp >= end)
            return objects.GetAllValues()
                .Where(x => x.TimeStamp >= start)
                .Where(x => x.TimeStamp <= end)
                .ToList();
        return new List<T>();
    }

    public T GetElement(long timeStamp)
    {
        if (objects.GetFirst().TimeStamp <= timeStamp && objects.GetLast().TimeStamp >= timeStamp)
            return objects
                .GetAllValues()
                .First(x => x.TimeStamp == timeStamp);
        return null;
    }

    public QueueCache<T> Add(T candle)
    {
        objects.Enque(candle);
        return this;
    }
}
