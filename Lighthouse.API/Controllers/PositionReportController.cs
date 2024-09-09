using Lighthouse.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace Lighthouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionReportController : ControllerBase
{
  private readonly ILogger<PositionReportController> _logger;
  
  public PositionReportController(ILogger<PositionReportController> logger)
  {
    _logger = logger;
  }
  
  [HttpGet(Name = "GetPositionReports")]
  public IActionResult Get([FromQuery] DateRangeModel dateRange)
  {
    if (!ModelState.IsValid)
    {
      var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
      Console.WriteLine($"GET FAILED | GetPositionReports {dateRange.StartDate} - {dateRange.EndDate}: {string.Join(",", errors)}");
      return BadRequest($"Request model validation failed: {errors}");
    }

    if (dateRange.StartDate > dateRange.EndDate)
    {
      Console.WriteLine($"GET FAILED | GetPositionReports {dateRange.StartDate} - {dateRange.EndDate}: Start date must be before end date");
      return BadRequest("Start date must be before end date");
    }
    
    var dbRecords = Database.GetPositionReportsBetweenDates(dateRange.StartDate, dateRange.EndDate);
    
    Console.WriteLine($"GET OK | GetPositionReports {dateRange.StartDate} - {dateRange.EndDate}: {dbRecords.Count} records returned");
    return Ok(dbRecords);
  }
  
}