using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradeBot.Data
{
    class Export
    {
        public static void WriteJson(List<Candle> candles, List<Accumulation> accumulation, List<Dot> zag, List<Mark> marks, string dir, string fileName)
        {
            var accum = new List<Accum>();
            foreach (var t in accumulation)
            {
                accum.Add(new Accum("Base", t.Type, new string[0],
                            new Settings(t.StartTimeStamp, t.LowPrice, t.EndTimeStamp, t.HighPrice)));
            }

            var chart = new Dictionary<string, List<decimal[]>>() { { "data", candles
                                        .Select(c => new[] { c.TimeStamp, c.Open, c.High, c.Low, c.Close }).ToList() } };
            var zigzag = new List<Segment>();
            for (int n = 1; n < zag.Count; n++)
            {
                zigzag.Add(new Segment("Line", "", new string[0],
                            new SegmentSettings(new decimal[] { zag[n - 1].TimeStamp, zag[n - 1].Price },
                                                new decimal[] { zag[n].TimeStamp, zag[n].Price }, 2, false)));
            }
            var output = new List<object>(accum);
            output.AddRange(zigzag);
            var mmarks =  marks.Select(c => new object[] { c.TimeStamp, c.Text, c.num1, c.Color, c.num2 } ).ToList()  ;
            var asdas = new Marks() { type = "Splitters", name = "Data", data = mmarks.ToArray() };
            output.Add(asdas);
            var outJsonStruct = new Dictionary<string, object>() { { "onchart", output }, { "chart", chart } };
            var options = new JsonSerializerOptions { WriteIndented = true, };
            var result = JsonSerializer.Serialize(outJsonStruct, options);

            Directory.CreateDirectory(dir);
            if (File.Exists($"{dir}\\{fileName}"))
            {
                File.Delete($"{dir}\\{fileName}");
            }
            File.WriteAllText($"{dir}\\{fileName}", result);
        }

        public static void SaveCandles(List<Candle> candles, string dir, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var asdas = new Candles();
            asdas.candles = candles;
            var result = JsonSerializer.Serialize(asdas, options);
            Directory.CreateDirectory(dir);
            if (File.Exists($"{dir}\\{fileName}"))
                File.Delete($"{dir}\\{fileName}");
            File.WriteAllText($"{dir}\\{fileName}", result);
        }
        public static void SaveAccums(List<Accumulation> accumulations, string dir, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var asdas = new Accums() { accums = accumulations };
            var result = JsonSerializer.Serialize(asdas, options);

            Directory.CreateDirectory(dir);
            if (File.Exists($"{dir}\\{fileName}"))
                File.Delete($"{dir}\\{fileName}");
            File.WriteAllText($"{dir}\\{fileName}", result);
        }
        public static void SaveZigZag(List<Dot> zigZag, string dir, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var asdas = new Dots() { zigZag = zigZag };
            var result = JsonSerializer.Serialize(asdas, options);

            Directory.CreateDirectory(dir);
            if (File.Exists($"{dir}\\{fileName}"))
                File.Delete($"{dir}\\{fileName}");
            File.WriteAllText($"{dir}\\{fileName}", result);
        }

        public static List<Accumulation> GetAccumsFromDB(string dir, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            if (File.Exists($"{dir}\\{fileName}"))
            {
                string json = File.ReadAllText($"{dir}\\{fileName}");
                return JsonSerializer.Deserialize<Accums>(json, options).accums;
            }
            else
            {
                throw new Exception("No savefile");
            }
        }
        public static List<Candle> GetCandlesFromDB(string dir, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            if (File.Exists($"{dir}\\{fileName}"))
            {
                string json = File.ReadAllText($"{dir}\\{fileName}");
                return JsonSerializer.Deserialize<Candles>(json, options).candles;
            }
            else
            {
                throw new Exception("No savefile");
            }
        }
        public static List<Dot> GetZigZagFromDB(string dir, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            if (File.Exists($"{dir}\\{fileName}"))
            {
                string json = File.ReadAllText($"{dir}\\{fileName}");
                return JsonSerializer.Deserialize<Dots>(json, options).zigZag;
            }
            else
            {
                throw new Exception("No savefile");
            }
        }

        public static object GetDataFromDB(DataFromDB dataType)
        {
            MySqlConnection conn = MySqlConnector.GetDBConnection();
            conn.Open();
            try
            {
                return GetPairs(conn);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return null;
        }
        private static List<Pair> GetPairs(MySqlConnection conn)
        {
            string sql = "Select id, nameOfPair from pairs";
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            var result = new List<Pair>();
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int idNameOfPair = reader.GetOrdinal("nameOfPair");
                        string nameOfPair = reader.GetString(idNameOfPair);
                        result.Add(new Pair(nameOfPair));
                    }
                }
            }
            return result;
        }

    }
    public class Pair
    {
        string Name;

        public Pair(string nameOfPair)
        {
            Name = nameOfPair;
        }
    }
    public class BalancesHistory
    {
        List<BalanceHistory> BalanceHistory;
    }
    public class SegmentSettings
    {
        [JsonPropertyName("p1")]
        public decimal[] P1 { get; set; }
        [JsonPropertyName("p2")]
        public decimal[] P2 { get; set; }
        [JsonPropertyName("lineWidth")]
        public int LineWidth { get; set; }
        [JsonPropertyName("legend")]
        public bool Legend { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }

        public SegmentSettings(decimal[] p1, decimal[] p2, int lineWidth, bool legend, string color = "#FFC0CB")
        {
            P1 = p1;
            P2 = p2;
            LineWidth = lineWidth;
            Legend = legend;
            Color = color;
        }
    }
    public class Segment
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("data")]
        public string[] Data { get; set; }
        [JsonPropertyName("settings")]
        public SegmentSettings Settings { get; set; }

        public Segment(string name, string type, string[] data, SegmentSettings settings)
        {
            Name = name;
            Type = "Segment";
            Data = data;
            Settings = settings;
        }
    }
    public class BalanceHistory
    {
        decimal price;
    }
    public enum DataFromDB
    {
        GetPairs,
        GetBalancesHistory,
        GetCandles,
        GetPriceGraph
    }
    public enum DataInDB
    {
        AddPair,
        AddBalancesHistory,
        AddCandles,
        AddPriceGraph
    }
    public class Accum
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("data")]
        public string[] Data { get; set; }
        [JsonPropertyName("settings")]
        public Settings Settings { get; set; }

        public Accum(string name, AccumulationType type, string[] data, Settings settings)
        {
            Name = name;
            Type = "Square";
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
        [JsonPropertyName("z-index")]
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
    public class Candles
    {
        public List<Candle> candles { get; set; }
    }
    public class Accums
    {
        public List<Accumulation> accums { get; set; }
    }
    public class Dots
    {
        public List<Dot> zigZag { get; set; }
    }
}
