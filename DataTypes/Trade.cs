namespace DataTypes
{
    public class Trade
    {
        public Trade(decimal price, decimal amount)
        {
            Amount = amount;
            Price = price;
        }

        public decimal Price;
        public decimal Amount;
    }
}