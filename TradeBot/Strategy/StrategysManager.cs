using ExchangeConnectors;
using System;
using System.Collections.Generic;
using DataTypes;

namespace TradeBot.Strategy
{
    public static class StrategysManager
    {
        private static Dictionary<Guid, StrategyRunner> strategys = new ();

        public static Guid AddStrategy(IStrategy strategy, TimeFrames.TimeFrame[] timeFrame, IExchange connector)
        {
            var guid = Guid.NewGuid();

            strategys.Add(guid, new StrategyRunner()
            {
                Strategy = strategy,
                TimeFrame = timeFrame,
               // Connector = new ExchangeWrapper(connector),
            });

            return guid;
        }


        public static void RunStrategy(Guid guid)
        {
            strategys[guid].Run();
        }

        public static void StopStrategy(Guid guid)
        {
            strategys[guid].Stop();
            strategys.Remove(guid);
        }

        public static void GetStatistics(StatisticsType type)
        {
            //место расширения
            throw new NotImplementedException();
            switch (type)
            {
                case StatisticsType.WinRate:
                    break;
                case StatisticsType.ProfitFactor:
                    break;
                case StatisticsType.LossFactor:
                    break;
                default:
                    break;
            }
        }

        public enum StatisticsType
        {
            WinRate,
            ProfitFactor,
            LossFactor,
            //todo новые виды статистик
        }
    }
}
