using DataStorage.DatabaseObjects;
using Microsoft.EntityFrameworkCore;

namespace DataStorage;

public class ApplicationContext : DbContext
{
    public DbSet<CandleDBO> Candles { get; set; }
    public DbSet<ZigZagDBO> ZigZag { get; set; }
    public DbSet<MovingAverageDBO> MovingAverage { get; set; }

    public ApplicationContext()
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CandleDBO>().HasKey(x =>
            new {x.Pair, x.ExchangeName, x.TimeFrame, x.TimeStamp});
        modelBuilder.Entity<ZigZagDBO>().HasKey(x =>
            new {x.Pair, x.ExchangeName, x.TimeFrame, x.TimeStamp});
        modelBuilder.Entity<MovingAverageDBO>().HasKey(x =>
            new {x.Pair, x.ExchangeName, x.TimeFrame, x.TimeStamp});
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=usersdb;Username=admin;Password=admin;Include Error Detail=True");
    }
}