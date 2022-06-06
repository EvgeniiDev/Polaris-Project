using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeConnectors
{
    public static class DateTimeExtension
    {
        private static DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long ToMilliseconds(this DateTime dt) => (dt - Jan1St1970).Ticks / TimeSpan.TicksPerMillisecond;
        public static DateTime ToDateTime(this long milliseconds) => Jan1St1970.AddMilliseconds(milliseconds);
    }
}
