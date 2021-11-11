using System.Collections.Generic;
using BinanceApiDataPArser;

namespace BinanceApiDataParser
{
    public class ZigZag
    {
        public static List<int> CalculateZigZag(List<CandleOHLC> candles, out int obsLow, out int obsHigh, int obsStart,
            int obsEnd, float deviationInPercent)
        {
            bool swingHigh = false, swingLow = false;
            obsLow = obsHigh = obsStart;
            List<int> zigZag = new List<int>();
            for (int obs = obsStart; obs <= obsEnd; obs++)
            {
                if (candles[obs].High > candles[obsHigh].High)
                {
                    obsHigh = obs;
                    if (!swingLow &&
                        ((candles[obsHigh].High - candles[obsLow].Low) / candles[obsLow].Low) * (decimal) 100F >= (decimal) deviationInPercent)
                    {
                        zigZag.Add(obsLow); // new swinglow
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
                        zigZag.Add(obsHigh); // new swinghigh
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