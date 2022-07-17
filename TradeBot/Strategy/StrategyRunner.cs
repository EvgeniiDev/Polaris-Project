using ExchangeConnectors;
using System.Collections.Generic;
using DataTypes;

namespace TradeBot.Strategy
{
    internal class StrategyRunner
    {
        public IStrategy Strategy;
        public TimeFrames.TimeFrame[] TimeFrame;
        public IExchange Connector;

        private List<Deal> deals = new();

        public void Stop()
        {
            Strategy.Stop();
          //  Connector.Stop();
        }

        internal void Run()
        {
            Strategy.Start(Connector);
        }
    }
}
