using Lighthouse.Listener.Models.IncomingMessages;
using Microsoft.EntityFrameworkCore;

namespace Lighthouse.Listener.data;

public class LighthouseDbContext : DbContext
{
  public DbSet<DbPositionReport> PositionReports { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseNpgsql(Program.Configuration["SQLConnectionString"]);
  }
  
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<DbPositionReport>().ToTable("TEST_PositionReports");
    modelBuilder.Entity<DbPositionReport>().HasKey(t => t.PositionReportID);
  }
  
}