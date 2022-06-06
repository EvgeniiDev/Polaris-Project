

using static DataObjects;

namespace ExchangeConnectors
{
    public class ExchangeOrder
    {
        public OrderType Type { get; }
        public Status Status { get; set; }
        public string Id;//хз мб вообще удалить?

        public decimal Price;
        public decimal Amount;
        public decimal FilledAmount;


        public ExchangeOrder(OrderType type, decimal price, decimal amount)
        {
            Type = type;
            Price = price;
            Amount = amount;
            Status = Status.Open;
        }
    }
}
