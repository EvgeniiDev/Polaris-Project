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
        public void EntryPatterns()
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
