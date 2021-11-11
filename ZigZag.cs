using System.Collections.Generic;
using BinanceApiDataPArser;

namespace BinanceApiDataParser
{
    public class ZigZag
    {
        public static List<int> CalculateZigZag(List<CandleOHLC> candles, float deviationInPercent)
        {
            bool swingHigh = false, swingLow = false;
            var obsLow = 0;
            var obsHigh = 0;
            var obsStart = 0;
            List<int> zigZag = new List<int>();
            for (int obs = obsStart; obs <= candles.Count; obs++)
            {
                if (candles[obs].High > candles[obsHigh].High)
                {
                    obsHigh = obs;
                    if (!swingLow &&
                        ((candles[obsHigh].High - candles[obsLow].Low) / candles[obsLow].Low) * (decimal) 100F >= (decimal) deviationInPercent)
                    {
                        zigZag.Add(obsLow);
                        swingHigh = false;
                        swingLow = true;
                    }

                    if (swingLow) obsLow = obsHigh;
                }
                else if (candles[obs].Low < candles[obsLow].Low)
                {
                    obsLow = obs;
                    if (!swingHigh &&
                        ((candles[obsHigh].High - candles[obsLow].Low) / candles[obsLow].Low) * (decimal) 100F >=
                        (decimal) deviationInPercent)
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

        public static List<Dot> GetZigZagDot(List<int> zigZag, List<CandleOHLC> candles)
        {
            var dots = new List<Dot>();
            var lowOrHigh = true;
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