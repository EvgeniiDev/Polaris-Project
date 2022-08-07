using System.ComponentModel.DataAnnotations.Schema;

namespace DataStorage.DatabaseObjects;

[Table("Candles")]
public class CandleDBO : MainDatabaseObject
{
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
}