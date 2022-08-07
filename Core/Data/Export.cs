using DataTypes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Data;

class Export
{
    public static void WriteJson(List<Candle> candles, List<Accumulation> accumulation, List<Dot> zag
                    , List<Mark> marks, List<Segment> segments, string dir, string fileName)
    {
        var chart = new Dictionary<string, List<decimal[]>>() { { "data", candles
                                    .Select(c => new[] { c.TimeStamp, c.Open, c.High, c.Low, c.Close }).ToList() } };

        var output = new List<object>();
        
        if (accumulation != null)
        {
            var accum = new List<Accum>();
            foreach (var t in accumulation)
            {
                accum.Add(new Accum("Base", t.Type, new string[0],
                            new AccumSettings(t.StartTimeStamp, t.LowPrice, t.EndTimeStamp, t.HighPrice)));
            }
            output.Add(accum);
        }

        if (zag != null)
        {
            var zigzag = new List<JsonSegment>();
            for (int n = 1; n < zag.Count; n++)
            {
                zigzag.Add(new JsonSegment("Line", "", new string[0],
                            new JsonSegmentSettings(new decimal[] { zag[n - 1].TimeStamp, zag[n - 1].Price },
                                                new decimal[] { zag[n].TimeStamp, zag[n].Price }, 2, false)));
            }
            output.AddRange(zigzag);
        }
   

        //var mmarks =  marks.Select(c => new object[] { c.TimeStamp, c.Text, c.num1, c.Color, c.num2 } ).ToList()  ;
        //var asdas = new Marks() { type = "Splitters", name = "Data", data = mmarks.ToArray() };
        //output.Add(asdas);

        if (segments != null)
        {
            var lines = new List<JsonSegment>();
            for (int n = 0; n < segments.Count; n++)
            {
                lines.Add(new JsonSegment("Line", "", new string[0],
                            new JsonSegmentSettings(new decimal[] { segments[n].FirstDot.TimeStamp, segments[n].FirstDot.Price },
                                                new decimal[] { segments[n].SecondDot.TimeStamp, segments[n].SecondDot.Price }, 2, false, "#FFFF00")));
            }
            output.AddRange(lines);
        }

        var outJsonStruct = new Dictionary<string, object>() { { "onchart", output }, { "chart", chart } };
        var options = new JsonSerializerOptions { WriteIndented = true, };
        var result = JsonSerializer.Serialize(outJsonStruct, options);

        Directory.CreateDirectory(dir);
        var path = $"{dir}\\{fileName}";
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, result);
    }

    public static void SaveCandles(List<Candle> candles, string dir, string fileName)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var asdas = new Candles();
        asdas.candles = candles;
        var result = JsonSerializer.Serialize(asdas, options);
        Directory.CreateDirectory(dir);
        var path = $"{dir}\\{fileName}";
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, result);
    }
    public static void SaveAccums(List<Accumulation> accumulations, string dir, string fileName)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var asdas = new Accums() { accums = accumulations };
        var result = JsonSerializer.Serialize(asdas, options);

        Directory.CreateDirectory(dir);
        var path = $"{dir}\\{fileName}";
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, result);
    }
    public static void SaveZigZag(List<Dot> zigZag, string dir, string fileName)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var asdas = new Dots() { zigZag = zigZag };
        var result = JsonSerializer.Serialize(asdas, options);

        Directory.CreateDirectory(dir);
        var path = $"{dir}\\{fileName}";
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, result);
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

    //public static object GetDataFromDB(DataFromDB dataType)
    //{
    //    MySqlConnection conn = MySqlConnector.GetDBConnection();
    //    conn.Open();
    //    try
    //    {
    //        return GetPairs(conn);
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine("Error: " + e);
    //        Console.WriteLine(e.StackTrace);
    //    }
    //    finally
    //    {
    //        conn.Close();
    //        conn.Dispose();
    //    }
    //    return null;
    //}
    //private static List<Pair> GetPairs(MySqlConnection conn)
    //{
    //    string sql = "Select id, nameOfPair from pairs";
    //    MySqlCommand cmd = new MySqlCommand();
    //    cmd.Connection = conn;
    //    cmd.CommandText = sql;
    //    var result = new List<Pair>();
    //    using (DbDataReader reader = cmd.ExecuteReader())
    //    {
    //        if (reader.HasRows)
    //        {
    //            while (reader.Read())
    //            {
    //                int idNameOfPair = reader.GetOrdinal("nameOfPair");
    //                string nameOfPair = reader.GetString(idNameOfPair);
    //                result.Add(new Pair(nameOfPair));
    //            }
    //        }
    //    }
    //    return result;
    //}

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
public class MarkSettings
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

    public MarkSettings(decimal[] p1, decimal[] p2, int lineWidth, bool legend, string color = "#FFC0CB")
    {
        P1 = p1;
        P2 = p2;
        LineWidth = lineWidth;
        Legend = legend;
        Color = color;
    }
}
public class Mark
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("data")]
    public string[] Data { get; set; }
    [JsonPropertyName("settings")]
    public MarkSettings Settings { get; set; }

    public Mark(string name, string type, string[] data, MarkSettings settings)
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
    public AccumSettings Settings { get; set; }

    public Accum(string name, AccumulationType type, string[] data, AccumSettings settings)
    {
        Name = name;
        Type = "Square";
        Data = data;
        Settings = settings;
    }
}
public class AccumSettings
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

    public AccumSettings(long time1, decimal price1, long time2, decimal price2, string color = "#27d588", int zIndex = 0)
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

//"name": "Line1",
//"type": "Segment",
//"data": [],
//"settings": {
//    "p1": [1555732800000, 5405],
//    "p2": [1555948800000, 5306],
//    "lineWidth": 1,
//    "legend": false
//}
public class JsonSegment
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("data")]
    public string[] Data { get; set; }
    [JsonPropertyName("settings")]
    public JsonSegmentSettings Settings { get; set; }

    public JsonSegment(string name, string type, string[] data, JsonSegmentSettings settings)
    {
        Name = name;
        Type = "Segment";
        Data = data;
        Settings = settings;
    }
}

public class JsonSegmentSettings
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

    public JsonSegmentSettings(decimal[] p1, decimal[] p2, int lineWidth, bool legend, string color = "#FFC0CB")
    {
        P1 = p1;
        P2 = p2;
        LineWidth = lineWidth;
        Legend = legend;
        Color = color;
    }
}


