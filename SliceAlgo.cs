using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeBot
{
    public class SliceAlgorithm
    {
        const decimal differentInPercent = 0.7m;
        const int minAmountCandles = 5; // >3
        const int maxAmountCandles = 20; // >2
        const int minCountTouchOfPrice = 3;
        const int minAmountOfCombinations = 1;
        const decimal StableFactor = 0.10m;
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
                var lastBox = new Accumulation(0, 0, 0, 0, 0);
                for (int j = i + minAmountCandles; j < Math.Min(zigZag.Count, i + maxAmountCandles); j++)
                {
                    var section = zigZag.GetRange(i, j - i);
                    var y = isAccum(section);
                    if (y.Item1)
                    {


                        lastBox = (new Accumulation(section[0].TimeStamp, y.Item2,
                            section[section.Count - 1].TimeStamp, y.Item3, AccumulationType.Rectangle));

                    }
                }

                if (lastBox.EndTimeStamp != 0 
                    && !boxes.Select(x=>x.EndTimeStamp).Contains(lastBox.EndTimeStamp))
                    boxes.Add(lastBox);
            }
            return boxes;
        }
        private static (bool,decimal,decimal) isAccum(List<Candle> section)
        {
            const int resolution = 40;
            (Dot ATL, Dot ATH) = GetMaxsAndMins(section);
            decimal delta = (ATH.Price - ATL.Price) / (resolution - 1);
            var list = new int[resolution];

            foreach (var candle in section)
            {
                int startN = (int)((candle.Low - ATL.Price) / delta);
                int endN = (int)((candle.High - ATL.Price) / delta);
                for (int step = startN; step <= endN; step++)
                    list[step] += 1;
                startN = (int)((Math.Min(candle.Open, candle.Close) - ATL.Price) / delta);
                endN = (int)((Math.Max(candle.Open, candle.Close) - ATL.Price) / delta);
                for (int step = startN; step <= endN; step++)
                    list[step] += 1;
                ;
            }
            var median = midArifm(list.Select(x => (decimal)x).ToList());
            var box = new List<decimal>();
            for (var a = 0; a < list.Length; a++)
            {
                if (list[a] >= median)
                    box.Add(a * delta+ ATL.Price);
            }



            return (
                checkProjectionOfBox(0.65m,section)
                && IsStable(section) 
                && checkSquareOfBox(0.36m, section)
                , box.Min(),box.Max());
        }
        private static bool checkProjectionOfBox(decimal kFactor, List<Candle> section)
        {
            const int resolution = 40;
            (Dot ATL, Dot ATH) = GetMaxsAndMins(section);
            decimal delta = (ATH.Price - ATL.Price) / (resolution - 1);
            var list = new int[resolution];

            foreach (var candle in section)
            {
                int startN = (int)((candle.Low - ATL.Price) / delta);
                int endN = (int)((candle.High - ATL.Price) / delta);
                for (int step = startN; step <= endN; step++)
                    list[step] += 1;
                startN = (int)((Math.Min(candle.Open, candle.Close) - ATL.Price) / delta);
                endN = (int)((Math.Max(candle.Open, candle.Close) - ATL.Price) / delta);
                for (int step = startN; step <= endN; step++)
                    list[step] += 1;
                ;
            }
            var median = midArifm(list.Select(x => (decimal)x).ToList());
            var box = new List<decimal>();
            for (var a = 0; a < list.Length; a++)
            {
                if (list[a] >= median)
                    box.Add(a * delta + ATL.Price);
            }
            return resolution * kFactor >= box.Count();
        }
        private static bool checkSquareOfBox(decimal kFactor, List<Candle> section)
        {
            var sectionSquare = (section.Select(x => x.High).Max() - section.Select(x => x.Low).Min()) * (section.Count);
            var candlesSquare = 0m;
            foreach (var t in section)
            {
                //candlesSquare += (Math.Abs(t.Open - t.Close)*0.4m + Math.Abs(t.High - t.Low) *0.6m);
                candlesSquare += (Math.Abs(t.Open - t.Close) + Math.Abs(t.High - t.Low)) / 2;
            }
            return candlesSquare >= sectionSquare * kFactor;
        }
        public static decimal midArifm(List<decimal> sourceNumbers)
        {
            return sourceNumbers.Sum()/sourceNumbers.Count;
        }
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