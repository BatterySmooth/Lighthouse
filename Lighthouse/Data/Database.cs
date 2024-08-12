using Lighthouse.Data.Models;
using Lighthouse.Logging;

namespace Lighthouse.Data;

public class Database
{
  private static LighthouseDbContext _dbContext;

  public static void Initialise()
  {
    Logger.LogSync("Connecting to Database");
    
    _dbContext = new LighthouseDbContext();
  }

  public static async Task Save(PositionReportRecord positionReportRecord)
  {
    positionReportRecord.ReceivedDate = positionReportRecord.ReceivedDate.ToUniversalTime();
    _dbContext.PositionReports.Add(positionReportRecord);
    await _dbContext.SaveChangesAsync();
  }

  public static List<PositionReportRecord>? GetPositionReportsBetweenDates(DateTime startDate, DateTime endDate)
  {
    try
    {
      DateTime utcStartDate = startDate.ToUniversalTime();
      DateTime utcEndDate = endDate.ToUniversalTime();
      
      var results = _dbContext.PositionReports
        .Where(r => r.ReceivedDate >= utcStartDate && r.ReceivedDate <= utcEndDate)
        .Select(r => new PositionReportRecord
        {
          PositionReportID = r.PositionReportID,
          ReceivedDate = r.ReceivedDate.ToUniversalTime(),
          MMSI = r.MMSI,
          Latitude = r.Latitude,
          Longitude = r.Longitude,
          TrueHeading = r.TrueHeading,
          RateOfTurn = r.RateOfTurn,
        })
        .ToList();
      
      return results;
    }
    catch (Exception e)
    {
      return null;
    }
  }
}