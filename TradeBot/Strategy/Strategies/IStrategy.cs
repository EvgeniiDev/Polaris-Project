using ExchangeConnectors;

namespace TradeBot
{
    public interface IStrategy
    {
        void Stop();
        void Start(IExchange connector);
    }
}
