﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceApiDataPArser.Data
{
    class DataExport
    {
        public static void WriteJson(List<CandleOHLC> candles, List<Accumulation> accumulation, string filePath)
        {
            var accum = new List<Accum>();
            foreach (var t in accumulation)
                accum.Add(new Accum("Base", t.Type, "",
                            new Settings(t.StartTimeStamp, t.LowPrice, t.EndTimeStamp, t.HighPrice)));

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var chart = new Dictionary<string, List<decimal[]>>() { { "data", candles
                                        .Select(c => new[] { c.TimeStamp, c.Open, c.High, c.Low, c.Close }).ToList() } };
            var outJsonStruct = new Dictionary<string, object>() { { "onchart", accum }, { "chart", chart } };
            var result = JsonSerializer.Serialize(outJsonStruct, options);

            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllText(filePath, result);
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
}
