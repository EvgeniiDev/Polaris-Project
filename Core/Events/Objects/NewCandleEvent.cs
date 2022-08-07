using DataTypes;
using System;
using static DataTypes.TimeFrames;

namespace Core.Events.Objects;

public class NewCandleEvent
{
    public Candle Candle;
    public string Pair;
    public TimeFrame TimeFrame;
    public Type ExchangeType;
    public long TimeStamp;
}
