using System.Text.Json.Nodes;
using Lighthouse.AISListener.Data.Models;
using Lighthouse.AISListener.Logging;
using Newtonsoft.Json;

namespace Lighthouse.AISListener.Data;

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

  public static async Task Get()
  {
    var results = _dbContext.PositionReports;
    Console.WriteLine(JsonConvert.SerializeObject(results.First()));
  }
  
}