using Lighthouse.API.Data.Models;

namespace Lighthouse.API.Data;

public class Database
{
  private static LighthouseDbContext _dbContext;

  public static void Initialise()
  {
    Console.WriteLine("Connecting to Database");
    
    _dbContext = new LighthouseDbContext();
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