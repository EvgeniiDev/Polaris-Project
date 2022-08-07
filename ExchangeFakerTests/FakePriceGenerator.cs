using DataTypes;
using ExchangeConnectors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeFaker
{
    public class FakePriceGenerator : IExchange
    {

        public Dictionary<string, decimal> prices = new();

        public Task CancelOrder(string pair, string orderId)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateOrder(string pair, OrderType orderType, decimal price, decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task<List<Candle>> GetCandles(string pair, TimeFrames.TimeFrame timeFrame, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetCurrentOrdersPerPair(string pair)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderInfo(string pair, string id)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, decimal> GetPrices()
        {
            return prices;
        }

        public Task<List<Trade>> GetTrades(string pair, TimeFrames.TimeFrame timeFrame, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOnNewKlines(string pair, TimeFrames.TimeFrame tf, Action<Kline> deleg)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeOnNewKlines(string pair, TimeFrames.TimeFrame tf, Action<Kline> deleg)
        {
            throw new NotImplementedException();
        }

        Task<Dictionary<string, decimal>> IExchange.GetPrices()
        {
            throw new NotImplementedException();
        }
    }
}