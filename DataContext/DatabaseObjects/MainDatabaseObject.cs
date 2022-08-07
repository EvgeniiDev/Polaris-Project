using System.ComponentModel.DataAnnotations;
using static DataTypes.TimeFrames;

namespace DataStorage.DatabaseObjects;

public class MainDatabaseObject
{
    public string Pair { get; set; }
    public string ExchangeName { get; set; }
    public TimeFrame TimeFrame { get; set; }
    public long TimeStamp { get; set; }
}