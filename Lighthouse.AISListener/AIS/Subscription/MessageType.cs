using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lighthouse.AISListener.AIS.Subscription;

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageType
{
  PositionReport,
  UnknownMessage,
  AddressedSafetyMessage,
  AddressedBinaryMessage,
  AidsToNavigationReport,
  AssignedModeCommand,
  BaseStationReport,
  BinaryAcknowledge,
  BinaryBroadcastMessage,
  ChannelManagement,
  CoordinatedUTCInquiry,
  DataLinkManagementMessage,
  DataLinkManagementMessageData,
  ExtendedClassBPositionReport,
  GroupAssignmentCommand,
  GnssBroadcastBinaryMessage,
  Interrogation,
  LongRangeAisBroadcastMessage,
  MultiSlotBinaryMessage,
  SafetyBroadcastMessage,
  ShipStaticData,
  SingleSlotBinaryMessage,
  StandardClassBPositionReport,
  StandardSearchAndRescueAircraftReport,
  StaticDataReport
}

public static partial class MessageTypeConverter
{
  public static bool TryParse(string value, out MessageType result)
  {
    return Enum.TryParse(value, out result);
  }
}