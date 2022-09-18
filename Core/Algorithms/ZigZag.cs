using DataTypes;

namespace Core.Algorithms;

public class ZigZag
{
    public static List<Dot> CalculatePriceStructLight(List<Candle> candles, float deviationInPercent)
    {
        var zigZag = new List<Dot>();

        var sell = new List<int>();
        var buy = new List<int>();

        if (candles.Count < 2)
            return zigZag;
        var trendUp = candles[1].High > candles[0].High;
        for (var n = 1; n < candles.Count; n++)
        {
            var previousCandle = candles[n - 1];
            var currentCandle = candles[n];
            if (currentCandle.High > previousCandle.High)
            {
                if (!trendUp)
                    zigZag.Add(new Dot(previousCandle.TimeStamp, previousCandle.Low));
                trendUp = true;
            }
            else if (currentCandle.Low < previousCandle.Low)
            {
                if (trendUp)
                    zigZag.Add(new Dot(previousCandle.TimeStamp, previousCandle.High));
                trendUp = false;
            }
        }

        if (zigZag.Count < 2)
            return zigZag;
        
        if (zigZag[^1].Price > candles[^1].High)
            zigZag.Add(new Dot(candles[^1].TimeStamp, candles[^1].Low));
        else
            zigZag.Add(new Dot(candles[^1].TimeStamp, candles[^1].High));

        return zigZag;
    }

    public static List<Dot> CalculateZigZag(List<Candle> candles, decimal deviationInPercent)
    {
        candles = candles.Where(x => x is not null).ToList();
        bool swingHigh = false, swingLow = false;
        var obsLow = 0;
        var obsHigh = 0;
        var obsStart = 0;
        var zigZag = new List<Dot>();

        for (int obs = obsStart; obs < candles.Count; obs++)
        {
            var candlesHighObs = (candles[obs].High + Math.Max(candles[obs].Open, candles[obs].Close)) / 2;
            
            var candlesHighObsHigh =
                (candles[obsHigh].High + Math.Max(candles[obsHigh].Open, candles[obsHigh].Close)) / 2;
            
            var candlesLowObsLow =
                (candles[obsLow].Low + Math.Min(candles[obsLow].Open, candles[obsLow].Close)) / 2;
            
            var candlesLowObs = (candles[obs].Low + Math.Min(candles[obs].Open, candles[obs].Close)) / 2;

            if (candlesHighObs > candlesHighObsHigh)
            {
                obsHigh = obs;
                if (!swingLow && (((candlesHighObsHigh - candlesLowObsLow) / candlesLowObsLow)) * 100m >=
                    deviationInPercent)
                {
                    zigZag.Add(new Dot(candles[obsLow].TimeStamp, candles[obsLow].Low));
                    swingHigh = false;
                    swingLow = true;
                }

                if (swingLow)
                    obsLow = obsHigh;
            }
            else if (candlesLowObs < candlesLowObsLow)
            {
                obsLow = obs;
                if (!swingHigh && (((candlesHighObsHigh - candlesLowObsLow) / candlesLowObsLow) * 100m >=
                                   deviationInPercent))
                {
                    zigZag.Add(new Dot(candles[obsHigh].TimeStamp, candles[obsHigh].High));
                    swingHigh = true;
                    swingLow = false;
                }

                if (swingHigh)
                    obsHigh = obsLow;
            }
        }

        if (swingHigh)
            zigZag.Add(new Dot(candles[obsHigh].TimeStamp, candles[obsHigh].High));
        else
            zigZag.Add(new Dot(candles[obsHigh].TimeStamp, candles[obsHigh].Low));

        return zigZag;
    }
}