using Binance.Net;
using Binance.Net.Objects;
using BinanceApiDataPArser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BinanceApiDataPArser
{
    class DataParser
    {
        public async Task<List<CandleOHLC>> GetCandles(string symbol, Binance.Net.Enums.KlineInterval interval, DateTime start, DateTime end)
        {

            var client = new BinanceClient(new BinanceClientOptions() { });
            var callResult = await client.Spot.Market.GetKlinesAsync(symbol, interval, start, end);
            var candles = new List<CandleOHLC>();

            foreach (var t in callResult.Data)
            {
                var candle = new CandleOHLC(t.OpenTime.Ticks / 1000, t.Open, t.High, t.Low, t.Close);
                candles.Add(candle);
            }
            return candles;
        }
        public void WriteJson(List<CandleOHLC> candles,List<Accumulation> accumulation, string file) { 
            var accum = new List<Accum>();
            foreach(var t in accumulation)
                accum.Add(new Accum("Base", t.Type, "",
                            new Settings(t.StartTimeStamp, t.LowPrice, t.EndTimeStamp,t.HighPrice)));

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            if (File.Exists(file)) File.Delete(file);
                var jsonStringCandlesData = JsonSerializer.Serialize(candles.Select(c =>
                                                    new[] { c.TimeStamp, c.Open, c.High, c.Low, c.Close }).ToArray(), options);
                var jsonStringAccumulationData = JsonSerializer.Serialize(accum, options);
                string pattern = "{\"onchart\": " + jsonStringAccumulationData + 
                                    ",\"chart\": {\"data\": " + jsonStringCandlesData + "}}";

                File.WriteAllText(file, pattern);
            }
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

    public Accum(string name, AccumulationType type, string data, Settings settings)
    {
        Name = name;
        Type = type.ToString();
        Data = data;
        Settings = settings;
    }
}
public class Settings
{
    [JsonPropertyName("t1")]
    public long T1 { get; set; }
    [JsonPropertyName("price1")]
    public decimal Price1 { get; set; }
    [JsonPropertyName("t2")]
    public long T2 { get; set; }
    [JsonPropertyName("price2")]
    public decimal Price2 { get; set; }
    [JsonPropertyName("color")]
    public string Color { get; set; }
    [JsonPropertyName("zIndex")]
    public int ZIndex { get; set; }

    public Settings(long time1, decimal price1, long time2, decimal price2, string color = "#27d588", int zIndex = 0)
    {
        T1 = time1;
        Price1 = price1;
        T2 = time2;
        Price2 = price2;
        Color = color;
        ZIndex = zIndex;
    }
}
