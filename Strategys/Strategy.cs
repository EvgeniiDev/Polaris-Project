using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeBot
{
    internal class Strategy : IStrategy
    {

        public void Init()
        {
            EventsCatalog.PP += (Dot dot) => EntryHandler(dot);
            //выполняет подписку на необходимые события для работы стратегии
        }

        public Strategy()
        {
            Init();
        }

        public void EntryHandler(Dot dot)
        {
            var entry = GetNearestLevels(TimeFrame.D1, Direction.Both, dot, 2, LevelType.Value);
            var takes = GetNearestLevels(TimeFrame.H1, Direction.Up, dot, 5, LevelType.Struct);
            var stops = GetNearestLevels(TimeFrame.H4, Direction.Down, dot, 10, LevelType.Value);

            //Выбрать нужные?

            if (CheckRiskProfitFactor(entry, takes, stops))
            {
                SendOrder();
            }
        }

        public void StopHandler(Dot dot)
        {
        }

        public void TakeHandler()
        {
        }

        private static bool CheckRiskProfitFactor(decimal entry, decimal take, decimal stop)
        {
            return Math.Abs(take - entry) / Math.Abs(entry - stop) > 2;
        }

        private static bool CheckRiskProfitFactor(List<decimal> entry, List<decimal> take, List<decimal> stop)
        {
            var entryAvg = entry.Sum() / entry.Count;
            var takeAvg = take.Sum() / take.Count;
            var stopAvg = stop.Sum() / stop.Count;

            return Math.Abs(takeAvg - entryAvg) / Math.Abs(entryAvg - stopAvg) > 2;
        }
    }
}
