using System;
using System.Collections.Generic;
using BinanceApiDataPArser;

namespace Alogrhythms
{
    public class BoxDetectionAlgoritm
    {
        const decimal differentInPercent = 0.05m;
        const int minCountCandlesForBox = 3;
        const int countTouchOfPrice = 3;

        public static bool Touch(int x, decimal y, decimal x1, decimal x2, decimal y1, decimal y2)
        {
            var a = y1 - y2;
            var b = x2 - x1;
            var c = x1 * y2 - x2 * y1;

            var expectedPrice = (x2 * y1 - x1 * y2 - a * x) / b;
            var priceDelta = Math.Abs(expectedPrice - y);
            if (expectedPrice <= differentInPercent * expectedPrice) return true;
            return false;
        }

        public static List<Accumulation> FindBoxes(List<CandleOHLC> candles)
        {
            var countCandles = candles.Count;
            var theLastPossibleStartOfBox = countCandles - minCountCandlesForBox + 1;
            var boxes = new List<Accumulation>();

            for (int i = 0; i < theLastPossibleStartOfBox; i++)
            {
                for (int j = i + minCountCandlesForBox - 1; j < theLastPossibleStartOfBox; j++)
                {
                    var section = candles.GetRange(i, j - i);

                    var sectionLength = section.Count;

                    var bottomWick = new List<decimal>();
                    var upperWick = new List<decimal>();
                    var sortedBottomWick = new List<decimal>();
                    var sortedUpperWick = new List<decimal>();

                    for (var k = 0; k < sectionLength; k++)
                    {
                        bottomWick.Add(candles[k].Low);
                        upperWick.Add(candles[k].High);
                        sortedBottomWick.Add(candles[k].Low);
                        sortedUpperWick.Add(candles[k].High);
                    }

                    sortedBottomWick.Sort();
                    sortedUpperWick.Sort();
                    sortedUpperWick.Reverse();
        
                    var firstHigh = sortedUpperWick[0]; // y1
                    var secondHigh = sortedUpperWick[1]; // y2
                    var firstLow = sortedBottomWick[0]; // y1
                    var secondLow = sortedBottomWick[1]; // y2

                    var firstHighIndex = Array.IndexOf(section.ToArray(), firstHigh); // x1
                    var secondHighIndex = Array.IndexOf(section.ToArray(), firstHigh); // x2
                    var firstLowIndex = Array.IndexOf(section.ToArray(), firstHigh); // x1
                    var secondLowIndex = Array.IndexOf(section.ToArray(), firstHigh); // x2

                    var countTouchHigh = 0;
                    var countTouchLow = 0;

                    for (int k = 2; k < sectionLength; k++)
		            {
                        var checkHigh = section[k].High; // y
                        var checkIndex = Array.IndexOf(section.ToArray(), firstHigh); // x
                        if (Touch(checkIndex, checkHigh, firstHighIndex, secondHighIndex, firstHigh, secondHigh)) countTouchHigh++;
                        var checkLow = section[k].Low; // y
                        if (Touch(checkIndex, checkLow, firstLowIndex, secondLowIndex, firstLow, secondLow)) countTouchLow++;
		            }

                    if (countTouchHigh > countTouchOfPrice && countTouchLow > countTouchOfPrice) boxes.Add(new Accumulation(candles[i].TimeStamp, 
                        candles[i].Low, candles[j].TimeStamp, candles[j].High, AccumulationType.Rectangle));
                }
            }
            return boxes;
        }
    }
}
