using ExchangeConnectors;
using System;
using System.Collections.Concurrent;
using Core.Data;
using static DataTypes.TimeFrames;

namespace TradeBot.Strategy
{
    public static class StrategysManager
    {
        public static ConcurrentDictionary<Guid, StrategyRunner> Strategys { get; private set; } = new();
        private static ConcurrentDictionary<IExchange, ExchangeFaker.ExchangeFaker> exchangeFakers = new();
        private static ConcurrentDictionary<Guid, IExchange> connectors = new();

        public static Guid AddStrategy(IStrategy strategy, IExchange connector)
        {
            //todo добавить обертку над IExchange, которая кеширует все свечи
            var id = Guid.NewGuid();
            var fakeExchange = exchangeFakers.GetOrAdd(connector, new ExchangeFaker.ExchangeFaker(connector));

            Strategys.TryAdd(id, new StrategyRunner()
            {
                Strategy = strategy,
                Connector = fakeExchange.CreateConnector(id, "USD", 10000m),
            });

            connectors.TryAdd(id, connector);
            return id;
        }


        public static void RunStrategy(Guid guid, string[] pairs, TimeFrame[] timeFrames, DateTime startDate)
        {
            //run dataSource
            var connector = connectors[guid];
            foreach (var pair in pairs)
                foreach (var timeFrame in timeFrames)
                {
                    DataConcentrator.RegisterDataSource(connector, typeof(ExchangeFaker.ExchangeFaker), pair, timeFrame, startDate);
                    DataConcentrator.RunDataSource(typeof(ExchangeFaker.ExchangeFaker), pair, timeFrame);
                }
            //DataConcentrator.StartConcentrator();
            Strategys[guid].Run();
        }

        //todo добавить метод на остановку стратегий
        public static void StopStrategy(Guid guid)
        {
            Strategys[guid].Stop();
            //strategys.TryRemove(guid);
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
