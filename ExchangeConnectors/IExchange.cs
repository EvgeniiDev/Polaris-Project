using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ExchangeConnectors.TimeFrames;

namespace ExchangeConnectors
{
    public interface IExchange
    {
        public Dictionary<string, decimal> GetPrices();
        public Task<List<Candle>> GetCandles(string pair, TimeFrame timeFrame, DateTime start, DateTime end);

        //GetVolume
        //PlaceOrder
        //getorderbook
        public Task<IEnumerable<ExchangeOrder>> GetCurrentDeals(string ticker);//нужно добавить какую-то свою структуру - обертку
        public Task<ExchangeOrder> GetOrderInfo(string id);
        void Stop();
    }
}
