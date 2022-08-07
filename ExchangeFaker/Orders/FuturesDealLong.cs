using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;

namespace ExchangeFaker.Orders
{
    public class FuturesDealLong : BaseDeal
    {
        private const decimal feeFactor = 1.002m;

        protected override void Buy(Order deal)
        {
            var amount = deal.Amount;

            deal.Amount -= amount;
            AveragePrice = (AveragePrice * Amount + amount * deal.Price) / (Amount + amount);
            Amount += amount;
            Balance -= amount * deal.Price;
            Console.WriteLine(Balance);
            var totalSpend = Math.Abs(AveragePrice * Amount);

            if (Math.Abs(Amount) < 0.00001m)
                LiquidationPrice = decimal.MinValue;
            else
                LiquidationPrice = (totalSpend - totalSpend / Leverage) / Math.Abs(Amount) * feeFactor;
        }

        protected override void Sell(Order deal)
        {
            if (Amount <= 0)
                return;

            var amount = deal.Amount <= Amount ? deal.Amount : Amount;
            deal.Amount -= amount;
            Amount -= amount;
            Balance += deal.Price * amount;
        }

        public FuturesDealLong(Account owner, string pair, List<Order> entry, int leverage,
            List<Order> takes = null, List<Order> stops = null)
            : base(owner, pair, entry, takes, stops)
        {
            if (leverage < 1)
                throw new Exception("Leverage should be more when 1");
            if (owner.Amount * leverage < entry.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("Not enough money");

            Leverage = leverage;

            var totalSum = entry.Sum(x => x.Price * x.Amount);
            StartBalance = totalSum;
            Balance = totalSum;
            owner.RemoveMoney(totalSum / leverage);

            EntryOrders.AddRange(entry.Select(x => new Order(x.Price, x.Amount, OrderType.BuyLimit)));

            if (takes != null)
                TakeOrders.AddRange(takes.Select(x => new Order(x.Price, x.Amount, OrderType.SellLimit)));

            if (stops != null)
                StopOrders.AddRange(stops.Select(x => new Order(x.Price, x.Amount, OrderType.SellLimit)));
        }
    }
}