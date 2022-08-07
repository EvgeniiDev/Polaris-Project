using System.ComponentModel.DataAnnotations.Schema;

namespace DataStorage.DatabaseObjects;

[Table("MovingAverage")]
public class MovingAverageDBO : MainDatabaseObject
{
    public decimal Price { get; set; }
}