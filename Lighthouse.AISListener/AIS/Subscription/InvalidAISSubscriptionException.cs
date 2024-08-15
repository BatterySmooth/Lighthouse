namespace Lighthouse.AISListener.AIS.Subscription;

[Serializable]
public class InvalidAISSubscriptionException : Exception
{
  public InvalidAISSubscriptionException ()
  {}

  public InvalidAISSubscriptionException (string message) 
    : base(message)
  {}

  public InvalidAISSubscriptionException (string message, Exception innerException)
    : base (message, innerException)
  {}    
}