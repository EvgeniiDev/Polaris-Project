using System;

namespace TradeBot.Algorithms.MA;

internal class MovingAverage
{
    private readonly decimal[] _values;

    private int _index = 0;
    private decimal _sum = 0;
    public MAType type;

    public MovingAverage(int period, MAType buildBy)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period), "Must be greater than 0");

        _values = new decimal[period];
        type = buildBy;
    }

    public decimal Update(decimal nextInput)
    {
        // calculate the new sum
        _sum = _sum - _values[_index] + nextInput;

        // overwrite the old value with the new one
        _values[_index] = nextInput;

        // increment the index (wrapping back to 0)
        _index = (_index + 1) % _values.Length;

        // calculate the average
        return _sum / _values.Length;
    }

    public enum MAType
    {
        Open,
        High,
        Low,
        Close,
    }
}
