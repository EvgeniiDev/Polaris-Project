using ExchangeConnectors;
using TradeBot.Strategy;

namespace TradeBot
{
    public interface IStrategy
    {
        public StrategyRunner StrategyRunner { get; set; }

        void Stop();
        void Start(IExchange connector);
    }
}
