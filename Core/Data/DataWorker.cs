using Core.Events;
using Core.Events.Objects;
using DataTypes;
using ExchangeConnectors;
using static DataTypes.TimeFrames;

namespace Core.Data;

public class DataWorker
{
    public readonly string Pair;
    public readonly TimeFrame TimeFrame;
    private readonly IExchange _connector;
    public Type ExchangeType;

    private DateTime _time;
    public bool IsStarted { get; private set; }
    private readonly Mode mode;

    public DataWorker(IExchange connector, Type exchangeType, string pair, TimeFrame timeFrame,
        DateTime? startDate = null)
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

    public void Start()
    {
        if (mode == Mode.Socket)
            _connector.SubscribeOnNewKlines(Pair, TimeFrame, NewCandleEventHandler);
        else if (mode == Mode.Requests)
            Task.Run(()=>CheckNewData(500));

        IsStarted = true;
    }

    public void Stop()
    {
        if (mode == Mode.Socket)
            _connector.UnsubscribeOnNewKlines(Pair, TimeFrame, NewCandleEventHandler);

        IsStarted = false;
    }

    public void CheckNewData(int candlesAmount)
    {
        Console.WriteLine("Стартую");
        while (IsStarted)
        {
            var lastCandleTime =
                new DateTime(_time.Ticks, DateTimeKind.Utc).AddSeconds(candlesAmount * TimeFrame.GetSeconds()-1);
            var lastCandles = _connector.GetCandles(Pair, TimeFrame, _time, lastCandleTime).Result;

            if (lastCandles.Count == 0)
                _time = lastCandleTime;

            foreach (var candle in lastCandles)
            {
                _time = (candle.TimeStamp+TimeFrame.GetSeconds()*1000).ToDateTime();
                NewCandleEventHandler(new Kline(candle.TimeStamp, TimeFrame, candle.Open,
                    candle.High, candle.Low, candle.Close));
            }
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
        EventsCatalog.InvokeNewCandle(ev);
    }
    //todo сделать так чтоб этот метод работал
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