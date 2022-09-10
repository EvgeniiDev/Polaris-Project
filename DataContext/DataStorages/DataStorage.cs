﻿using DataStorage.DatabaseObjects;
using static DataTypes.TimeFrames;

namespace DataStorage.DataStorages;

public interface IDataStorage<T> where T : MainDatabaseObject
{
    public void OpenApplicationContext();
    public void CloseApplicationContext();
    public void Write((string ExchangeName, string Pair, TimeFrame) key, T candle);
    public void WriteWithOutCloseContext((string ExchangeName, string Pair, TimeFrame) key, T candle);
    public T Get((string ExchangeName, string Pair, TimeFrame) key, long timeStamp);
    public List<T> GetRange((string ExchangeName, string Pair, TimeFrame) key, long start, long end);
}