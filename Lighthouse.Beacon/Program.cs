namespace Lighthouse.Beacon;

public class Program
{
  static void Main(string[] args)
  {
    var apiListener = new APIListener();
    _ = APIListener.Start();
  }
}