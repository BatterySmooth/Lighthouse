using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lighthouse.Tower.Data.Models;
using Lighthouse.Tower.Logging;
using Newtonsoft.Json;

namespace Lighthouse.Tower.Data
{
  public class Database
  {
    private static LighthouseDbContext _dbContext;

    public Database()
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

    public static async Task Get()
    {
      var results = _dbContext.PositionReports;
      Console.WriteLine(JsonConvert.SerializeObject(results.First()));
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
        Logger.LogAsync($"Error fetching Position Reports: {e.Message}");
        return null;
      }
    }
  }
}