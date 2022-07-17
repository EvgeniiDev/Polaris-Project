using static DataTypes.TimeFrames;

namespace DataTypes
{
    public class Kline : Candle
    {
        public Kline(long timeStamp, TimeFrame timeFrame, decimal open, decimal high, decimal low, decimal close) :
            base(timeStamp, open, high, low, close)
        {
            TimeFrame = timeFrame;
        }

        public TimeFrame TimeFrame { get; set; }
    }
}