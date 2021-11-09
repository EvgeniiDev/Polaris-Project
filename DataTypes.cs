using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceApiDataPArser
{
    public class CandleOHLC
    {
        public CandleOHLC(long ticks, decimal open, decimal high, decimal low, decimal close)
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
