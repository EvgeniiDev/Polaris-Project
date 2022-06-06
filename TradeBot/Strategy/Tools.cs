using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExchangeConnectors.TimeFrames;

namespace TradeBot.Strategy
{
    public class Tools
    {
        private Dictionary<TimeFrame, Action> subcsriptions = new();

        Tools()
        {
            EventsCatalog.PP += (Dot dot) => EventHandler(dot);
            EventsCatalog.ReboundFromTheLevel += (Dot dot) => EventHandler(dot);
            EventsCatalog.Slom += (Dot dot) => EventHandler(dot);
        }

        public IEnumerable<Deal> GetDeals()
        {

        }

        private void EventHandler(Dot dot)
        {
            throw new NotImplementedException();
        }

        public void PlaceOrder(IStrategy strategy)
        {

        }

        public List<decimal> GetNearestLevels(TimeFrame d1, object both, Dot dot, int v, object value)
        {
            throw new NotImplementedException();
        }
    }
}
