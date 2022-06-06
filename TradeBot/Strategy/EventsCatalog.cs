using System;

namespace TradeBot
{
    internal static class EventsCatalog
    {
        public static event Action<Dot> PP;

        public static event Action<Dot> Slom;

        public static event Action<Dot> ReboundFromTheLevel;
    }
}
