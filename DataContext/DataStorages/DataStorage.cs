using DataStorage.DatabaseObjects;
using static DataTypes.TimeFrames;

namespace DataStorage.DataStorages;

public interface IDataStorage<T> where T : MainDatabaseObject
{
    public void OpenApplicationContext();
    public void CloseApplicationContext();
    public void Write((string, string, TimeFrame) key, T candle);
    public void WriteWithOutCloseContext((string, string, TimeFrame) key, T candle);
    public T Get((string, string, TimeFrame) key, long timeStamp);
    public List<T> GetRange((string, string, TimeFrame) key, long start, long end);
}