namespace DataStorage.Cache;
//todo потокобезопасным сделать
public class LimitedSizeQueue<T>
{
    private LinkedList<T> list;
    private readonly int limit;

    public LimitedSizeQueue(int limit)
    {
        this.limit = limit;
        list = new LinkedList<T>();
    }

    public void Enque(T item)
    {
        if (limit != 0)
        {
            if (list.Count == limit)
                list.RemoveFirst();
            list.AddLast(item);
        }
    }

    public T Deque()
    {
        var value = list.Last.Value;
        list.RemoveLast();
        return value;
    }

    public T GetLast()
    {
        if (list.Last == null)
            throw new NullReferenceException();
        return list.Last.Value;
    }

    public T GetFirst()
    {
        if (list.First == null)
            throw new NullReferenceException();
        return list.First.Value;
    }

    public IEnumerable<T> GetAllValues()
    {
        return list;
    }

    public int Count
    {
        get => list.Count;
    }
}