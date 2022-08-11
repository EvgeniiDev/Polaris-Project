using System;
using System.Collections.Generic;
using Core;
using Core.Events;
using DataTypes;

namespace TradeBot.Strategy
{
    public class Tools
    {
        private Dictionary<TimeFrames.TimeFrame, Action> subcsriptions = new();

        Tools()
        {
            EventsCatalog.PP += (Dot dot) => EventHandler(dot);
          //  EventsCatalog.ReboundFromTheLevel += (Dot dot) => EventHandler(dot);
            EventsCatalog.Slom += (Dot dot) => EventHandler(dot);
        }

        public IEnumerable<Deal> GetDeals()
        {
            return null;
        }

        private void EventHandler(Dot dot)
        {
            throw new NotImplementedException();
        }

        public void PlaceOrder(Strategy strategy)
        {

        }

        public List<decimal> GetNearestLevels(TimeFrames.TimeFrame d1, object both, Dot dot, int v, object value)
        {
            throw new NotImplementedException();
        }
    }
}
