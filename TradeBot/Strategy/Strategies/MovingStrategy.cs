using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using ExchangeConnectors;
using ExchangeConnectors.Connectors;
using ExchangeFaker;
using TradeBot.Algorithms.MA;
using TradeBot.Events;
using TradeBot.Events.Objects;

namespace TradeBot.Strategy.Strategies
{
    internal class SimpleStrategy : IStrategy
    {
        private List<decimal> _maHigh = new();
        private List<decimal> _maLow = new();
        private NewCandleEvent lastCandle;
        private NewCandleEvent prevlastCandle;
        static string key = "70boOKYkASoJPBAjbjkMWU6UdMCy9T3NserZaaIkSiBpSFFRnL8bzJsxBaLZMWey";
        static string secret = "MrA8gCZzQUZaW7mdvGhnErlgoYw8OTr7Ge0l4Z8OlYX5oGcrbOgaLodCT9nQ1iR7";
        private Exchange exch = new(new BinanceConnector(key, secret));
        private IExchange connector;

        private string buyOrderId;
        private string sellOrderId;


        public SimpleStrategy()
        {
            //todo просто в мой ексчендж фейкер надо кинуть делегат, который будет подписан на получение свечи и все
            connector = exch.CreateConnector("USD", 10020m);
            Init();
        }

        private void Init()
        {
            var ma = new MovingAverage(180, MovingAverage.Type.High, "BTCUSDT", TimeFrames.TimeFrame.h1);
            var maa = new MovingAverage(180, MovingAverage.Type.Low, "BTCUSDT", TimeFrames.TimeFrame.h1);
            EventsCatalog.NewCandle += EntryHandler;
            EventsCatalog.MovingAverage += MAEntryHandler;
            //выполняет подписку на необходимые события для работы стратегии
        }


        //todo чет говно какое-то я не хочу думать эвенты какой пары и какого тф мне пришли
        private void MAEntryHandler(NewMovingAverageEvent obj)
        {
            var type = obj.type;
            var val = obj.value;
            var pair = obj.Pair;

            if (obj.Pair != "BTCUSDT" || obj.TimeFrame != TimeFrames.TimeFrame.h1)
                return;

            if (type == MovingAverage.Type.High)
                _maHigh.Add(val);
            else if (type == MovingAverage.Type.Low)
                _maLow.Add(val);

            if (_maHigh.Count > 0 && lastCandle != null && _maHigh.Last() >= lastCandle.Candle.Low &&
                _maHigh.Last() <= lastCandle.Candle.High)
            {
                var res = connector.CreateOrder(pair, OrderType.BuyLimit, lastCandle.Candle.High,
                    10 / lastCandle.Candle.High);
                buyOrderId = res.Result;
                if (sellOrderId != null)
                    connector.CancelOrder(pair, sellOrderId);
            }


            if (_maLow.Count > 0 && lastCandle != null && _maLow.Last() >= lastCandle.Candle.Low &&
                _maLow.Last() <= lastCandle.Candle.High)
            {
                if (buyOrderId != null)
                    connector.CancelOrder(pair, buyOrderId);
                var res = connector.CreateOrder(pair, OrderType.SellLimit, lastCandle.Candle.High,
                    10 / lastCandle.Candle.High);
            }
        }

        private bool IsHighCrossLow()
        {
            return _maHigh.Count > 1 && _maHigh[^1] >= _maLow[^1] && _maHigh[^0] <= _maLow[^0];
        }

        private bool IsLowCrossHigh()
        {
            return _maHigh.Count > 1 && _maHigh[^1] <= _maLow[^1] && _maHigh[^0] >= _maLow[^0];
        }

        private bool IsTouch()
        {
            return _maHigh.Count > 0 && _maHigh[^0] == _maLow[^0];
        }

        public void EntryHandler(NewCandleEvent candle) // на входе Event class, который содержит много разной инфы
        {
            prevlastCandle = lastCandle;
            lastCandle = candle;
            /*foreach(var deal in deals.Where(x=>x.Status!= Status.Close))
            {
                var tk = deal.Takes;
                var st = deal.Stops;
                var ent = deal.Entryes;
                if (deal.TimeFrame == TimeFrames.TimeFrame.D1)
                {
                    var entry = Tools.GetNearestLevels(TimeFrames.TimeFrame.D1, Direction.Both, dot, 2, LevelType.Value);
                    var takes = Tools.GetNearestLevels(TimeFrames.TimeFrame.h4, Direction.Up, dot, 5, LevelType.Struct);
                    var stops = Tools.GetNearestLevels(TimeFrames.TimeFrame.h1, Direction.Down, dot, 10, LevelType.Value);

                    if (CheckRiskProfitFactor(entry, takes, stops))
                    {
                        //SendOrder();
                    }
                }
            }*/
        }

        public void StopHandler(Dot dot)
        {
            //Реализация динамического стопа
            //Например, сделка выполнена на Н4 , я хочу пододвигать стоп ,при образовании на 
            //меньшем тф (Н1) слома или пп.
            //я подписываюсь на событие обнаружения пп\слома на этой монете,
            //далее я проверяю ,что этот уровень выше ,чем текущий стоп и ниже текущей цены?

            //тогда я отправляю запрос на биржу на изменение цены стопа
            //но как мне отправить запрос на биржу?
            //как биржа поймет какой именно ордер надо менять?
            //я где-то храню ид ордеров?
            //мне где-то нужно хранить локальные копии всех сделок и их ид на бирже,
            //но как я буду синхронизировать их с биржей?
            //я могу получить с биржи все заявки, но мне это не поможет 
            //т.к не получится сопоставить заявку с конкретным ордером
            //поэтому я буду локально хранить ордер
            //ордер в этом контексте будет называться группа из заявок на покупку и продажу 
            //связанные в рамках одной торговой ситуации
            //далее имея их id я могу поддерживать их в актуальном состоянии при необходимости
            //перед тем как двигать стоп или ещё что-то я буду сначала обновлять состояние сделок
            //в текущем ордере т.е должен быть список локальных ордеров.
            //и только после этого принимать решение об сдвигании.
        }


        public void TakeHandler()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetStops()
        {
            throw new NotImplementedException();
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

            return CheckRiskProfitFactor(entryAvg, takeAvg, stopAvg);
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Start(IExchange connector)
        {
            throw new NotImplementedException();
        }
    }
}