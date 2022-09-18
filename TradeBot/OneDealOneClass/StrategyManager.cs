using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Core.Data;
using Core.Events;
using ExchangeConnectors;
using ExchangeFaker;
using Prometheus.DotNetRuntime.EventListening.Parsers;
using static DataTypes.TimeFrames;

namespace TradeBot.OneDealOneClass
{
    public class StrategysManager
    {
        //todo из коробки подписан на все события, внутри себя содержит экземпляры актуальных и старых стратегий
        // при получении события проходится по своим стратегиям, находит, для каких эта инфа актуально и отправляет в них
        public static ConcurrentDictionary<Guid, Strategy> Strategys { get; private set; } = new();
        //хз что для эвента ключем сделать
        private static ConcurrentDictionary<IExchange, FakeExchange> exchangeFakers = new();
        private static ConcurrentDictionary<Guid, IExchange> connectors = new();

        //как менеджер поймет, что его дочерний класс стартегии нужно прибить?
        
        public StrategysManager()
        {
            EventsCatalog.NewCandle += EventsCatalog_NewCandle;
            EventsCatalog.MovingAverage += EventsCatalog_MovingAverage;
            EventsCatalog.NewAccumulation += EventsCatalog_NewAccumulation;
            EventsCatalog.PP += EventsCatalog_PP;
            EventsCatalog.NewZigZag += EventsCatalog_NewZigZag;
            EventsCatalog.ReboundFromTheLevel += EventsCatalog_ReboundFromTheLevel;
        }

        private void EventsCatalog_ReboundFromTheLevel(Core.Dot obj)
        {
            foreach (var strategy in Strategys)
                strategy.Value.EventsCatalog_ReboundFromTheLevel(obj);
        }

        private void EventsCatalog_NewZigZag(Core.Events.Objects.NewZigZagEvent obj)
        {
            throw new NotImplementedException();
        }

        private void EventsCatalog_PP(Core.PP obj)
        {
            throw new NotImplementedException();
        }

        private void EventsCatalog_NewAccumulation(Core.Accumulation obj)
        {
            throw new NotImplementedException();
        }

        private void EventsCatalog_MovingAverage(Core.Events.Objects.NewMovingAverageEvent obj)
        {
            throw new NotImplementedException();
        }

        private void EventsCatalog_NewCandle(Core.Events.Objects.NewCandleEvent obj)
        {
           // foreach(var strategy in StrategysEvents[typeof(obj)])
           // {
            //    strategy.EmitEvent();
                //выглядит как дерьмо , ведь для каждого эвента нужен свой метод
           // }
        }

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

            Strategys[guid].Start(new[] { pair }, new[] { timeFrame });
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
