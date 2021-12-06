using System;
using System.Collections.Generic;
using System.Linq;

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
            if(zigZag[zigZag.Count-1].Price> candles[candles.Count - 1].High)
                zigZag.Add(new Dot(candles[candles.Count-1].TimeStamp, candles[candles.Count-1].Low));
            else
                zigZag.Add(new Dot(candles[candles.Count - 1].TimeStamp, candles[candles.Count - 1].High));

            return zigZag;
        }

        public static List<Dot> CalculateZigZag(List<Candle> candles, float deviationInPercent)
        {
            candles = candles.Where(x => x != null).ToList();
            bool swingHigh = false, swingLow = false;
            var obsLow = 0;
            var obsHigh = 0;
            var obsStart = 0;
            var zigZag = new List<Dot>();
            for (int obs = obsStart; obs < candles.Count; obs++)
            {
                var candlesHighObs = (candles[obs].High + Math.Max(candles[obs].Open, candles[obs].Close)) / 2;
                var candlesHighObsHigh =
                    (candles[obsHigh].High + Math.Max(candles[obsHigh].Open, candles[obsHigh].Close)) / 2;
                var candlesLowObsLow =
                    (candles[obsLow].Low + Math.Min(candles[obsLow].Open, candles[obsLow].Close)) / 2;
                var candlesLowObs = (candles[obs].Low + Math.Min(candles[obs].Open, candles[obs].Close)) / 2;
                if (candlesHighObs > candlesHighObsHigh)
                {
                    obsHigh = obs;
                    if (!swingLow && (((candlesHighObsHigh - candlesLowObsLow) / candlesLowObsLow)) * (decimal) 100F >=
                        (decimal) deviationInPercent)
                    {
                        zigZag.Add(new Dot(candles[obsLow].TimeStamp, candles[obsLow].Low));
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
                        zigZag.Add(new Dot(candles[obsHigh].TimeStamp, candles[obsHigh].High));
                        swingHigh = true;
                        swingLow = false;
                    }

                    if (swingHigh) obsHigh = obsLow;
                }
            }

            return zigZag;
        }
    }
}