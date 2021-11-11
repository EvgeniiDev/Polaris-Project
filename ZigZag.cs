using System;
using System.Collections.Generic;

namespace TradeBot
{
    public static class ZigZag
    {
        public static List<Dot> CalculatePriceStructLight(List<Candle> candles, float deviationInPercent)
        {
            bool trendUp = candles[1].High > candles[0].High;
            var zigZag = new List<Dot>();
            for (int n = 1; n < candles.Count; n++)
            {
                var previousCandle = candles[n - 1];
                var currentCandle = candles[n];
                if (currentCandle.High > previousCandle.High)
                {
                    if (!trendUp)
                        zigZag.Add(new Dot(previousCandle.TimeStamp, previousCandle.Low));
                    trendUp = true;
                }
                else if (currentCandle.Low < previousCandle.Low)
                {
                    if (trendUp)
                        zigZag.Add(new Dot(previousCandle.TimeStamp, previousCandle.High));
                    trendUp = false;
                }
            }

            return zigZag;
        }

        public static List<int> CalculateZigZag(List<Candle> candles, float deviationInPercent)
        {
            bool swingHigh = false, swingLow = false;
            var obsLow = 0;
            var obsHigh = 0;
            var obsStart = 0;
            List<int> zigZag = new List<int>();
            for (int obs = obsStart; obs < candles.Count; obs++)
            {
                var candlesHighObs = (candles[obs].High + Math.Max(candles[obs].Open, candles[obs].Close)) / 2;
                var candlesHighObsHigh =
                    (candles[obsHigh].High + Math.Max(candles[obsHigh].Open, candles[obsHigh].Close)) / 2;
                var candlesLowObsLow =
                    ((candles[obsLow].Low + Math.Min(candles[obsLow].Open, candles[obsLow].Close)) / 2);
                var candlesLowObs = ((candles[obs].Low + Math.Min(candles[obs].Open, candles[obs].Close)) / 2);
                if (candlesHighObs > candlesHighObsHigh)
                {
                    obsHigh = obs;
                    if (!swingLow && (((candlesHighObsHigh - candlesLowObsLow) / candlesLowObsLow)) * (decimal) 100F >=
                        (decimal) deviationInPercent)
                    {
                        zigZag.Add(obsLow);
                        swingHigh = false;
                        swingLow = true;
                    }

                    if (swingLow) obsLow = obsHigh;
                }


                else if (candlesLowObs < candlesLowObsLow)
                {
                    obsLow = obs;
                    if (!swingHigh && (((candlesHighObsHigh - candlesLowObsLow) / candlesLowObsLow) * (decimal) 100F >=
                                       (decimal) deviationInPercent))
                    {
                        zigZag.Add(obsHigh);
                        swingHigh = true;
                        swingLow = false;
                    }

                    if (swingHigh) obsHigh = obsLow;
                }
            }

            return zigZag;
        }

        public static List<Dot> GetZigZagDot(List<int> zigZag, List<Candle> candles)
        {
            var dots = new List<Dot>();
            bool lowOrHigh = zigZag[0] > zigZag[1];
            foreach (var pointer in zigZag)
            {
                Dot dot;
                if (lowOrHigh)
                {
                    dot = new Dot(candles[pointer].TimeStamp, candles[pointer].Low);
                    lowOrHigh = false;
                }
                else
                {
                    dot = new Dot(candles[pointer].TimeStamp, candles[pointer].High);
                    lowOrHigh = true;
                }

                dots.Add(dot);
            }

            return dots;
        }
    }
}