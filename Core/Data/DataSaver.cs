using Core.Events;
using Core.Events.Objects;
using DataStorage.DatabaseObjects;
using DataStorage.DataStorages;
using System;

namespace Core.Data;

public class DataSaver
{
    public static CandleDataStorage _candleDataStorage = new();
    private static ZigZagDataStorage _zigZagDataStorage = new();
    private static MovingAverageDataStorage _movingAverageDataStorage = new();

    public DataSaver()
    {
        EventsCatalog.NewCandle += SaveCandle;
        //EventsCatalog.NewZigZag += SaveZigZag;
        //EventsCatalog.MovingAverage += SaveMovingAverage;
    }

    /*private void SaveMovingAverage(NewMovingAverageEvent obj)
    {
        var zigZag = new MovingAverageDBO
        {
            ExchangeName = obj.ExchangeType.Name,
            Pair = obj.Pair,
            TimeStamp = obj.TimeStamp,
            TimeFrame = obj.TimeFrame,
            Price = obj.value,
        };
        var key = (obj.ExchangeType.Name, obj.Pair, obj.TimeFrame);
        _movingAverageDataStorage.Write(key, zigZag);
    }*/

    private void SaveZigZag(NewZigZagEvent obj)
    {
        var zigZag = new ZigZagDBO
        {
            ExchangeName = obj.ExchangeType.Name,
            Pair = obj.Pair,
            TimeStamp = obj.TimeStamp,
            TimeFrame = obj.TimeFrame,
            Price = obj.value,
        };
        var key = (obj.ExchangeType.Name, obj.Pair, obj.TimeFrame);
        _zigZagDataStorage.Write(key, zigZag);
    }

    private void SaveCandle(NewCandleEvent candleEvent)
    {
        var cndl = new CandleDBO
        {
            ExchangeName = candleEvent.ExchangeType.Name,
            Pair = candleEvent.Pair,
            TimeStamp = candleEvent.TimeStamp,
            TimeFrame = candleEvent.TimeFrame,
            Open = candleEvent.Candle.Open,
            High = candleEvent.Candle.High,
            Low = candleEvent.Candle.Low,
            Close = candleEvent.Candle.Close,
        };
        var key = (candleEvent.ExchangeType.Name, candleEvent.Pair, candleEvent.TimeFrame);
        _candleDataStorage.Write(key, cndl);
    }
}
