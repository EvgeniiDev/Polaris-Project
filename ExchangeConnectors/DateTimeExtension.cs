using System;

namespace ExchangeConnectors
{
    public static class DateTimeExtension
    {
        private static readonly DateTime Jan1St1970 = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long ToMilliseconds(this DateTime dt) => (dt - Jan1St1970).Ticks / TimeSpan.TicksPerMillisecond;
        public static DateTime ToDateTime(this long milliseconds) => Jan1St1970.AddMilliseconds(milliseconds);
    }
}
