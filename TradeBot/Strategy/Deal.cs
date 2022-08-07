using ExchangeConnectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTypes;

namespace TradeBot.Strategy
{
    public class Deal
    {
        private OrderType Type;
        private IExchange _exchange;
        private string _ticker;
        public Status Status;
        public TimeFrames.TimeFrame TimeFrame;
        public decimal Amount { get; private set; }

        public List<Order> Entries = new();
        public List<Order> Stops = new();
        public List<Order> Takes = new();
        public Guid Id = Guid.NewGuid();

        private Dictionary<Guid, string> _tableOfCorrespondence = new();


        public Deal(string ticker, OrderType type, List<Order> entries, List<Order> stops, List<Order> takes, TimeFrames.TimeFrame timeframe)
        {
            throw new NotImplementedException();
            _ticker = ticker;
            Type = type;

            Entries = entries;
            Stops = stops;
            Takes = takes;
            TimeFrame = timeframe;
            //order = new Order(type, entryes, stops,takes);
        }


        private void UpdateDeal()
        {
            var isAllClosed = Entries.Union(Stops).Union(Takes).All(x => x.Status == Status.Close);

            if(isAllClosed)
                Status = Status.Close;
        }

        private async Task UpdateOrders()
        {
            // и как теперь реализовыввать систему зависимости частоты обновляения от расстояния до ордера? -- потом сделаю
            // мб перетащить это в ордер?
            // нет. в ордер нельзя перетаскивать. 
            // а вдруг я извне захочу узнать о заполненности сделки
            //EventsCatalog.
            //Возможно Amount считается не верно. надо проверять
            for (var num = 0; num < Entries.Count; num++)
            {
                var order = Entries[num];

                if (order.Status == Status.Close)
                    continue;

                var id = _tableOfCorrespondence[order.Id];
                var newInfo = await _exchange.GetOrderInfo(_ticker, id);


                Entries[num].Status = newInfo.Status;
                Entries[num].FilledAmount = newInfo.FilledAmount;
                Amount += newInfo.Amount;
            }

            for (var num = 0; num < Stops.Count; num++)
            {
                var order = Stops[num];

                if (order.Status == Status.Close)
                    continue;

                var id = _tableOfCorrespondence[order.Id];
                var newInfo = await _exchange.GetOrderInfo(_ticker, id);


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
                var newInfo = await _exchange.GetOrderInfo(_ticker, id);


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
