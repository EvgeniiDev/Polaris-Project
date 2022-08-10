using ExchangeConnectors;
using DataTypes;
using ExchangeFaker;

namespace TradeBot.Strategy
{
    public class StrategyRunner
    {
        public IStrategy Strategy;
        public TimeFrames.TimeFrame[] TimeFrame;
        public IExchange Connector;
        public Account account;

        public void Stop()
        {
            Strategy.Stop();
          //  Connector.Stop();
        }

        internal void Run()
        {
            Strategy.Start(Connector);
        }

        public void IterationCompleted(string pair)
        {
            account.NextPrice(pair);
        }
    }
}
