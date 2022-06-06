
namespace DataTypes
{
    public class Candle
    {
        public Candle(long timeStamp, decimal open, decimal high, decimal low, decimal close)
        {
            TimeStamp = timeStamp;
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
}
