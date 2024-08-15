namespace Lighthouse.AISListener.AIS.Subscription;

public class SubscriptionBuilder : ISubscriptionBuilder
{
  private readonly Subscription _subscription;

  public SubscriptionBuilder(string apiKey)
  {
    _subscription = new Subscription();
    _subscription.APIKey = apiKey;
    _subscription.BoundingBoxes = new List<double[][]>();
  }

  public ISubscriptionBuilder WithAPIKey(string apiKey)
  {
    _subscription.APIKey = apiKey;
    return this;
  }

  public ISubscriptionBuilder AddBoundingBox((double, double) corner1, (double, double) corner2)
  {
    _subscription.BoundingBoxes.Add(new[] {
        new[] { corner1.Item1, corner1.Item2 }, new[] { corner2.Item1, corner2.Item2 }
    });
    return this;
  }

  public ISubscriptionBuilder AddShipMMSIFilter(string mmsi)
  {
    _subscription.FiltersShipMMSI ??= new List<string>();
    if (!_subscription.FiltersShipMMSI.Contains(mmsi))
      _subscription.FiltersShipMMSI.Add(mmsi);
    return this;
  }
  public ISubscriptionBuilder AddShipMMSIFilter(IEnumerable<string> mmsis)
  {
    _subscription.FiltersShipMMSI ??= new List<string>();
    foreach (var mmsi in mmsis)
      if (!_subscription.FiltersShipMMSI.Contains(mmsi))
        _subscription.FiltersShipMMSI.Add(mmsi);
    return this;
  }
  
  public ISubscriptionBuilder AddMessageTypeFilter(MessageType messageType)
  {
    _subscription.FilterMessageTypes ??= new List<MessageType>();
    if (!_subscription.FilterMessageTypes.Contains(messageType))
      _subscription.FilterMessageTypes.Add(messageType);
    return this;
  }
  public ISubscriptionBuilder AddMessageTypeFilter(IEnumerable<MessageType> messageTypes)
  {
    _subscription.FilterMessageTypes ??= new List<MessageType>();
    foreach (var messageType in messageTypes)
      if (!_subscription.FilterMessageTypes.Contains(messageType))
        _subscription.FilterMessageTypes.Add(messageType);
    return this;
  }
  public ISubscriptionBuilder AddMessageTypeFilter(string messageTypeString)
  {
    _subscription.FilterMessageTypes ??= new List<MessageType>();
    if (!MessageTypeConverter.TryParse(messageTypeString, out var messageType))
      throw new ArgumentException("Invalid AIS Message Type");
    if (!_subscription.FilterMessageTypes.Contains(messageType))
      _subscription.FilterMessageTypes.Add(messageType);
    return this;
  }
  public ISubscriptionBuilder AddMessageTypeFilter(IEnumerable<string> messageTypesString)
  {
    _subscription.FilterMessageTypes ??= new List<MessageType>();
    foreach (var messageTypeString in messageTypesString)
    {
      if (!MessageTypeConverter.TryParse(messageTypeString, out var messageType))
        throw new ArgumentException("Invalid AIS Message Type");
      if (!_subscription.FilterMessageTypes.Contains(messageType))
        _subscription.FilterMessageTypes.Add(messageType);
    }
    return this;
  }

  public Subscription Build()
  {
    if (_subscription.BoundingBoxes.Count == 0)
      throw new InvalidAISSubscriptionException("At least 1 bounding box must be specified in the AIS Subscription");
    return _subscription;
  }
}