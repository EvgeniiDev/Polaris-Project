using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Core.Events;
using Core.Events.Objects;
using ExchangeConnectors;

namespace ExchangeFaker
{
    //todo если свечи будут приходить очень быстро, то стратегия не будет успевать отрабатывать
    //todo этой штуке нужно написать свой контроллер на коннектор к бирже, она не должна работать через евенты ядра
    //todo сейчас вирт биржа работает в режиме фьючерсной торговли, надо сделать так, чтоб была маркет торговля(тупо ввести общий баланс монет на аккаунт)
    //todo сейчас при тестовых прогонах стратегий нет никакой обратной связью между вирт биржей и стратегией, соответсвенно может оказаться так, что стратегия ещё не закончила свою работу, а вирт биржа уже обновилась на новые данные
    public class ExchangeFaker
    {
        public List<Account> accounts { get; private set; } = new();
        private readonly IExchange exchange;
        public ExchangeFaker(IExchange exchange)
        {
            this.exchange = exchange;
            EventsCatalog.NewCandle += EventsCatalog_NewCandle;
        }

        //private void UpdateStates()
        //{
        //    EventsCatalog.NewCandle += EventsCatalog_NewCandle;
        //    var prices = exchange.GetPrices().Result;
        //    foreach (var acc in accounts)
        //        acc.DataReceiver(prices);
        //}

        //todo по умному распределять данные по аккаунтам, только тогда когда он нужен опред аккаунту,
        //т.е между стратегией и аккаунтом должна быть обратная связь
        private void EventsCatalog_NewCandle(NewCandleEvent obj)
        {
            //todo надо как-то по более умному эмулировать движение цены за этот промежуток

            //todo исправить этот дибильный вариант получения данных, или понять что он нормальный и хорошо работает
            foreach (var acc in accounts.ToArray())
            {
                Console.WriteLine(obj.Candle.High);
                acc.DataReceiver(obj);
            }
        }

        public Account CreateAccount(string name, string defaultCurrency, decimal startBalance)
        {
            var account = new Account(name, defaultCurrency, startBalance);
            lock (accounts)
            {
                accounts.Add(account);
            }
            return account;
        }

        public Connector CreateConnector(Guid id, string defaultCurrency, decimal startBalance)
        {
            var acc = CreateAccount(id.ToString(), defaultCurrency, startBalance);
            return new Connector(exchange, acc);
        }

        public Connector CreateConnector(Account acc)
        {
            return new Connector(exchange, acc);
        }
    }
}