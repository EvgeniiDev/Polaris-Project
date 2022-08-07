using Core.Algorithms.MA;
using static DataTypes.TimeFrames;
using Type = System.Type;

namespace Core.Events.Objects;

public class NewZigZagEvent
{
    public decimal value;
    public MovingAverage.Type type;
    public string Pair;
    public TimeFrame TimeFrame;
    public Type ExchangeType;
    public long TimeStamp;
}