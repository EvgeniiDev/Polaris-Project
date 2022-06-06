using ExchangeConnectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DataObjects;
using static ExchangeConnectors.TimeFrames;

namespace TradeBot.Strategy
{
    internal class Deal
    {
        private OrderType Type;
        private IExchange _exchange;
        private string _ticker;
        public Status Status;
        public TimeFrame TimeFrame;
        public decimal Amount { get; private set; }

        public List<Order> Entryes = new();
        public List<Order> Stops = new();
        public List<Order> Takes = new();
        public Guid Id = Guid.NewGuid();

        private Dictionary<Guid, string> _tableOfCorrespondence = new();


        public Deal(string ticker, OrderType type, List<Order> entryes, List<Order> stops, List<Order> takes, TimeFrame timeframe)
        {
            throw new NotImplementedException();
            _ticker = ticker;
            Type = type;

            Entryes = entryes;
            Stops = stops;
            Takes = takes;
            TimeFrame = timeframe;
            //order = new Order(type, entryes, stops,takes);
        }


        private void UpdateDeal()
        {
            var isAllClosed = Entryes.Union(Stops).Union(Takes).All(x => x.Status == Status.Close);

            if(isAllClosed)
                Status = Status.Close;
        }

        private async Task UpdateOrders()
        {
            // и как теперь реализовыввать систему зависимости частоты обновляения от расстояния до ордера? -- потом сделаю
            // мб перетащить это в ордер?
            // нет. в ордер нельзя перетаскивать. 
            // а вдруг я извне захочу узнать о заполненности сделки
            
            //Возможно Amount считается не верно. надо проверять
            for (var num = 0; num < Entryes.Count; num++)
            {
                var order = Entryes[num];

                if (order.Status == Status.Close)
                    continue;

                var id = _tableOfCorrespondence[order.Id];
                var newInfo = await _exchange.GetOrderInfo(id);


                Entryes[num].Status = newInfo.Status;
                Entryes[num].FilledAmount = newInfo.FilledAmount;
                Amount += newInfo.Amount;
            }

            for (var num = 0; num < Stops.Count; num++)
            {
                var order = Stops[num];

                if (order.Status == Status.Close)
                    continue;

                var id = _tableOfCorrespondence[order.Id];
                var newInfo = await _exchange.GetOrderInfo(id);


                Stops[num].Status = newInfo.Status;
                Stops[num].FilledAmount = newInfo.FilledAmount;

                Amount -= newInfo.Amount;
            }

            for (var num = 0; num < Takes.Count; num++)
            {
                var order = Takes[num];

                if (order.Status == Status.Close)
                    continue;

                var id = _tableOfCorrespondence[order.Id];
                var newInfo = await _exchange.GetOrderInfo(id);


                Takes[num].Status = newInfo.Status;
                Takes[num].FilledAmount = newInfo.FilledAmount;

                Amount -= newInfo.Amount;
            }
        }
        //класс сделки, существует только в рамках жизни одной торговой стратегии, создается
        //классом стратегии, содержит в себе информацию о стопах, тейках. входах
        //является самодостаточным классом, который сам поддерживает себя в актуальном состоянии
        //
        //


    }
}
