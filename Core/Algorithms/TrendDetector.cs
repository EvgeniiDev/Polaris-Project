
using Core.Events;

namespace Core
{
    public class TrendDetector
    {

        public static List<Segment> TrendDetect(List<Dot> dots)
        {
            if (dots.Count <= 2)
                return new List<Segment>();// throw new Exception("Мало данных");

            var result = new List<Segment>();
            int lastPPNum = -1;
            int lastSlomNum = -1;

            var startTrend = dots[0].Price < dots[2].Price ? Trend.Up : Trend.Down;
            var currentTrend = startTrend;

            for (int n = 3; n < dots.Count; n++)
            {
                var t = 0.05m;
                if (currentTrend == Trend.Up && dots[n - 1].Price > dots[n - 0].Price && dots[n - 1].Price > dots[n - 2].Price)
                {
                    if (dots[n - 1].Price > dots[n - 3].Price)
                        lastPPNum = n - 2;
                    //Console.WriteLine(dots[lastPPNum]);

                    if (dots[n - 0].Price > dots[n - 2].Price && dots[n - 1].Price > dots[n - 3].Price)
                        lastSlomNum = n;
                    //Console.WriteLine(dots[lastSlomNum]);

                    else if (dots[n - 2].Price > dots[n - 0].Price)
                    {
                        //Console.WriteLine($"ПП в short {dots[n - 2]}");
                        if (lastPPNum != -1)
                        {
                            result.Add(new Segment(dots[lastPPNum].TimeStamp, dots[lastPPNum].Price,
                                                   dots[lastPPNum].TimeStamp + 500000000, dots[lastPPNum].Price));

                            EventsCatalog.InvokePP(new PP()
                            {
                                Dot = new Dot(dots[lastPPNum].TimeStamp, dots[lastPPNum].Price),
                                Trend = Trend.Down
                            });
                            //PPDownDetected();
                        }

                        if (lastSlomNum != -1)
                        {
                            result.Add(new Segment(dots[lastSlomNum].TimeStamp, dots[lastSlomNum].Price,
                                                   dots[lastSlomNum].TimeStamp + 500000000, dots[lastSlomNum].Price));

                            EventsCatalog.InvokeSlom(new PP()
                            {
                                Dot = new Dot(dots[lastSlomNum].TimeStamp, dots[lastSlomNum].Price),
                                Trend = Trend.Down
                            });
                            //SlomDownDetected();
                            //Console.WriteLine($"Слом в short {lastSlom}");
                        }

                        currentTrend = Trend.Down;
                    }
                }

                if (currentTrend == Trend.Down && dots[n - 1].Price < dots[n - 0].Price && dots[n - 1].Price < dots[n - 2].Price)
                {
                    if (dots[n - 1].Price < dots[n - 3].Price)
                        lastPPNum = n - 2;
                    //Console.WriteLine(dots[lastPPNum]);

                    if (dots[n - 0].Price < dots[n - 2].Price && dots[n - 1].Price < dots[n - 3].Price)
                        lastSlomNum = n;

                    else if (dots[n - 2].Price < dots[n - 0].Price)
                    {
                        //Console.WriteLine($"ПП в short {dots[n - 2]}");
                        if (lastPPNum != -1)
                        {
                            result.Add(new Segment(dots[lastPPNum].TimeStamp, dots[lastPPNum].Price,
                                                   dots[lastPPNum].TimeStamp + 500000000, dots[lastPPNum].Price));

                            EventsCatalog.InvokePP(new PP()
                            {
                                Dot = new Dot(dots[lastPPNum].TimeStamp, dots[lastPPNum].Price),
                                Trend = Trend.Up
                            });
                            //PPUpDetected();
                        }
                        if (lastSlomNum != -1)
                        {
                            result.Add(new Segment(dots[lastSlomNum].TimeStamp, dots[lastSlomNum].Price,
                                                   dots[lastSlomNum].TimeStamp + 500000000, dots[lastSlomNum].Price));

                            EventsCatalog.InvokeSlom(new PP()
                            {
                                Dot = new Dot(dots[lastSlomNum].TimeStamp, dots[lastSlomNum].Price),
                                Trend = Trend.Up
                            });
                            //SlomUpDetected();
                        }
                        currentTrend = Trend.Up;
                    }
                }
            }
            //for (int n = 4; n < dots.Count; n++)
            //{
            //    if (dots[n - 0].Price < dots[n - 2].Price
            //            && dots[n - 2].Price > dots[n - 4].Price
            //            && dots[n - 1].Price > dots[n - 2].Price)
            //    {
            //        if (dots[n - 3].Price <= dots[n - 1].Price)
            //        {
            //            Console.Write(dots[n - 2].Price);
            //            Console.WriteLine(new Mark(dots[n - 2].TimeStamp, "Down PP", 0, string.Empty, 0));
            //        }
            //        else
            //        {
            //            Console.Write(dots[n - 2].Price);
            //            Console.WriteLine(new Mark(dots[n - 2].TimeStamp, "Down Slom", 0, string.Empty, 0));
            //        }
            //        currentTrend = Trend.Down;
            //    }
            //    if (dots[n - 0].Price > dots[n - 2].Price
            //            && dots[n - 2].Price < dots[n - 4].Price
            //            && dots[n - 1].Price < dots[n - 2].Price)
            //    {
            //        if (dots[n - 3].Price >= dots[n - 1].Price)
            //        {
            //            Console.Write(dots[n - 2].Price);
            //            Console.WriteLine(new Mark(dots[n - 2].TimeStamp, "Up PP", 0, string.Empty, 0));
            //        }
            //        else
            //        {
            //            Console.Write(dots[n - 2].Price);
            //            Console.WriteLine(new Mark(dots[n - 2].TimeStamp, "Up Slom", 0, string.Empty, 0));
            //        }
            //        currentTrend = Trend.Up;
            //    }
            //}
            return result;
        }

    }
    public class PP
    {
        public Trend Trend;
        public Dot Dot;
    }
    public enum Trend
    {
        Down,
        Up,
        Side
    }
}
