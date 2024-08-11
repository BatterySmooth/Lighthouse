using Lighthouse.Configuration;
using Lighthouse.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Lighthouse.Data;

public class LighthouseDbContext : DbContext
{
  public DbSet<PositionReportRecord> PositionReports { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseNpgsql(Config.SQLConnectionString);
  }
  
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<PositionReportRecord>().ToTable("TEST_PositionReports");
    modelBuilder.Entity<PositionReportRecord>().HasKey(t => t.PositionReportID);
  }
  
}