using DataTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TradeBot.Data;
using static DataTypes.TimeFrames;

namespace TradeBot.Algorithms;

public class DataDistributor
{
    ConcurrentDictionary<(Type, string, TimeFrame), List<Candle>> storage=new();
    public void DataReceivedEventHandler(NewCandleEvent candleEvent)
    {
        //Console.WriteLine(candleEvent.TimeStamp);
        var key = (candleEvent.ExchangeType, candleEvent.Pair, candleEvent.TimeFrame);

        if (storage.ContainsKey(key))
        {
            var candles = storage[key];
            if (candleEvent.TimeStamp <= candles.Last().TimeStamp)
            {
                Console.WriteLine("перемешалось");
            }
            storage[key].Add(candleEvent.Candle);
        }
        else
            storage.TryAdd(key, new List<Candle> { candleEvent.Candle });


        var _candles = storage[key];
       // var timer = new Stopwatch();
       // timer.Start();
        var a = Task.Run(() => ZigZag.CalculatePriceStructLight(_candles, 1));
        var b = Task.Run(() => SliceAlgorithm.FindBoxes(_candles));
        a.Wait();
        b.Wait();
        //
        // timer.Stop();
        // Console.WriteLine(timer.ElapsedMilliseconds);
        //Send to trader
        //JoinBoxes(accumulations);
    }
}