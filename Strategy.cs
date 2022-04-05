using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBot
{
    internal class Strategy: IStrategy
    {

        public void Init()
        {
            EventsCatalog.PP += () => EntryHandler();
            //выполняет подписку на необходимые события для работы стратегии
        }
        public Strategy()
        {
            //slom += EntryPatterns;
            //pp += EntryPatterns;

            //add паттерн входа от уровня
            //newLevel += EntryPatterns;
            //newBase += EntryPatterns;
        }
        public void EntryHandler()
        {

        }
        public void StopPatterns()
        {

        }
        public void TakePatterns()
        {

        }

        private bool CheckRiskProfitFactor(decimal entry, decimal take, decimal stop)
        {
            return Math.Abs(take - entry) / Math.Abs(entry - stop) > 2;
        }
    }
}
