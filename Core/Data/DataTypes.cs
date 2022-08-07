using ExchangeConnectors;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core
{
    public class Dot
    {
        public override string ToString()
        {
            return Price.ToString();
        }
        public Dot(long timeStamp, decimal price)
        {
            TimeStamp = timeStamp;
            Price = price;
        }

        public long TimeStamp { get; set; }
        public decimal Price { get; set; }
    }

    internal class Graph
    {
        public List<CandleDTO> Candles;

        public Graph(List<CandleDTO> candles)
        {
            Candles = candles;
        }
    }

    internal class CandleDTO
    {
        internal CandleDTO(long timeStamp, decimal open, decimal high, decimal low, decimal close)
        {
            TimeStamp = timeStamp;
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }
        [JsonPropertyName("timeStamp")]
        public long TimeStamp { get; set; }
        [JsonPropertyName("open")]
        public decimal Open { get; set; }
        [JsonPropertyName("high")]
        public decimal High { get; set; }
        [JsonPropertyName("low")]
        public decimal Low { get; set; }
        [JsonPropertyName("close")]
        public decimal Close { get; set; }
    }

    public class Accumulation
    {
        //public double VolumeLevel = 0;
        [JsonPropertyName("firstname")]
        public long StartTimeStamp { get; set; }
        public decimal LowPrice { get; set; }
        public long EndTimeStamp { get; set; }
        public decimal HighPrice { get; set; }
        public AccumulationType Type { get; set; }
    }

    internal class Marks
    {
        public string name { get; set; }
        public string type { get; set; }
        public object[] data { get; set; }
    }
    internal class Mark
    {
        [JsonPropertyName("timeStamp")]
        internal long TimeStamp { get; set; }
        [JsonPropertyName("text")]
        internal string Text { get; set; }
        [JsonPropertyName("num1")]
        internal decimal num1 { get; set; }
        [JsonPropertyName("color")]
        internal string Color { get; set; }
        [JsonPropertyName("num2")]
        internal decimal num2 { get; set; }

        internal Mark(long timeStamp, string text, decimal v2, string color, decimal v3)
        {
            TimeStamp = timeStamp;
            Text = text;
            num1 = v2;
            Color = color;
            num2 = v3;
        }
        public override string ToString()
        {
            return $"{Text} : {TimeStamp.ToDateTime()}";
        }
    }
    //{
    //    "name": "Line1",
    //    "type": "Segment",
    //    "data": [],
    //    "settings": {
    //        "p1": [1555732800000, 5405],
    //        "p2": [1555948800000, 5306],
    //        "lineWidth": 1,
    //        "legend": false
    //    }
    //}
    internal class Segment
    {
        public Dot FirstDot = new Dot(0, 0);
        public Dot SecondDot = new Dot(0, 0);

        public Segment(long firstTimeStamp, decimal firstPrice, long secondTimeStamp, decimal secondPrice)
        {
            FirstDot.TimeStamp = firstTimeStamp;
            FirstDot.Price = firstPrice;
            SecondDot.TimeStamp = secondTimeStamp;
            SecondDot.Price = secondPrice;
        }

        public Segment(Dot firstDot, Dot secondDot)
        {
            FirstDot = firstDot;
            SecondDot = secondDot;
        }
    }

    public enum AccumulationType
    {
        Rectangle,
        Triangle,
        Сonstriction,
        CompressionUp,
        CompressionDown
    }


    public enum LineType
    {
        Horizontal,
        Wane,
        Increase,
    }
}
