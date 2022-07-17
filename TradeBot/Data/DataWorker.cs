using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DataTypes;
using ExchangeConnectors;
using TradeBot.Algorithms;
using static DataTypes.TimeFrames;

namespace TradeBot.Data;

public class DataWorker
{
    public readonly string Pair;
    public readonly TimeFrame TimeFrame;
    private readonly IExchange _connector;
    private DataDistributor _distributor;
    public Type ExchangeType;

    private DateTime _time;
    public float Rps;
    public bool IsStarted { get; private set; } = false;
    private readonly Mode mode;

    public DataWorker(IExchange connector, Type exchangeType, string pair, TimeFrame timeFrame, DateTime? startDate = null)
    {
        Pair = pair;
        TimeFrame = timeFrame;

        if (startDate != null)
        {
            _time = startDate.Value;
            mode = Mode.Requests;
        }
        else
        {
            _time = DateTime.UtcNow;
            mode = Mode.Socket;
        }

        _connector = connector;
        ExchangeType = exchangeType;
    }

    internal void RegisterDistributor(DataDistributor distributor)
    {
        _distributor = distributor;
    }

    public void Start()
    {
        if (mode == Mode.Socket)
            _connector.SubscibeOnNewKlines(Pair, TimeFrame, NewCandleEventHandler);
        else if (mode == Mode.Requests)
            Task.Run(() => CheckNewData(2));
        IsStarted = true;
    }

    public void Stop()
    {
        if (mode == Mode.Socket)
            _connector.UnsubscibeOnNewKlines(Pair, TimeFrame, NewCandleEventHandler);
        IsStarted = false;
    }

    public void CheckNewData(int candlesAmount)
    {
        var stopWatch = new Stopwatch();
        while (IsStarted)
        {
            stopWatch.Restart();
            var lastCandleTime = new DateTime(_time.Ticks, DateTimeKind.Utc).AddSeconds((candlesAmount - 1) * TimeFrame.GetSeconds());
            var lastCandles = _connector.GetCandles(Pair, TimeFrame, _time, lastCandleTime).Result;
            foreach (var candle in lastCandles)
            {
                _time = _time.AddSeconds(TimeFrame.GetSeconds());
                NewCandleEventHandler(new Kline(candle.TimeStamp, TimeFrame, candle.Open,
                    candle.High, candle.Low, candle.Close));
            }
            var sleep = (int)(1000 / Rps - stopWatch.ElapsedMilliseconds);
            Thread.Sleep(sleep > 0 ? sleep : 0);
        }
    }

    private void NewCandleEventHandler(Kline candle)
    {
        //Console.WriteLine($"{candle.TimeStamp} {candle.TimeFrame}");
        DataProcessing(candle);
    }

    private void DataProcessing(Kline candle)
    {
        var ev = new NewCandleEvent
        {
            ExchangeType = ExchangeType,
            Candle = candle,
            Pair = Pair,
            TimeFrame = candle.TimeFrame,
            TimeStamp = candle.TimeStamp,
        };
        _distributor.DataReceivedEventHandler(ev);

        // if (_candles.Count % 100 == 0)
        //     Export.WriteJson(_candles, null, null, null, null, $".\\data\\{Pair}",
        //         $"{TimeFrame}-candles.json");
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


    public void ImportData()
    {
        var candles = Export.GetCandlesFromDB($".\\data\\{Pair}", $"{TimeFrame}-candles.json");
        var accumulations = Export.GetAccumsFromDB($".\\data\\{Pair}", $"{TimeFrame}-accums.json");
        var zigZag = Export.GetZigZagFromDB($".\\data\\{Pair}", $"{TimeFrame}-zigzag.json");
    }

    private void ExportData()
    {
        //Export.WriteJson(Candles, Accumulations, zigZag, $".\\data\\{Pair}", $"{TimeFrame}-candles.json");
        // Export.SaveCandles(_candles, $".\\data\\{Pair}", $"{TimeFrame}-candles.json");
        //Export.SaveAccums(_accumulations, $".\\data\\{Pair}", $"{TimeFrame}-accums.json");
        //Export.SaveZigZag(_zigZag, $".\\data\\{Pair}", $"{TimeFrame}-zigzag.json");
        //save data to json
        //save data to db
    }


    public enum Mode
    {
        Socket,
        Requests,
    }
}