namespace Lighthouse.Beacon.Data.Models;

public readonly struct DateRange(DateTime startDate, DateTime endDate)
{
  public readonly DateTime StartDate { get; } = startDate;
  public readonly DateTime EndDate { get; } = endDate;
}