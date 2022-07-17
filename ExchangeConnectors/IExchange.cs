using DataTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DataTypes.TimeFrames;

namespace ExchangeConnectors
{
    public interface IExchange
    {
        public Task<Dictionary<string, decimal>> GetPrices();

        public Task<List<Candle>> GetCandles(string pair, TimeFrame timeFrame, DateTime start, DateTime end);
        public void SubscibeOnNewKlines(string ticker, TimeFrame tf, Action<Kline> deleg);
        public void UnsubscibeOnNewKlines(string ticker, TimeFrame tf, Action<Kline> deleg);

        public Task<List<Order>> GetCurrentOrdersPerPair(string ticker);
        public Task<List<Trade>> GetTrades(string pair, TimeFrame timeFrame, DateTime startTime, DateTime endTime);

        public Task<string> CreateOrder(string pair, OrderType orderType, decimal price, decimal amount);
        public Task<Order> GetOrderInfo(string pair, string id);
        public Task CancelOrder(string pair, string orderId);
    }
}
