using System;
using System.Collections.Generic;

namespace TradeBot
{
    public static class BoxDetectionAlgoritm
    {
        const decimal differentInPercent = 0.01m;
        const int minCountCandlesForBox = 10;
        const int countTouchOfPrice = 5;

        public static List<List<decimal>> FindPointsForLine(List<Candle> candlesPrices)
        {
            var firstHighPrice = new List<decimal>() { 0, 0 }; // index, price
            var secondHighPrice = new List<decimal>() { 0, 0 };
            var firstLowPrice = new List<decimal>() { 0, decimal.MaxValue };
            var secondLowPrice = new List<decimal>() { 0, decimal.MaxValue };

            for (int i = 0; i < candlesPrices.Count; i++)
            {
                if (candlesPrices[i].High > firstHighPrice[1] && Math.Abs(i - firstHighPrice[0]) > 1)
                    (firstHighPrice, secondHighPrice) = (new List<decimal> { i, candlesPrices[i].High }, firstHighPrice);
                else if (candlesPrices[i].High > secondHighPrice[1] && Math.Abs(i - secondHighPrice[0]) > 1)
                    secondHighPrice = new List<decimal> { i, candlesPrices[i].High };

                if (candlesPrices[i].Low < firstLowPrice[1] && Math.Abs(i - firstLowPrice[0]) > 1)
                    (firstLowPrice, secondLowPrice) = (new List<decimal> { i, candlesPrices[i].Low }, firstLowPrice);
                else if (candlesPrices[i].Low < secondHighPrice[1] && Math.Abs(i - secondLowPrice[0]) > 1)
                    secondLowPrice = new List<decimal> { i, candlesPrices[i].Low };
            }

            return new List<List<decimal>> { firstHighPrice, secondHighPrice, firstLowPrice, secondLowPrice };
        }

        public static bool Touch(int x, decimal y, decimal x1, decimal x2, decimal y1, decimal y2)
        {
            var a = y1 - y2;
            var b = x2 - x1;
            var c = x1 * y2 - x2 * y1;
            decimal expectedPrice;

            expectedPrice = (x2 * y1 - x1 * y2 - a * x) / b;

            var priceDelta = Math.Abs(expectedPrice - y);
            if (priceDelta <= differentInPercent * expectedPrice)
                return true;
            return false;
        }

        public static List<Accumulation> FindBoxes(List<Candle> candles)
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

                    for (var k = 0; k < sectionLength; k++)
                    {
                        bottomWick.Add(candles[k].Low);
                        upperWick.Add(candles[k].High);
                    }

                    var candlesForLines = FindPointsForLine(section);
                    var firstHith = candlesForLines[0]; // Index and Price
                    var secondHith = candlesForLines[1];
                    var firstLow = candlesForLines[2];
                    var secondLow = candlesForLines[3];


                    var countTouchHigh = 0;
                    var countTouchLow = 0;

                    for (int indexCandle = 0; indexCandle < sectionLength; indexCandle++)
                    {
                        if (indexCandle != firstHith[0] && indexCandle != secondHith[0] &&
                            Touch(indexCandle, section[indexCandle].High, firstHith[0], secondHith[0], firstHith[1], secondHith[1]))
                            countTouchHigh++;
                        if (indexCandle != firstLow[0] && indexCandle != secondLow[0] &&
                            Touch(indexCandle, section[indexCandle].Low, firstLow[0], secondLow[0], firstLow[1], secondLow[1]))
                            countTouchLow++;
                    }

                    if (countTouchHigh > countTouchOfPrice && countTouchLow > countTouchOfPrice)
                        boxes.Add(new Accumulation(candles[i].TimeStamp, candles[i].Low,
                            candles[j].TimeStamp, candles[j].High, AccumulationType.Rectangle));
                }
            }
            return boxes;
        }
    }
}