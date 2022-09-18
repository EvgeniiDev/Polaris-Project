using ExchangeConnectors;
using DataTypes;
using ExchangeFaker;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace TradeBot.Strategy
{
    public abstract class Strategy : IExchange
    {
        public TimeFrames.TimeFrame[] TimeFrames;
        public IExchange Connector;
        public Account Account;
        public string[] Pairs;

        public virtual void Init()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {

        }

        public void Start(string[] pairs, TimeFrames.TimeFrame[] timeFrames)
        {
            Pairs = pairs;
            TimeFrames = timeFrames;
            Init();
        }

        public void IterationCompleted(string pair)
        {
            Account.NextPrice(pair);
        }

        public Task<Dictionary<string, decimal>> GetPrices()
        {
            return Connector.GetPrices();
        }

        public Task<List<Candle>> GetCandles(string pair, TimeFrames.TimeFrame timeFrame, DateTime start, DateTime end)
        {
            return Connector.GetCandles(pair, timeFrame, start, end);
        }

        public void SubscribeOnNewKlines(string pair, TimeFrames.TimeFrame tf, Action<Kline> deleg)
        {
            Connector.SubscribeOnNewKlines(pair, tf, deleg);
        }

        public void UnsubscribeOnNewKlines(string pair, TimeFrames.TimeFrame tf, Action<Kline> deleg)
        {
            Connector.UnsubscribeOnNewKlines(pair, tf, deleg);
        }

        public Task<List<DataTypes.Order>> GetCurrentOrdersPerPair(string pair)
        {
            return Connector.GetCurrentOrdersPerPair(pair);
        }

        public Task<List<Trade>> GetTrades(string pair, TimeFrames.TimeFrame timeFrame, DateTime startTime, DateTime endTime)
        {
            return Connector.GetTrades(pair, timeFrame, startTime, endTime);
        }

        public Task<string> CreateOrder(string pair, OrderType orderType, decimal price, decimal amount)
        {
            return Connector.CreateOrder(pair, orderType, price, amount);
        }

        public Task<DataTypes.Order> GetOrderInfo(string pair, string id)
        {
            return Connector.GetOrderInfo(pair, id);
        }

        public Task CancelOrder(string pair, string orderId)
        {
            return Connector.CancelOrder(pair, orderId);
        }
    }
}
