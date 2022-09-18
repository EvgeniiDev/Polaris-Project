using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Algorithms;
using Core.Events;
using Core.Events.Objects;
using DataTypes;
using Log;

namespace TradeBot.Strategies
{
    public class PPStrategy : Strategy
    {

        private NewCandleEvent lastCandle;
        private List<Candle> lastCandles = new();

        private List<string> buyOrderId = new();
        private List<string> sellOrderId = new();
        private TrendDetector trendDetector = new();
        private ZigZag ZigZag = new();

        public override void Init()
        {
            //foreach (var pair in Pairs)
            //{
            //    foreach (var timeFrame in TimeFrames)
            //    {
            //        var ma = new MovingAverage(180, MovingAverage.Type.High, pair, timeFrame);
            //        var maa = new MovingAverage(180, MovingAverage.Type.Low, pair, timeFrame);
            //    }
            //}

            EventsCatalog.NewCandle += EntryHandler;
            EventsCatalog.PP += EventsCatalog_PP;
            EventsCatalog.Slom += EventsCatalog_PP;
            //выполняет подписку на необходимые события для работы стратегии
        }

        //todo чет говно какое-то я не хочу думать эвенты какой пары и какого тф мне пришли
        // а если я хочу использовать для входа в сделку условия с нескольких тф?
        private void EventsCatalog_PP(PP obj)
        {
            Logger.SendTimerData("PP", (double)obj.Dot.Price);
            var pair = "BTCUSDT";

            if (obj.Trend == Trend.Up)
            {
                foreach (var so in sellOrderId.ToList())
                {
                    CancelOrder(pair, so);
                    sellOrderId.Remove(so);
                }

                var res = CreateOrder(pair, OrderType.Long, lastCandle.Candle.High,
                    100 / lastCandle.Candle.High);
                buyOrderId.Add(res.Result);

            }

            if (obj.Trend == Trend.Down)
            {
                foreach (var so in buyOrderId.ToList())
                {
                    CancelOrder(pair, so);
                    buyOrderId.Remove(so);
                }

                var res = CreateOrder(pair, OrderType.Short, lastCandle.Candle.High,
                    100 / lastCandle.Candle.High);
                sellOrderId.Add(res.Result);

            }
            lastCandles.Clear();
            lastCandles.Add(lastCandle.Candle);
            IterationCompleted(pair);
        }

        public void EntryHandler(NewCandleEvent candle) // на входе Event class, который содержит много разной инфы
        {
            lastCandle = candle;
            lastCandles.Add(candle.Candle);
            var zigZag = ZigZag.CalculateZigZag(lastCandles, 5);
            TrendDetector.TrendDetect(zigZag);


            Logger.SendTimerData("HighCandle", (double)candle.Candle.High);
            Logger.SendTimerData("LowCandle", (double)candle.Candle.Low);
            Logger.SendTimerData("CloseCandle", (double)candle.Candle.Close);
            Logger.SendTimerData("OpenCandle", (double)candle.Candle.Open);
        }

        private static bool CheckRiskProfitFactor(decimal entry, decimal take, decimal stop)
        {
            return Math.Abs(take - entry) / Math.Abs(entry - stop) > 2;
        }

        private static bool CheckRiskProfitFactor(List<decimal> entry, List<decimal> take, List<decimal> stop)
        {
            var entryAvg = entry.Sum() / entry.Count;
            var takeAvg = take.Sum() / take.Count;
            var stopAvg = stop.Sum() / stop.Count;

            return CheckRiskProfitFactor(entryAvg, takeAvg, stopAvg);
        }
    }
}