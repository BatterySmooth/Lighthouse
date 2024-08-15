using Newtonsoft.Json;

namespace Lighthouse.AISListener.AIS.Subscription;

public class Subscription
{
  public string APIKey;
  public List<double[][]> BoundingBoxes;
  public List<string>? FiltersShipMMSI;
  public List<MessageType>? FilterMessageTypes;

  public string Serialise()
  {
    return JsonConvert.SerializeObject(this,
      new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
  }
}