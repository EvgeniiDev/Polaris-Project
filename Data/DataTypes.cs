using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TradeBot
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

    public class Graph
    {
        public List<Candle> Candles;

        public Graph(List<Candle> candles)
        {
            Candles = candles;
        }
    }

    public class Candle
    {
        public Candle(long timeStamp, decimal open, decimal high, decimal low, decimal close)
        {
            this.TimeStamp = timeStamp;
            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
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

    public class Marks
    {
        public string name { get; set; }
        public string type { get; set; }
        public object[] data { get; set; }
    }
    public class Mark
    {
        [JsonPropertyName("timeStamp")]
        public long TimeStamp { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("num1")]
        public decimal num1 { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("num2")]
        public decimal num2 { get; set; }

        public Mark(long timeStamp, string text, decimal v2, string color, decimal v3)
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
    public class Segment
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
