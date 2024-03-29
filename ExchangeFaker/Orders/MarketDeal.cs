using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;

namespace ExchangeFaker.Orders
{
    public class MarketDeal : BaseDeal
    {
        protected override void Buy(Order deal)
        {
            owner.coinsAmount[Pair] += deal.Amount;
            owner.RemoveMoney(deal.Amount * deal.Price);
            deal.Amount = 0;
        }

        protected override void Sell(Order deal)
        {
            var amount = deal.Amount <= owner.coinsAmount[Pair] ? deal.Amount : owner.coinsAmount[Pair];
            deal.Amount -= amount;
            owner.coinsAmount[Pair] -= amount;
            owner.AddMoney(amount * deal.Price);
        }

        public MarketDeal(OrderType orderType, Account owner, string pair, List<Order> entry,
                     List<Order> takes = null, List<Order> stops = null) : base(owner, pair, entry, takes, stops)
        {
            if (owner.Amount < entry.Select(x => x.Amount * x.Price).Sum())
                throw new Exception("No money");

            if (orderType == OrderType.BuyLimit)
                EntryOrders.AddRange(entry.Select(x => new Order(x.Price, x.Amount, OrderType.BuyLimit)));
            else
                EntryOrders.AddRange(entry.Select(x => new Order(x.Price, x.Amount, OrderType.SellLimit)));

            if (takes != null)
                TakeOrders.AddRange(takes.Select(x => new Order(x.Price, x.Amount, OrderType.SellLimit)));

            if (stops != null)
                StopOrders.AddRange(stops.Select(x => new Order(x.Price, x.Amount, OrderType.SellLimit)));
        }
    }
}
