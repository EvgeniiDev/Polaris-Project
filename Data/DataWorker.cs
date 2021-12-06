using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net.Enums;
using TradeBot.Data;

namespace TradeBot
{
    class DataWorker : IDisposable
    {
        public string Pair;
        public KlineInterval TimeFrame = new KlineInterval();
        private List<Candle> Candles = new List<Candle>();
        private List<Accumulation> Accumulations = new List<Accumulation>();
        private List<Dot> zigZag = new List<Dot>();
        private DateTime Time = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private bool IsStarted = false;
        public void Run()
        {
            //Timer t = new Timer(1000);
            //t.AutoReset = true;
            //t.Elapsed += new ElapsedEventHandler(CheckNewData);
            //t.Start();
            IsStarted = true;
            CheckNewData();
        }

        private async void CheckNewData()
        {
            while (IsStarted)
            {
                var lastCandles = await new DataParser().GetCandles(Pair, TimeFrame, Time, Time);
                lastCandles = lastCandles.Where(x => x != null).ToList();
                if (lastCandles.Count == 0)
                    continue;
                if (lastCandles.Count > 0 && Candles.Count > 0 && Candles.Last() != lastCandles.First())
                {
                    await DataProcessing();
                }

                Candles.AddRange(lastCandles);
                var timeOfLastData = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                                   .AddMilliseconds(Candles.Last().TimeStamp);
                Time = timeOfLastData.AddSeconds((int)GetSeconds(TimeFrame));
                Console.WriteLine(Time);
                if (Candles.Count % 20 == 0)
                    Export.WriteJson(Candles, Accumulations, zigZag, @"C:\Users\user\Desktop\tvjs-xp-main\src\apps\data.json");
            }
        }

        private async Task<int> DataProcessing()
        {
            var timer = new Stopwatch();
            timer.Start();
            await UpdateZigZags();
            await UpdateAccumulations();
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);
            //Send to trader
            //JoinBoxes(accumulations);
            return 1;
        }

        private async Task UpdateZigZags()
        {
            //int countOfReferenceDots = 2;
            //if (candles.Count > 20)
            //{
            //    var test = ZigZag.CalculateZigZag(candles, 5);
            //    if (zigZag.Count >= countOfReferenceDots)
            //    {
            //        var time = zigZag[zigZag.Count - countOfReferenceDots].TimeStamp;
            //        var firstCandle = candles.Find(x => x.TimeStamp == time);
            //        var firstCandleId = candles.IndexOf(firstCandle);
            //        var newZigZag = ZigZag.CalculateZigZag(candles.TakeLast(candles.Count()-firstCandleId).ToList(), 5);
            //       // zigZag.RemoveRange()
            //        zigZag.AddRange(newZigZag.Skip(countOfReferenceDots));
            //    }
            //    else if (test.Count >= zigZag.Count)
            //    {
            //        zigZag.Clear();
            //        zigZag.AddRange(test);
            //    }
            //}
            zigZag = await Task.Run(() => ZigZag.CalculateZigZag(Candles, 5));
        }

        private async Task UpdateAccumulations()
        {
            Accumulations = await Task.Run(() => SliceAlgorithm.FindBoxes(Candles));
        }

        private void Exit()
        {
            //stop workin cycle
            //save data there to files
            //save data to db
            //
            throw new NotImplementedException();
        }

        private void ImportSavedData()
        {

        }
        private void ExportData()
        {
            Export.WriteJson(Candles, Accumulations, zigZag, $"./data/{Pair}-{TimeFrame}.json");
            //save data to json
            //save data to db
            throw new NotImplementedException();
        }
        private static void JoinBoxes(List<Accumulation> accumulations)
        {
            ;
            foreach (var a in accumulations.ToList())
            {
                foreach (var t in accumulations.ToList())
                {
                    if (a.StartTimeStamp < t.StartTimeStamp && t.EndTimeStamp < a.EndTimeStamp)
                    {
                        accumulations.Remove(t);
                    }
                }
            }

        }
    
        private static TimeFrame GetSeconds(KlineInterval timeFrame)
        {
            switch (timeFrame)
            {
                case KlineInterval.OneMinute:
                    return TradeBot.TimeFrame.m1;
                case KlineInterval.FiveMinutes:
                    return TradeBot.TimeFrame.m5;
                case KlineInterval.FifteenMinutes:
                    return TradeBot.TimeFrame.m15;
                case KlineInterval.ThirtyMinutes:
                    return TradeBot.TimeFrame.m30;
                case KlineInterval.OneHour:
                    return TradeBot.TimeFrame.h1;
                case KlineInterval.FourHour:
                    return TradeBot.TimeFrame.h4;
                case KlineInterval.TwelveHour:
                    return TradeBot.TimeFrame.h12;
                case KlineInterval.OneDay:
                    return TradeBot.TimeFrame.D1;
                case KlineInterval.OneWeek:
                    return TradeBot.TimeFrame.W1;
                case KlineInterval.OneMonth:
                    return TradeBot.TimeFrame.M1;
                default:
                    throw new Exception("Unknown timeframe!");
            }
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
