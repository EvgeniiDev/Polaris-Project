using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTypes;
using ExchangeConnectors;
using ExchangeFaker;

namespace TradeBot.OneDealOneClass
{
    public abstract class Strategy : IExchange
    {
        public TimeFrames.TimeFrame[] TimeFrames;
        public IExchange Connector;
        public Account Account;
        public string[] Pairs;
        public bool Alive = true;   //если сделка завершена, то false, иначе true

        public virtual void Init()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Start(string[] pairs, TimeFrames.TimeFrame[] timeFrames)
        {
            Pairs = pairs;
            TimeFrames = timeFrames;
            Init();
        }

        private void OpenDeal(IExchange exchange, string pair, TimeFrames.TimeFrame tf, Func<Deal, bool> func)
        {
            throw new NotImplementedException();
        }
        
        private void ChangeDeal(IExchange exchange, string pair, TimeFrames.TimeFrame tf, Func<Deal, bool> func)
        {
            throw new NotImplementedException();
        }
        
        private void CloseDeal(IExchange exchange, string pair, TimeFrames.TimeFrame tf, Func<Deal, bool> func)
        {
            throw new NotImplementedException();
        }

        public void IterationCompleted(string pair)
        {
            Account.NextPrice(pair);
        }

        #region EventsPlug
        public virtual void EventsCatalog_ReboundFromTheLevel(Core.Dot obj)
        {
            return;
        }

        public virtual void EventsCatalog_NewZigZag(Core.Events.Objects.NewZigZagEvent obj)
        {
            return;
        }

        public virtual void EventsCatalog_PP(Core.Dot obj)
        {
            return;
        }

        public virtual void EventsCatalog_NewAccumulation(Core.Accumulation obj)
        {
            return;
        }

        public virtual void EventsCatalog_MovingAverage(Core.Events.Objects.NewMovingAverageEvent obj)
        {
            return;
        }

        public virtual void EventsCatalog_NewCandle(Core.Events.Objects.NewCandleEvent obj)
        {
            return;
        }
        #endregion

        #region InterfaceMembers
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
        #endregion
    }
}
