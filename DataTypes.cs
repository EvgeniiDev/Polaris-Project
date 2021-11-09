using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceApiDataPArser
{
    class DataTypes
    {
    }
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
        public long TimeStamp1;
        public decimal Price1;
        public long TimeStamp2;
        public decimal Price2;
        public AccumulationType Type;

        public Accumulation(long timeStamp1, decimal low, long timeStamp2, decimal high, AccumulationType type)
        {
            TimeStamp1 = timeStamp1;
            Price1 = low;
            TimeStamp2 = timeStamp2;
            Price2 = high;
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
