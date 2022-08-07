using System.Collections.Generic;
using ExchangeConnectors;

namespace ExchangeFaker
{
    public class Exchange
    {
        private List<Account> accounts = new();
        readonly IExchange exchange;

        public Exchange(IExchange exchange)
        {
            this.exchange = exchange;
        }

        public void UpdateStates()
        {
            var prices = exchange.GetPrices();
            foreach (var acc in accounts)
                acc.DataReceiver(prices);
        }

        public Account CreateAccount(string name, string defaultCurrency, decimal startBalance)
        {
            var account = new Account(name, defaultCurrency, startBalance);
            accounts.Add(account);
            return account;
        }
    }
}
