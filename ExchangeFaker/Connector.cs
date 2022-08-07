using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTypes;
using ExchangeConnectors;
using static DataTypes.TimeFrames;

namespace ExchangeFaker
{
    public class Connector : IExchange
    {
        private readonly IExchange _connector;
        private readonly Account _acc;

        public Connector(IExchange connector, Account acc)
        {
            _connector = connector;
            _acc = acc;
        }

        public Task CancelOrder(string pair, string orderId)
        {
            return Task.Run(() => _acc.CancelOrder(Guid.Parse(orderId)));
        }

        public Task<string> CreateOrder(string pair, OrderType orderType, decimal price, decimal amount)
        {
            //добавить нормальную поддержку ордеров всех
            switch (orderType)
            {
                case OrderType.BuyLimit:
                    return Task.Run(() =>
                        _acc.PostMarketOrder(orderType, pair, new List<Order> { new Order(price, amount) }).ToString());
                case OrderType.SellLimit:
                    return Task.Run(() =>
                        _acc.PostMarketOrder(orderType, pair, new List<Order> { new Order(price, amount) }).ToString());
                case OrderType.Long:
                    return Task.Run(() =>
                        _acc.PostFuturesOrder(orderType, pair, 1, new List<Order> { new Order(price, amount) }).ToString());
                case OrderType.Short:
                    return Task.Run(() =>
                        _acc.PostFuturesOrder(orderType, pair, 1, new List<Order> { new Order(price, amount) }).ToString());

                default:
                    throw new Exception("другие типы ордеров не умею(");
            }
        }

        public Task<List<Candle>> GetCandles(string pair, TimeFrame timeFrame, DateTime start, DateTime end)
        {
            return _connector.GetCandles(pair, timeFrame, start, end);
        }

        public Task<List<Order>> GetCurrentOrdersPerPair(string pair)
        {
            var t = _acc.Orders.Values.Where(x => x.Pair == pair).ToList();
            return Task.Run(() =>
                t.SelectMany(x => x.TakeOrders).Union(t.SelectMany(x => x.StopOrders))
                    .Union(t.SelectMany(x => x.EntryOrders)).ToList());
        }

        public Task<Order> GetOrderInfo(string pair, string id)
        {
            return Task.Run(() => _acc.Orders[Guid.Parse(id)].EntryOrders.First());
        }

        public Task<Dictionary<string, decimal>> GetPrices()
        {
            return _connector.GetPrices();
        }

        public Task<List<Trade>> GetTrades(string pair, TimeFrame timeFrame, DateTime startTime, DateTime endTime)
        {
            return _connector.GetTrades(pair, timeFrame, startTime, endTime);
        }

        public void SubscribeOnNewKlines(string pair, TimeFrame tf, Action<Kline> deleg)
        {
            _connector.SubscribeOnNewKlines(pair, tf, deleg);
        }

        public void UnsubscribeOnNewKlines(string pair, TimeFrame tf, Action<Kline> deleg)
        {
            _connector.UnsubscribeOnNewKlines(pair, tf, deleg);
        }
    }
}