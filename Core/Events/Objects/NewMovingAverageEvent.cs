using static DataTypes.TimeFrames;
using Core.Algorithms.MA;

namespace Core.Events.Objects;

public class NewMovingAverageEvent
{
    public decimal value;
    public MovingAverage.Type type;

    public string Pair;
    public TimeFrame TimeFrame;
    public Type ExchangeType;
    public long TimeStamp;
}