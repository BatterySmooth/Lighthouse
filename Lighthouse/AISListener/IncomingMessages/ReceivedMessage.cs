namespace Lighthouse.AISListener.IncomingMessages;

public class ReceivedMessage
{
  public Message Message { get; set; }
  public string MessageType { get; set; }
  public MetaData MetaData { get; set; }
  
  public ReceivedMessage(Message message, string messageType, MetaData metaData)
  {
    Message = message;
    MessageType = messageType;
    MetaData = metaData;
  }
}