using Lighthouse.Tower.Configuration;
using Lighthouse.Tower.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Lighthouse.Tower.Data
{
  public class LighthouseDbContext : DbContext
  {
    public DbSet<PositionReportRecord> PositionReports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseNpgsql(Config.DBConnectionString);
    }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<PositionReportRecord>().ToTable(Config.DBPositionReportTable);
      modelBuilder.Entity<PositionReportRecord>().HasKey(t => t.PositionReportID);
    }
  
  }
}