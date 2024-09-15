using System;
using System.Collections.Generic;

namespace Lighthouse.AISListener.AIS.Subscription;

public interface ISubscriptionBuilder
{
  ISubscriptionBuilder WithAPIKey(string apiKey);
  ISubscriptionBuilder AddBoundingBox((double, double) corner1, (double, double) corner2);
  ISubscriptionBuilder RemoveBoundingBox((double, double) corner1, (double, double) corner2);
  ISubscriptionBuilder AddShipMMSIFilter(string mmsi);
  ISubscriptionBuilder AddShipMMSIFilter(IEnumerable<string> mmsis);
  ISubscriptionBuilder RemoveShipMMSIFilter(string mmsi);
  ISubscriptionBuilder RemoveShipMMSIFilter(IEnumerable<string> mmsis);
  ISubscriptionBuilder AddMessageTypeFilter(MessageType messageType);
  ISubscriptionBuilder AddMessageTypeFilter(IEnumerable<MessageType> messageTypes);
  [Obsolete("Addition of string-based message filter is not reliable. It is recommended to use the MessageType Enum instead.", false)]
  ISubscriptionBuilder AddMessageTypeFilter(string messageTypeString);
  [Obsolete("Addition of string-based message filter is not reliable. It is recommended to use the MessageType Enum instead.", false)]
  ISubscriptionBuilder AddMessageTypeFilter(IEnumerable<string> messageTypesString);
  Subscription Build();
}