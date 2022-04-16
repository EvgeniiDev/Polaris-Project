using System;
using System.Collections.Generic;
using System.Linq;
using TradeBot.Strategys;
using static ExchangeConnectors.TimeFrames;

namespace TradeBot
{
    internal class Strategy : IStrategy
    {
        private List<Order> orders = new();
        public Strategy()
        {
            Init();
        }

        public void Init()
        {
            EventsCatalog.PP += (Dot dot) => EntryHandler(dot);
            //выполняет подписку на необходимые события для работы стратегии
        }

        public void EntryHandler(Dot dot)
        {
            var entry = GetNearestLevels(TimeFrame.D1, Direction.Both, dot, 2, LevelType.Value);
            var takes = GetNearestLevels(TimeFrame.h4, Direction.Up, dot, 5, LevelType.Struct);
            var stops = GetNearestLevels(TimeFrame.h1, Direction.Down, dot, 10, LevelType.Value);

            //Выбрать нужные?

            if (CheckRiskProfitFactor(entry, takes, stops))
            {
                SendOrder();
            }
        }

        public void StopHandler(Dot dot)
        {
            //Реализация динамического стопа
            //Например, сделка выполнена на Н4 , я хочу пододвигать стоп ,при образовании на 
            //меньшем тф (Н1) слома или пп.
            //я подписываюсь на событие обнаружения пп\слома на этой монете,
            //далее я проверяю ,что этот урвоень выше ,Чем текущий стоп и ниже текущей цены?
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
    }
}
