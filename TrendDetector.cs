using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBot
{
    class TrendDetector
    {
        public static List<Dot> TrandDetect(List<Dot> dots)
        {
            var answer = new List<Dot>();
            var startTrend = dots[0].Price < dots[1].Price ? Trend.Up : Trend.Down;
            var currentTrend = startTrend;
            decimal lastLocalLow = startTrend == Trend.Up ? dots[0].Price : dots[1].Price;
            decimal lastLocalHigh = startTrend == Trend.Down ? dots[0].Price : dots[1].Price;

            for (int n = startTrend == Trend.Up ? 3 : 2; n < dots.Count; n += 2)
            {
                ;
                var low = dots[n - 1];
                var high = dots[n];
                if (lastLocalLow <= low.Price && lastLocalHigh <= high.Price ||
                    (lastLocalLow <= low.Price && !(lastLocalHigh <= high.Price) && low.Price<=dots[n+2].Price))//up trend
                {
                    if (currentTrend == Trend.Down)
                        answer.Add(dots[n]);
                    currentTrend = Trend.Up;
                    Console.WriteLine($"Up Trend {low.Price}");
                }
                else if (lastLocalHigh >= high.Price && lastLocalLow >= low.Price ||
                    !(lastLocalHigh <= high.Price) && lastLocalLow <= low.Price && high.Price <= dots[n + 1].Price)//down trend
                {
                    if (currentTrend == Trend.Up)
                        answer.Add(dots[n-1]);
                    currentTrend = Trend.Down;
                    Console.WriteLine($"Down Trend {low.Price}");
                }
                else
                {
                    Console.WriteLine($"Trend Broken");

                    if (currentTrend == Trend.Up)
                    {
                        answer.Add(dots[n-1]);
                        currentTrend = Trend.Down;
                    }
                    else
                    {
                        answer.Add(dots[n-2]);
                        currentTrend = Trend.Up;
                    }
                }
                lastLocalLow = low.Price;
                lastLocalHigh = high.Price;
            }

            return answer;
        }
        public static List<Dot> CalculatePriceStructLight(List<Candle> candles, decimal deviationInPercent)
        {
            var trend = candles[1].High > candles[0].High || candles[1].Low > candles[0].Low ? Trend.Up : Trend.Down;
            var zigZag = new List<Dot>();
            AddDot(candles[0], zigZag, trend);
            for (int n = 1; n < candles.Count; n++)
            {
                var previousCandle = candles[n - 1];
                var currentCandle = candles[n];
                if (currentCandle.High > previousCandle.High)
                {
                    if (trend == Trend.Down) AddDot(previousCandle, zigZag, trend);
                    trend = Trend.Up;
                }
                else if (currentCandle.Low < previousCandle.Low)
                {
                    if (trend == Trend.Up) AddDot(previousCandle, zigZag, trend);
                    trend = Trend.Down;
                }
            }
            AddDot(candles[candles.Count - 1], zigZag, trend);
            return zigZag;
        }

        private static void AddDot(Candle candle, List<Dot> dots, Trend trend)
        {
            if (trend == Trend.Down)
                dots.Add(new Dot(candle.TimeStamp, candle.Low));
            else
                dots.Add(new Dot(candle.TimeStamp, candle.High));
        }

        public enum Trend
        {
            Down,
            Up,
            Side
        }
    }
}
