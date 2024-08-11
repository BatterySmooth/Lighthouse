namespace Lighthouse.AISListener.IncomingMessages;

public class MetaData
{
  public int MMSI { get; set; }
  public int MMSI_String { get; set; }
  public string ShipName { get; set; }
  public double latitude { get; set; }
  public double longitude { get; set; }
  public string time_utc { get; set; }
  
  public MetaData(int mmsi, int mmsiString, string shipName, double latitude, double longitude, string timeUtc)
  {
    MMSI = mmsi;
    MMSI_String = mmsiString;
    ShipName = shipName;
    this.latitude = latitude;
    this.longitude = longitude;
    time_utc = timeUtc;
  }
}