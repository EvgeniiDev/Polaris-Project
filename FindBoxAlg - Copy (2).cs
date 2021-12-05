﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeBot
{
    public class BoxDetectionAlgoritm2
    {
        const decimal differentInPercent = 0.05m;
        const int minAmountCandles = 4; // >3
        const int maxAmountCandles = 15; // >2
        const int minCountTouchOfPrice = 3;
        const int minAmountOfCombinations = 1;
        const decimal StableFactor = 0.3m;
        public static (decimal, decimal) calcFactors(List<Dot> twoDots)
        {
            if (twoDots[1].TimeStamp - twoDots[0].TimeStamp == 0)
                return (0,twoDots[0].Price);
            return ((twoDots[0].Price - twoDots[1].Price) / (twoDots[1].TimeStamp - twoDots[0].TimeStamp),
                (twoDots[0].TimeStamp * twoDots[1].Price - twoDots[1].TimeStamp * twoDots[0].Price) / (twoDots[1].TimeStamp - twoDots[0].TimeStamp));
        }
        public static List<Dot> GetTouches(Dot border, List<Candle> dots, Direction dir)
        {
            ;
            decimal factorB = border.Price;
            var touches = new List<Dot>() { new Dot(0,0)};
            for (int i = 0; i < dots.Count; i++)
            {
                if (dir == Direction.Up)
                {
                    var expectedPrice = factorB;
                    var priceDelta = Math.Abs(expectedPrice - dots[i].High);
                    if (priceDelta <= differentInPercent * expectedPrice
                        && (-touches.Last().TimeStamp + dots[i].TimeStamp) >= 86400000*2)
                    {
                        touches.Add(new Dot(dots[i].TimeStamp, dots[i].High));
                    }
                }
                else
                {

                    var expectedPrice = factorB;
                    var priceDelta = Math.Abs(expectedPrice - dots[i].Low);
                    if (priceDelta <= differentInPercent * expectedPrice
                        && (-touches.Last().TimeStamp + dots[i].TimeStamp) >= 86400000*2)
                    {
                        touches.Add(new Dot(dots[i].TimeStamp, dots[i].Low));
                    }
                    ;
                }
                
            }
            touches.RemoveAt(0);
            return touches;
        }
        //TODO interface
        public static List<Accumulation> FindBoxes(List<Candle> zigZag)
        {
            var boxes = new List<Accumulation>();
            for (int i = 0; i < zigZag.Count; i++)
            {
                var lastBox = new Accumulation(0,0,0,0,0);
                for (int j = i + minAmountCandles; j < Math.Min(zigZag.Count, i + maxAmountCandles+1); j++)
                {
                    var section = zigZag.GetRange(i, j - i+1);

                    //var section = ZigZag.CalculatePriceStructLight(candles.Result, 1);
                    ;
                    if (isAccum(section))
                    {
                        var a = section.OrderBy(x => x.High).ToList();
                        var b = section.OrderBy(x => x.Low).ToList();
                        lastBox = (new Accumulation(section[0].TimeStamp, b[0].Low,
                                    section[a.Count - 1].TimeStamp, a[a.Count - 1].High, AccumulationType.Rectangle));
                        ExtendBox(zigZag, section);
                        //boxes.Add(new Accumulation(section[0].TimeStamp, b[0].Low,
                        //            section[a.Count - 1].TimeStamp, a[a.Count - 1].High, AccumulationType.Rectangle));
  
                    }
                }
                if (lastBox.EndTimeStamp != 0)
                {
                    boxes.Add(lastBox);
                    i += minAmountCandles;
                }
            }
            return boxes;
        }
        private static bool isAccum(List<Candle> section)
        {
            var sectionSquare = (section.Select(x => x.High).Max() - section.Select(x => x.Low).Min()) * (section.Count);
            var candlesSquare = 0m;
            foreach (var t in section)
                candlesSquare += (Math.Abs(t.Open - t.Close) + Math.Abs(t.High - t.Low)) / 2;

            return candlesSquare >= sectionSquare * 0.4m;
        }
        //private static bool isAccum(List<Candle> section)
        //{
        //    (var lowDots, var highDots) = GetMaxsAndMins(section);
        //    var upTouches = GetTouches(highDots, section, Direction.Up);
        //    var downTouches = GetTouches(lowDots, section, Direction.Down);
        //    var combinations = 0;
        //    if (upTouches.Count > downTouches.Count)
        //    {
        //        for(int n =1; n < upTouches.Count; n++)
        //        {
        //            foreach (var d in downTouches)
        //                if (upTouches[n - 1].TimeStamp < d.TimeStamp && d.TimeStamp < upTouches[n].TimeStamp)
        //                    combinations++;
        //        }
        //    }
        //    else
        //    {
        //        for (int n = 1; n < downTouches.Count; n++)
        //        {
        //            foreach (var d in upTouches)
        //                if (downTouches[n - 1].TimeStamp < d.TimeStamp && d.TimeStamp < downTouches[n].TimeStamp)
        //                    combinations++;
        //        }
        //    }
        //    return minCountTouchOfPrice <= downTouches.Count
        //            && minCountTouchOfPrice <= upTouches.Count
        //            && minAmountOfCombinations <= combinations
        //            && IsStable(section);
        //}

        private static bool IsStable(List<Candle> section)
        {
            var medianDelta = new List<decimal>();
            decimal maxMedian = 0;
            foreach(var t in section)
            {
                medianDelta.Add(t.High - t.Close);
                maxMedian = Math.Max(maxMedian, t.High - t.Close);
            }
            var median = GetMedian(medianDelta);
            return maxMedian*StableFactor< median;
        }

        private static void ExtendBox(List<Candle> zigZag, List<Candle> section)
        {
            var a = section.OrderBy(x => x.High).ToList();
            var topBorder = a[a.Count() - 1].High; // firstMax, secondMax
            var b = section.OrderBy(x => x.Low).ToList();
            var lowBorder = b[0].Low;

            var firstIndex = section.IndexOf(section.First());
            var lastIndex = section.IndexOf(section.Last());

            for (int i = lastIndex-1; i < zigZag.Count; i++)
            {
                if (lowBorder <= zigZag[i].Low*0.98m && zigZag[i].High * 1.02m <= topBorder)
                    section.Add(zigZag[i]);
                else
                {
                    break;
                }
            }
            for (int i = firstIndex-1; i >= 0; i--)
            {
                if (lowBorder <= zigZag[i].Low * 0.98m && zigZag[i].High * 1.02m <= topBorder)
                    section.Add(zigZag[i]);
                else
                {
                    break;
                }
            }
        }
        public static decimal GetMedian(List<decimal> sourceNumbers)
        {    
            if (sourceNumbers == null || sourceNumbers.Count == 0)
                throw new System.Exception("Median of empty array not defined.");

            var sortedPNumbers = new List<decimal>(sourceNumbers);
            sortedPNumbers.Sort();
            int size = sortedPNumbers.Count;
            int mid = size / 2;
            decimal median = (size % 2 != 0) ? (decimal)sortedPNumbers[mid] : 
                                ((decimal)sortedPNumbers[mid] + (decimal)sortedPNumbers[mid - 1]) / 2;
            return median;
        }
        public static (Dot, Dot) GetMaxsAndMins(List<Candle> dots)
        {
            ;
            var a = dots.OrderBy(x => x.High).ToList();
            var highDots = new Dot(a[a.Count - 1].TimeStamp, a[a.Count-1].High); // firstMax, secondMax
            var b = dots.OrderBy(x => x.Low).ToList();
            var lowDots = new Dot(b[0].TimeStamp, b[0].Low);
            return (lowDots,highDots);
        }

        public enum Direction
        {
            Up,
            Down
        }
    }
}