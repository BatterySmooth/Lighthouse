namespace Lighthouse.AISListener.AIS.OutgoingMessages;

public class ConnectionRequest
{
  public string APIKey { get; set; }
  public List<List<List<double>>> BoundingBoxes { get; set; } = new();
  public List<string>? FiltersShipMMSI { get; set; } = new();
  public List<string>? FilterMessageTypes { get; set; } = new();
  
  // object test =
  // {
  //   "APIKey": "a",
  //   "BoundingBoxes": [
  //     [ [-180, -90], [180, 90] ],
  //     [ [-180, -90], [180, 90] ],
  //   ],
  //   "FilterShipMMSI": ["1", "2", "3"],
  //   "FilterMessageTypes": ["1", "2", "3"]
  // }
}


