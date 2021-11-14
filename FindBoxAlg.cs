using System;
using System.Collections.Generic;

namespace TradeBot
{
    public class BoxDetectionAlgoritm
    {
        const decimal differentInPercent = 0.04m;
        const int minCountCandles = 4; // >3
        const int maxCountCandles = 50; // >2
        const int minCountTouchOfPrice = 2;

        public static (decimal, decimal) calcFactors(List<Dot> twoDots)
        {
            if (twoDots[1].TimeStamp - twoDots[0].TimeStamp == 0)
                return (0,twoDots[0].Price);
            return ((twoDots[0].Price - twoDots[1].Price) / (twoDots[1].TimeStamp - twoDots[0].TimeStamp),
                (twoDots[0].TimeStamp * twoDots[1].Price - twoDots[1].TimeStamp * twoDots[0].Price) / (twoDots[1].TimeStamp - twoDots[0].TimeStamp));
        }
        public static int CountTouches(List<Dot> maxs, List<Dot> dots, Direction dir)
        {
            (decimal factorA, decimal factorB) = calcFactors(maxs);
            bool trendUp = dots[1].Price > dots[0].Price;
            int touchCount = 0;
            decimal expectedPrice;
            for (int i = 0; i < dots.Count; i++)
            {
                if (i % 2 == (trendUp == (dir == Direction.Up) ? 1 : 0))
                {
                    expectedPrice = -dots[i].TimeStamp * factorA - factorB;
                    var priceDelta = Math.Abs(expectedPrice - dots[i].Price);
                    if (priceDelta <= differentInPercent * expectedPrice)
                        touchCount++;
                }
            }
            return touchCount;
        }

        //TODO interface
        public static List<Accumulation> FindBoxes(List<Dot> zigZag)
        {
            var boxes = new List<Accumulation>();
            for (int i = 0; i < zigZag.Count; i++)
            {
                var lastBox = new Accumulation(0,0,0,0,0);
                for (int j = i + minCountCandles; j < Math.Min(zigZag.Count, i + maxCountCandles); j++)
                {
                    var section = zigZag.GetRange(i, j - i);
                    if (isAccum(section))
                       lastBox = (new Accumulation(section[0].TimeStamp, section[0].Price,
                           section[section.Count - 1].TimeStamp, section[section.Count - 1].Price, AccumulationType.Rectangle));
                }
                if (lastBox.EndTimeStamp!=0)
                    boxes.Add(lastBox);
            }
            return boxes;
        }

        private static bool isAccum(List<Dot> section)
        {
            (var lowDots, var highDots) = GetTwoMaxsAndMins(section);
            var upTouches = CountTouches(highDots, section, Direction.Up);
            var downTouches = CountTouches(lowDots, section, Direction.Down);
            return minCountTouchOfPrice <= downTouches && minCountTouchOfPrice <= upTouches;
        }

        public static (List<Dot>, List<Dot>) GetTwoMaxsAndMins(List<Dot> dots)
        {
            var highDots = new List<Dot>() { { new Dot(0, 0) }, { new Dot(0, 0) } }; // firstMax, secondMax
            var lowDots = new List<Dot>() { { new Dot(0, decimal.MaxValue) }, { new Dot(0, decimal.MaxValue) } };
            bool trendUp = dots[1].Price > dots[0].Price;
            for (int i = 0; i < dots.Count; i++)
            {
                if (i % 2 == (trendUp == true ? 1 : 0))
                {
                    //high dots
                    if (dots[i].Price >= highDots[0].Price)
                    {
                        highDots[1] = highDots[0];
                        highDots[0] = dots[i];
                    }
                    else if (dots[i].Price > highDots[1].Price)
                        highDots[1] = dots[i];
                }
                else
                {
                    //low dots
                    if (dots[i].Price <= lowDots[0].Price)
                    {
                        lowDots[1] = lowDots[0];
                        lowDots[0] = dots[i];
                    }
                    else if (dots[i].Price < lowDots[1].Price)
                        lowDots[1] = dots[i];
                }
            }
            if (lowDots[1].TimeStamp == 0) lowDots[1] = lowDots[0];
            if (highDots[1].TimeStamp == 0) highDots[1] = highDots[0];

            return (lowDots,highDots);
        }
        public enum Direction
        {
            Up,
            Down
        }
    }
}