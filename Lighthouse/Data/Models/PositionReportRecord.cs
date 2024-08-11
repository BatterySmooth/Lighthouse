namespace Lighthouse.Data.Models;

public class PositionReportRecord
{
  public Guid PositionReportID { get; set; }
  public DateTime ReceivedDate { get; set; }
  public int MMSI { get; set; }
  public int RateOfTurn { get; set; }
  public double Latitude { get; set; }
  public double Longitude { get; set; }
  public int TrueHeading { get; set; }
}