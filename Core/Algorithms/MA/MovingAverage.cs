using Core.Events;
using Core.Events.Objects;
using DataTypes;
using Log;

namespace Core.Algorithms.MA;

public class MovingAverage
{
    public List<Dot> History = new();

    private decimal[] _values;
    private int _index = 0;
    private decimal _sum = 0;
    private readonly Type _type;
    private readonly string _pair;
    private readonly TimeFrames.TimeFrame _timeFrame;

    public MovingAverage(int period, Type buildBy, string pair, TimeFrames.TimeFrame timeFrame)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period), "Must be greater than 0");

        _values = new decimal[period];
        _type = buildBy;
        _pair = pair;
        _timeFrame = timeFrame;
        EventsCatalog.NewCandle += ReceiveData;
    }

    private void ReceiveData(NewCandleEvent obj)
    {
        if (obj.Pair != _pair || obj.TimeFrame != _timeFrame)
            return;

        var lastValue = _type switch
        {
            Type.Open => Update(obj.Candle.Open),
            Type.High => Update(obj.Candle.High),
            Type.Low => Update(obj.Candle.Low),
            Type.Close => Update(obj.Candle.Close),
            _ => throw new ArgumentOutOfRangeException(),
        };
        History.Add(new Dot(obj.TimeStamp, lastValue));

        Logger.SendTimerData(_type.ToString(), (double)lastValue);
        Logger.SendTimerData("HighCandle", (double)obj.Candle.High);
        Logger.SendTimerData("LowCandle", (double)obj.Candle.Low);
        Logger.SendTimerData("CloseCandle", (double)obj.Candle.Close);
        Logger.SendTimerData("OpenCandle", (double)obj.Candle.Open);

        var objEvent = new NewMovingAverageEvent
        {
            type = _type,
            ExchangeType = obj.ExchangeType,
            Pair = obj.Pair,
            TimeFrame = obj.TimeFrame,
            TimeStamp = obj.TimeStamp,
            value = lastValue,
        };

        EventsCatalog.InvokeMovingAverage(objEvent);
    }
    //todo переписать красиво
    private decimal Update(decimal nextInput)
    {
        if (_values.Any(x => x == 0))
        {
            _sum += nextInput;
            _values[_index] = nextInput;
            _index = (_index + 1) % _values.Length;
            return nextInput;
        }
        // calculate the new sum
        _sum = _sum - _values[_index] + nextInput;
        // overwrite the old value with the new one
        _values[_index] = nextInput;
        // increment the index (wrapping back to 0)
        _index = (_index + 1) % _values.Length;
        // calculate the average
        return _sum / _values.Length;
    }

    public enum Type
    {
        Open,
        High,
        Low,
        Close,
    }
}