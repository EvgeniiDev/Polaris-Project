
using System.Collections.Generic;

namespace TradeBot
{
    public class Graph
    {
        public List<Candle> Candles;

        public Graph(List<Candle> candles)
        {
            Candles = candles;
        }
    }
    public class Candle
    {
        public Candle(long ticks, decimal open, decimal high, decimal low, decimal close)
        {
            TimeStamp = ticks;
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }
        public long TimeStamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }

    public class Accumulation
    {
        public double VolumeLevel = 0;
        public long StartTimeStamp;
        public decimal LowPrice;
        public long EndTimeStamp;
        public decimal HighPrice;
        public AccumulationType Type;

        public Accumulation(long startTimeStamp, decimal lowPrice, long endTimeStamp, decimal highPrice, AccumulationType type)
        {
            StartTimeStamp = startTimeStamp;
            LowPrice = lowPrice;
            EndTimeStamp = endTimeStamp;
            HighPrice = highPrice;
            Type = type;
        }
    }
    public enum AccumulationType
    {
        Rectangle,
        Triangle,
        Сonstriction,
        CompressionUp,
        CompressionDown
    }
}
