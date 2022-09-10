using ExchangeConnectors;
using System;
using System.Collections.Concurrent;
using Core.Data;
using static DataTypes.TimeFrames;
using ExchangeFaker;

namespace TradeBot
{
    public static class StrategysManager
    {
        public static ConcurrentDictionary<Guid, Strategy> Strategys { get; private set; } = new();
        private static ConcurrentDictionary<IExchange, FakeExchange> exchangeFakers = new();
        private static ConcurrentDictionary<Guid, IExchange> connectors = new();

        public static Guid AddStrategy(Strategy strategy, IExchange connector)
        {
            //todo добавить обертку над IExchange, которая кеширует все свечи
            var id = Guid.NewGuid();
            var fakeExchange = exchangeFakers.GetOrAdd(connector, new FakeExchange(connector));
            var acc = fakeExchange.CreateAccount(id.ToString(), "USD", 10000m);

            strategy.Account = acc;
            strategy.Connector = fakeExchange.CreateConnector(acc);

            Strategys.TryAdd(id, strategy);
            connectors.TryAdd(id, connector);

            return id;
        }


        public static void RunStrategy(Guid guid, string pair, TimeFrame timeFrame, DateTime startDate)
        {
            //run dataSource
            var connector = connectors[guid];
            DataConcentrator.RegisterDataSource(connector, typeof(FakeExchange), pair, timeFrame, startDate);
            DataConcentrator.RunDataSource(typeof(FakeExchange), pair, timeFrame);

            Strategys[guid].Start(pair, timeFrame);
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
