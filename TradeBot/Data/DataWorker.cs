using Binance.Net.Enums;
using DataTypes;
using ExchangeConnectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeBot.Data;
using static ExchangeConnectors.TimeFrames;

namespace TradeBot
{
    class DataWorker
    {
        public string Pair;
        public TimeFrame TimeFrame = new();

        private List<Candle> Candles = new();
        private List<Accumulation> Accumulations = new();
        private List<Dot> zigZag = new();

        private DateTime Time = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private IExchange parser;
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

        private async Task CheckNewData()
        {
            while (IsStarted)
            {
                var lastCandles = await parser.GetCandles(Pair, TimeFrame, Time, Time);

                if (lastCandles.Count() != 0)
                {
                    if (Candles.Count > 0 && Candles.Last() != lastCandles.First())
                        await DataProcessing();
                    Candles.AddRange(lastCandles);
                    var timeOfLastData = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    timeOfLastData = timeOfLastData.AddMilliseconds(Candles.Last().TimeStamp);
                    Time = timeOfLastData.AddSeconds((int)GetSeconds(TimeFrame));
                    Console.WriteLine(Time);
                    if (Candles.Count % 100 == 0)
                        Export.WriteJson(Candles, Accumulations, zigZag, null, null, @"C:\Users\user\Desktop\tvjs-xp-main\src\apps", "data.json");
                }
            }
        }

        private async Task DataProcessing()
        {
            //var timer = new Stopwatch();
            // timer.Start();
            var a = Task.Run(() => ZigZag.CalculatePriceStructLight(Candles, 1));
            var b = Task.Run(() => SliceAlgorithm.FindBoxes(Candles));
            a.Wait();
            b.Wait();
            ;
            zigZag = a.Result;
            ;
            Accumulations = b.Result;
            //timer.Stop();
            //Console.WriteLine(timer.ElapsedMilliseconds);
            //Send to trader
            //JoinBoxes(accumulations);
        }

        public void Exit()
        {
            //stop working cycle
            //save data there to files
            //save data to db
            //
            IsStarted = false;
            Thread.Sleep(3000);
            ExportData();

        }

        private void ImportSavedData()
        {
            throw new NotImplementedException();
        }
        
        public void ImportData()
        {
            var candles = Export.GetCandlesFromDB($".\\data\\{Pair}", $"{TimeFrame}-candles.json");
            var accumulations = Export.GetAccumsFromDB($".\\data\\{Pair}", $"{TimeFrame}-accums.json");
            var zigZag = Export.GetZigZagFromDB($".\\data\\{Pair}", $"{TimeFrame}-zigzag.json");
        }
        
        private void ExportData()
        {
            //Export.WriteJson(Candles, Accumulations, zigZag, $".\\data\\{Pair}", $"{TimeFrame}-candles.json");
            Export.SaveCandles(Candles, $".\\data\\{Pair}", $"{TimeFrame}-candles.json");
            Export.SaveAccums(Accumulations, $".\\data\\{Pair}", $"{TimeFrame}-accums.json");
            Export.SaveZigZag(zigZag, $".\\data\\{Pair}", $"{TimeFrame}-zigzag.json");
            //save data to json
            //save data to db
        }

        private static int GetSeconds(TimeFrame timeFrame)
        {
            return (int)timeFrame;
        }
    }
}
