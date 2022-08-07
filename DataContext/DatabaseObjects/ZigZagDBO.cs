using System.ComponentModel.DataAnnotations.Schema;

namespace DataStorage.DatabaseObjects;
[Table("ZigZag")]
public class ZigZagDBO : MainDatabaseObject
{
    public decimal Price { get; set; }
}
