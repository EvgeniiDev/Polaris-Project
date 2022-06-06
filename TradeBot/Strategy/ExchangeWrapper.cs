using ExchangeConnectors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradeBot.Strategy
{
    public class ExchangeWrapper : IExchange
    {
        private IExchange exchangeConnector;

        public ExchangeWrapper(IExchange exchange)
        {
            exchangeConnector = exchange;
        }

        public Task<List<ExchangeConnectors.Candle>> GetCandles(string pair, TimeFrames.TimeFrame timeFrame, DateTime start, DateTime end)
        {
            return exchangeConnector.GetCandles(pair, timeFrame, start, end);
        }

        public Task<IEnumerable<ExchangeOrder>> GetCurrentDeals(string ticker)
        {
            return exchangeConnector.GetCurrentDeals(ticker);
        }

        public Task<ExchangeOrder> GetOrderInfo(string id)
        {
            return exchangeConnector.GetOrderInfo(id);
        }

        public Dictionary<string, decimal> GetPrices()
        {
            return exchangeConnector.GetPrices();
        }

        public void Stop()
        {
            exchangeConnector.Stop();
        }
    }
}
