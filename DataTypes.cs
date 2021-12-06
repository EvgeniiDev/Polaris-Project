using System;
using System.Collections.Generic;

namespace TradeBot
{
    public class Dot
    {
        public Dot(long timeStamp, decimal price)
        {
            TimeStamp = timeStamp;
            Price = price;
        }

        public long TimeStamp;
        public decimal Price;
    }
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

    public enum LineType
    {
        Horizontal,
        Wane,
        Increase,
    }


    public enum TimeFrame
    {
        m1 = 60,
        m5 = 300,
        m15 = 900,
        m30 = 1800,
        h1 = 3600,
        h4 = 14400,
        h12 = 43200,
        D1 = 86400,
        D3 = 259200,
        W1 = 604800,
        M1 = 2592000
    }


}
