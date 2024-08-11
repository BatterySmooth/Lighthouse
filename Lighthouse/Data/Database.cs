using Lighthouse.Data.Models;
using Lighthouse.Logging;

namespace Lighthouse.Data;

public class Database
{
  private static LighthouseDbContext _dbContext;
  
  public static void Initialise()
  {
    Logger.LogSync("Connecting to Database...");
    
    _dbContext = new LighthouseDbContext();
  }

  public static async Task Save(PositionReportRecord positionReportRecord)
  {
    _dbContext.PositionReports.Add(positionReportRecord);
    await _dbContext.SaveChangesAsync();
  }
}