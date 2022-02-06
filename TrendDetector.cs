using System;
using System.Collections.Generic;

namespace TradeBot
{
    class TrendDetector
    {
        public static List<Mark> TrendDetect(List<Dot> dots)
        {
            var result = new List<Mark>();

            if (dots.Count < 3)
                throw new Exception("Мало данных");

            var startTrend = dots[0].Price < dots[1].Price ? Trend.Up : Trend.Down;
            var currentTrend = startTrend;
            decimal previousLow = startTrend == Trend.Up ? dots[0].Price : dots[1].Price;
            decimal previousHigh = startTrend == Trend.Down ? dots[0].Price : dots[1].Price;

            for (int n = 4; n < dots.Count; n++)
            {

                if (dots[n].Price < dots[n - 2].Price
                        && dots[n - 2].Price > dots[n - 4].Price
                        && dots[n - 1].Price > dots[n - 2].Price)
                {
                    if (dots[n - 3].Price <= dots[n - 1].Price)
                    {
                        result.Add(new Mark(dots[n - 2].TimeStamp, "Down PP", 0, string.Empty, 0));
                    }
                    else
                    {
                        result.Add(new Mark(dots[n - 2].TimeStamp, "Down Slom", 0, string.Empty, 0));
                    }
                    currentTrend = Trend.Up;
                }

                if (dots[n].Price > dots[n - 2].Price
                        && dots[n - 2].Price < dots[n - 4].Price
                        && dots[n - 1].Price < dots[n - 2].Price)
                {
                    if (dots[n - 3].Price >= dots[n - 1].Price)
                    {
                        result.Add(new Mark(dots[n - 2].TimeStamp, "Up PP", 0, string.Empty, 0));
                    }
                    else
                    {
                        result.Add(new Mark(dots[n - 2].TimeStamp, "Up Slom", 0, string.Empty, 0));
                    }
                    currentTrend = Trend.Up;
                }



                //if (n % 2 == 0 && startTrend == Trend.Up || n % 2 == 1 && startTrend == Trend.Down)
                //    dotType = DotType.Low;
                //if (n % 2 == 0 && startTrend == Trend.Down || n % 2 == 1 && startTrend == Trend.Up)
                //    dotType = DotType.High;

                //if (dotType == DotType.Low && currentTrend == Trend.Up && previousLow < current.Price)
                //    previousLow = current.Price;

                //if (dotType == DotType.High && currentTrend == Trend.Down && previousHigh > current.Price)
                //    previousHigh = current.Price;

                //if (dotType == DotType.High && currentTrend == Trend.Down)
                //{
                //    if (previousHigh < current.Price)
                //    {
                //        result.Add(new Mark(dots[n - 1].TimeStamp, "Up PP", 0, string.Empty, 0));
                //        currentTrend = Trend.Up;
                //    }
                //}
                ////result.Add(new Mark(dots[n].TimeStamp, "Up", 0, string.Empty, 0));
                //if (dotType == DotType.Low && currentTrend == Trend.Up)
                //{
                //    if (previousLow > current.Price)
                //    {
                //        result.Add(new Mark(dots[n - 2].TimeStamp, "Down PP", 0, string.Empty, 0));
                //        currentTrend = Trend.Down;
                //    }
                //}
                //result.Add(new Mark(dots[n].TimeStamp, "Down", 0, string.Empty, 0));

                //if(dotType == DotType.Low)
                //    previousLow = current.Price;
                //if (dotType == DotType.High)
                //    previousHigh = current.Price;
            }
            return result;
        }
        public enum Trend
        {
            Down,
            Up,
            Side
        }
        public enum DotType
        {
            Low,
            High
        }
    }
}
