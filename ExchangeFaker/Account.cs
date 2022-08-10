using DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeFaker.Orders;
using Log;
using System.Collections.Concurrent;
using Core.Events.Objects;

namespace ExchangeFaker
{
    public class Account
    {
        readonly string DefaultCurrency;
        private static decimal counter = 0;
        private static int lon;
        private static int shor;
        public string Name { get; private set; }
        public decimal Amount { get; private set; }
        public List<decimal> BalanceHistory { get; private set; } = new();
        public ConcurrentDictionary<Guid, BaseDeal> Orders { get; private set; } = new();
        public ConcurrentDictionary<string, decimal> coinsAmount { get; private set; } = new();

        private ConcurrentDictionary<string, Queue<NewCandleEvent>> candleQueue = new();


        public Account(string name, string defaultCurrency, decimal startBalance)
        {
            Name = name;
            Amount = startBalance;
            DefaultCurrency = defaultCurrency;
            lock (BalanceHistory)
            {
                BalanceHistory.Add(startBalance);
            }
        }

        public Guid PostMarketOrder(OrderType orderType, string pair, List<Order> prices,
            List<Order> takes = null, List<Order> stops = null)
        {
            var order = new MarketDeal(orderType, this, pair, prices, takes, stops);
            var guid = Guid.NewGuid();
            coinsAmount.TryAdd(pair, 0);
            Orders.TryAdd(guid, order);

            return guid;
        }

        public Guid PostFuturesOrder(OrderType orderType, string pair, int leverage, List<Order> prices,
                    List<Order> takes = null, List<Order> stops = null)
        {
            BaseDeal order = orderType == OrderType.Long ?
                                new FuturesDealLong(this, pair, prices, leverage, takes, stops)
                              : new FuturesDealShort(this, pair, prices, leverage, takes, stops);
            var guid = Guid.NewGuid();

            Orders.TryAdd(guid, order);

            if (orderType == OrderType.Long)
            {
                counter += prices.Sum(x => x.Amount);
                lon++;
            }
            else if (orderType == OrderType.Short)
            {
                counter -= prices.Sum(x => x.Amount);
                shor++;
            }

            Console.WriteLine($"{counter} {lon} {shor}");
            return guid;
        }

        public void RemoveMoney(decimal v)
        {
            Amount -= v;
            lock (BalanceHistory)
            {
                BalanceHistory.Add(Amount);
            }
            Logger.SendTimerData("Balance", (double)Amount);
        }

        public void AddMoney(decimal v)
        {
            Amount += v;
            lock (BalanceHistory)
            {
                BalanceHistory.Add(Amount);
            }
            Logger.SendTimerData("Balance", (double)Amount);
        }

        public void ChangeStops(Guid orderId, List<Order> stops)
        {
            if (!Orders.ContainsKey(orderId))
                throw new Exception("Unknown orderId");

            Orders[orderId].StopOrders = stops;
        }

        public void ChangeEntryes(Guid orderId, List<Order> entryes)
        {
            if (!Orders.ContainsKey(orderId))
                throw new Exception("Unknown orderId");

            Orders[orderId].EntryOrders = entryes;
        }

        public void ChangeTakes(Guid orderId, List<Order> takes)
        {
            if (!Orders.ContainsKey(orderId))
                throw new Exception("Unknown orderId");

            Orders[orderId].TakeOrders = takes;
        }

        //todo понять,правильно ли работает этот метод и исправить
        public void CancelOrder(Guid OrderId)
        {
            var order = Orders[OrderId];
            order.Status = Status.Close;
            order.CloseOrder();
            //order.
            // throw new NotImplementedException();
        }

        public void NextPrice(string pair)
        {
            //todo надо как-то по более умному эмулировать движение цены за этот промежуток
            if (candleQueue.TryGetValue(pair, out var pairCandleQueue) && pairCandleQueue.Count > 0)
            {
                var candle = pairCandleQueue.Dequeue();
                foreach (var order in Orders.Values.Where(x => x.Status == Status.Open)
                                                    .Where(x => x.Pair == pair))
                {
                    //todo подумать а ничего ли страшного нет в том, что сначала лоу цена отправляется а потом хай
                    order.UpdateStatusOfOrder(candle.Candle.Low);
                    order.UpdateStatusOfOrder(candle.Candle.High);
                }
            }
        }

        internal void DataReceiver(NewCandleEvent candleEvent)
        {
            var queue = candleQueue.GetOrAdd(candleEvent.Pair, new Queue<NewCandleEvent>());
            queue.Enqueue(candleEvent);
        }
    }
}
