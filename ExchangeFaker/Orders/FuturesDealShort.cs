using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;

namespace ExchangeFaker.Orders
{
    public class FuturesDealShort : BaseDeal
    {
        private const decimal feeFactor = 1.002m;

        public override void UpdateStatusOfOrder(decimal price)
        {
            if (lastPrice == 0)
            {
                UpdateLastPrice(price);
                return;
            }
            UpdateAllDeals(price);

            if (Math.Abs(Amount) < 10e-3m && EntryOrders.All(x => x.Status == Status.Close))
            {
                CloseOrder();
                return;
            }
            //if(EntryDeals.All(x => x.Status == Status.Close)
            //    && TakeDeals.All(x => x.Status == Status.Close)
            //    && StopDeals.All(x => x.Status == Status.Close))
            //    CloseOrder();
            if (LiquidationPrice <= price)
            {
                //Liquidation
                //CloseOrder();
                Status = Status.Close;
                return;
               // Amount = 0;
            }
            UpdateLastPrice(price);
        }

        protected override void UpdateAllDeals(decimal price)
        {
            UpdateStatusOfDeals(EntryOrders, price);
            UpdateStatusOfDeals(TakeOrders, price);
            UpdateStatusOfDeals(StopOrders, price);
        }


        protected override void Buy(Order deal)
        {
            var amount = deal.Amount;
            deal.Amount -= amount;
            Amount += amount;
            var basePrice = AveragePrice * amount;
            Balance += basePrice - amount * deal.Price;
        }

        protected override void Sell(Order deal)
        {
            AveragePrice = (AveragePrice * Amount + deal.Amount * deal.Price) /
                                                        (Amount + deal.Amount);
            var amount = deal.Amount;
            deal.Amount -= amount;
            Amount -= amount;
            var priceOfSell = amount * deal.Price;

            var totalSpend = Math.Abs(AveragePrice * Amount);
            if (Math.Abs(Amount) < 0.00001m)
                LiquidationPrice = decimal.MaxValue;
            else
                LiquidationPrice = (totalSpend + totalSpend / Leverage) / Math.Abs(Amount) * feeFactor;
            Balance += priceOfSell;
        }

        public FuturesDealShort(Account owner, string pair, List<Order> entry, int leverage,
                                List<Order> takes = null, List<Order> stops = null) 
            : base(owner, pair, entry, takes, stops)
        {
            if (leverage < 1)
                throw new Exception("Leverage should be more when 1");
            if (owner.Amount * leverage < entry.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("No money");

            Leverage = leverage;
            LiquidationPrice = decimal.MaxValue;

            var totalSum = entry.Sum(x => x.Price * x.Amount);
            StartBalance = totalSum;
            owner.RemoveMoney(totalSum/leverage);

            EntryOrders.AddRange(entry.Select(x => 
                        new Order(x.Price, x.Amount, OrderType.SellLimit)));
            if (takes != null)
                TakeOrders.AddRange(takes.Select(x => 
                            new Order(x.Price, x.Amount, OrderType.BuyLimit)));
            if (stops != null)
                StopOrders.AddRange(stops.Select(x => 
                            new Order(x.Price, x.Amount, OrderType.BuyLimit)));
        }
    }
}