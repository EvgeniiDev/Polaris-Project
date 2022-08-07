using DataTypes;
using Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeFaker.Orders
{
    public abstract class BaseDeal
    {
        public decimal Amount;
        protected decimal AveragePrice;
        public Status Status;
        protected decimal lastPrice;
        protected decimal Balance;
        protected decimal StartBalance = 0;

        public List<Order> TakeOrders = new();
        public List<Order> StopOrders = new();
        public List<Order> EntryOrders = new();

        public int Leverage { get; set; } = 1;
        protected decimal LiquidationPrice;
        protected Account owner { get; }
        public string Pair { get; }

        public virtual void UpdateStatusOfOrder(decimal price)
        {
            if (lastPrice == 0)
            {
                UpdateLastPrice(price);
                return;
            }

            UpdateAllDeals(price);

            if (Math.Abs(Amount) < 10e-4m && EntryOrders.All(x => x.Status == Status.Close))
            {
                CloseOrder();
                return;
            }

            if (LiquidationPrice >= price)
            {
                //Liquidation
                Status = Status.Close;
                Amount = 0;
                return;
            }
            UpdateLastPrice(price);
        }

        protected virtual void UpdateAllDeals(decimal price)
        {
            UpdateStatusOfDeals(EntryOrders, price);
            UpdateStatusOfDeals(TakeOrders, price);
            UpdateStatusOfDeals(StopOrders, price);
        }
        public void CloseOrder()
        {
            var balance = Balance - StartBalance + StartBalance / Leverage;

            if (balance >= 0)
                owner.AddMoney(balance);
            else
                owner.RemoveMoney(balance);

            Status = Status.Close;
        }

        protected virtual void UpdateStatusOfDeals(List<Order> deals, decimal price)
        {
            foreach (var deal in deals.Where(x => x.Status == Status.Open
                                                   && IsPriceCrossedLevel(x.Price, price)))
            {
                switch (deal.OrderType)
                {
                    case OrderType.BuyLimit:
                        Buy(deal);
                        break;

                    case OrderType.SellLimit:
                        Sell(deal);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                //todo добавить неточное сравнение
                if (deal.Amount == 0)
                    deal.Status = Status.Close;
            }
        }
        public BaseDeal(Account owner, string pair, List<Order> entry,
                                List<Order> takes = null, List<Order> stops = null)
        {
            if (!IsPositive(entry) || takes != null && !IsPositive(takes)
                                   || stops != null && !IsPositive(stops))
                throw new Exception("Incorrect input");

            this.owner = owner;
            Pair = pair;
            Status = Status.Open;
        }

        protected bool IsPriceCrossedLevel(decimal level, decimal price)
        {
            return lastPrice >= price && level <= lastPrice && level >= price
                   || lastPrice <= price && level >= lastPrice && level <= price;
        }

        protected static bool IsPositive(List<Order> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return dictionary.All(x => x.Amount >= 0)
                   && dictionary.All(x => x.Price >= 0);
        }

        protected void UpdateLastPrice(decimal price)
        {
            lastPrice = price;
        }

        protected abstract void Buy(Order deal);

        protected abstract void Sell(Order deal);
    }
}