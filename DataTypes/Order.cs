namespace DataTypes
{
    public class Order
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal FilledAmount { get; set; }
        public OrderType OrderType { get; set; }
        public Status Status { get; set; }

        public Order(decimal price, decimal amount)
        {
            Price = price;
            Amount = amount;
        }
        //нафига нужен такой конструктор?

        public Order(decimal price, decimal amount, OrderType orderType, Status status = Status.Open)
        {
            Price = price;
            Amount = amount;
            OrderType = orderType;
            Status = status;
        }

        public Order(decimal price, decimal baseAmount, decimal quoteAmount, OrderType orderType,
            Status status = Status.Open)
        {
            Price = price;
            Amount = baseAmount;
            FilledAmount = quoteAmount;
            OrderType = orderType;
            Status = status;
        }

        public Order(decimal price, int baseAmount)
        {
            Price = price;
            Amount = baseAmount;
        }

        public override string ToString()
        {
            return $"{OrderType} {Amount} coins for {Price} every, ({Status}) ({FilledAmount}/{Amount})";
        }
    }
}