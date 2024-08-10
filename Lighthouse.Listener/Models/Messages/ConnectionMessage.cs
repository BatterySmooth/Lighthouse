namespace Lighthouse.Listener.Models.Messages;

public class ConnectionMessage
{
  public string APIKey { get; set; } = "";
  public List<List<List<double>>> BoundingBoxes { get; set; } = new();
  public List<string>? FiltersShipMMSI { get; set; } = new();
  public List<string>? FilterMessageTypes { get; set; } = new();
}