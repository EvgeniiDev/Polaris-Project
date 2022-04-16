using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueRealExchange.Orders;

namespace TradeBot.Strategys
{
    internal class Order
    {
        public OrderType type { get; }
        public List<decimal> Entryes = new();
        public List<decimal> Stops = new();
        public List<decimal> Takes = new();
        public Status Status { get; set; }

        public Order(OrderType type, List<decimal> entryes, List<decimal> stops, List<decimal> takes)
        {
            this.type = type;
            Entryes = entryes;
            Stops = stops;
            Takes = takes;
            Status = Status.Open;
        }
    }
}
