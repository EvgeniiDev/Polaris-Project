using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BinanceApiDataParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new BinanceClient(new BinanceClientOptions(){});
            var callResult = await client.Spot.Market.GetKlinesAsync("ETHUSDT"
                , Binance.Net.Enums.KlineInterval.OneDay,
                new DateTime(2021, 5, 19),
                new DateTime(2021, 10, 1)
                );
            var candles = new List<CandleOHLC>();
            ;
            foreach(var candle in callResult.Data)
            {
                var cndl = new CandleOHLC(candle.OpenTime.Ticks,candle.Open, candle.High, candle.Low, candle.Close );
                candles.Add(cndl);
            }


            //так создаётся объект накопления первые 2 параметра это левая нижняя точка, оставшиеся 2 правая верхняя
            var a = new Accumulation(candles[0].TimeStamp, candles[0].Low, candles[5].TimeStamp, candles[5].High);







            ////string json = JsonSerializer.Serialize(candles);
            //var options = new JsonSerializerOptions
            //{
            //    WriteIndented = true,
            //    IncludeFields = true,
            //};

            //if (File.Exists("data.json"))   File.Delete("data.json");

            //var jsonString = JsonSerializer.Serialize(candles.ToArray(),options);
            //File.WriteAllText("data.json", jsonString);
            //Console.WriteLine("Data has been saved to file");

            Console.ReadKey();
        }
    }

    public class Accumulation
    {
        double VolumeLevel = 0;
        long TimeStamp1;
        decimal Price1;
        long TimeStamp2;
        decimal Price2;

        public Accumulation(long timeStamp1, decimal low, long timeStamp2, decimal high)
        {
            TimeStamp1 = timeStamp1;
            Price1 = low;
            TimeStamp2 = timeStamp2;
            Price2 = high;
        }
    }
    public class CandleOHLC
    {
        public CandleOHLC(long ticks, decimal open, decimal high, decimal low, decimal close)
        {
            TimeStamp = ticks;
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }
        public long TimeStamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }


}
