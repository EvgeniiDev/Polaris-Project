using Binance.Net;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BinanceApiDataParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new BinanceClient(new BinanceClientOptions() { });
            ;
            var callResult = await client.Spot.Market.GetKlinesAsync("ETHUSDT"
                , Binance.Net.Enums.KlineInterval.OneDay,
                new DateTime(2021, 5, 19),
                new DateTime(2021, 10, 1)
                );
            var candles = new List<CandleOHLC>();
            ;

            foreach (var t in callResult.Data)
            {
                var candle = new CandleOHLC(t.OpenTime.Ticks / 1000, t.Open, t.High, t.Low, t.Close);
                candles.Add(candle);
            }

            //так создаётся объект накопления первые 2 параметра это левая нижняя точка, оставшиеся 2 правая верхняя
            //var a = new Accumulation(candles[0].TimeStamp, candles[0].Low, candles[5].TimeStamp, candles[5].High);
            //string json = JsonSerializer.Serialize(candles);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var accum = new List<Accum>(){ new Accum("Base2", "triangle", "", new Settings(5, 6, 7, 8)),
                                            new Accum("Base2", "triangle", "", new Settings(5, 6, 7, 8)) };





            if (File.Exists("data.json")) File.Delete("data.json");
            var jsonStringCandlesData = JsonSerializer.Serialize(candles.Select(c =>
                                                new[] { c.TimeStamp, c.Open, c.High, c.Low, c.Close }).ToArray(), options);
            var jsonStringAccumulationData = JsonSerializer.Serialize(accum, options);
            ;
            string pattern = "{\"onchart\": " + jsonStringAccumulationData +",\"chart\": {\"data\": "+jsonStringCandlesData+"}}";

            File.WriteAllText("data.json", pattern);
            Console.WriteLine("Data has been saved to file");

            Console.ReadKey();
        }
    }
    [DataContract]
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


    public class Accum
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("data")]
        public string Data { get; set; }
        [JsonPropertyName("settings")]
        public Settings Settings { get; set; }

        public Accum(string name, string type, string data, Settings settings)
        {
            Name = name;
            Type = type;
            Data = data;
            Settings = settings;
        }
    }
    public class Settings
    {
        [JsonPropertyName("Wind")]
        public long T1 { get; set; }
        [JsonPropertyName("t1")]
        public decimal Price1 { get; set; }
        [JsonPropertyName("price1")]
        public long T2 { get; set; }
        [JsonPropertyName("t2")]
        public decimal Price2 { get; set; }
        [JsonPropertyName("price2")]
        public string Color { get; set; }
        [JsonPropertyName("zIndex")]
        public int ZIndex { get; set; }

        public Settings(int time1, int price1, int time2, int price2, string color= "#27d588", int zIndex=0)
        {
            T1 = time1;
            Price1 = price1;
            T2 = time2;
            Price2 = price2;
            Color = color;
            ZIndex = zIndex;
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
