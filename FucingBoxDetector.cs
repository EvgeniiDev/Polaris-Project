using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeBot
{
    class FucingBoxDetector
    {
        public static List<Dot> TrandDetector(List<Dot> dots)
        {
            var answer = new List<Dot>();
            var startTrand = Trand.Down;//dots[0].Price < dots[1].Price ? Trand.Up : Trand.Down;
            var currentTrand = startTrand;
            var distribution = new List<Dot>();
            decimal lastLocalLow = 0;
            decimal lastLocalHigh = 0;

            for (int n = 1; n < dots.Count; n+=2)
            {
                ;
                var low = startTrand == Trand.Down ? dots[n] : dots[n - 1];
                var high = startTrand == Trand.Up ? dots[n]: dots[n - 1];
                if (lastLocalLow<= low.Price && lastLocalHigh <= high.Price)//up trend
                {
                   // answer.Add(dots[n]);
                    currentTrand = Trand.Up;
                    Console.WriteLine("Up Trend");
                }
                else if(lastLocalLow >= low.Price && lastLocalHigh >= high.Price)//down trend
                {
                  //  answer.Add(dots[n]);
                    currentTrand = Trand.Down;
                    Console.WriteLine("Down Trend");
                }
                else if(currentTrand == Trand.Up && (lastLocalLow >= low.Price && lastLocalHigh >= high.Price))
                {
                    Console.WriteLine("UpTrend Broken");
                    currentTrand = Trand.Down;
                }
                else if (currentTrand == Trand.Down && (lastLocalLow <= low.Price && lastLocalHigh <= high.Price))
                {
                    Console.WriteLine("DownTrend Broken");
                }
                else
                {
                   // Console.WriteLine("Govno");
                }
                lastLocalLow = low.Price;
                lastLocalHigh = high.Price;
            }
            return answer;
        }
        public static List<Dot> CalculatePriceStructLight(List<Candle> candles, decimal deviationInPercent)
        {
            var trend = candles[1].High > candles[0].High || candles[1].Low > candles[0].Low? Trand.Up: Trand.Down;
            var zigZag = new List<Dot>();
            AddDot(candles[0], zigZag, trend);
            for (int n = 1; n < candles.Count; n++)
            {
                var previousCandle = candles[n - 1];
                var currentCandle = candles[n];
                if (currentCandle.High > previousCandle.High)
                {
                    if (trend == Trand.Down)   AddDot(previousCandle, zigZag, trend);
                    trend = Trand.Up;
                }
                else if (currentCandle.Low < previousCandle.Low)
                {
                    if (trend == Trand.Up)    AddDot(previousCandle, zigZag, trend);
                    trend = Trand.Down;
                }
            }
            AddDot(candles[candles.Count-1], zigZag, trend);
            return zigZag;
        }

        private static void AddDot(Candle candle, List<Dot> dots, Trand trend)
        {
            if (trend == Trand.Down)
                dots.Add(new Dot(candle.TimeStamp, candle.Low));
            else
                dots.Add(new Dot(candle.TimeStamp, candle.High));
        }

        public enum Trand
        {
            Down,
            Up,
            Side
        }
    }
}
