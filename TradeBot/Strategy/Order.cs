using System;
using DataTypes;

namespace TradeBot.Strategy
{
    public class Order
    {
        public OrderType Type { get; }
        public Status Status { get; set; }
        public Guid Id;

        public decimal Price;
        public decimal Amount;
        public decimal FilledAmount;
        //public List<decimal> Entryes = new();
        //public List<decimal> Stops = new();
        //public List<decimal> Takes = new();


        public Order(OrderType type, decimal price, decimal amount)
        {
            Type = type;
            Price = price;
            Amount = amount;
            Status = Status.Open;
            Id = Guid.NewGuid();
        }
    }
}
