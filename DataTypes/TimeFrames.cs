namespace DataTypes
{
    public static class TimeFrames
    {
        public enum TimeFrame
        {
            m1 = 60,
            m3 = 180,
            m5 = 300,
            m15 = 900,
            m30 = 1800,
            h1 = 3600,
            h4 = 14400,
            h12 = 43200,
            D1 = 86400,
            D3 = 259200,
            W1 = 604800,
            M1 = 2592000,
        }
        public static int GetSeconds(this TimeFrame timeFrame)
        {
            return (int)timeFrame;
        }
    }
}