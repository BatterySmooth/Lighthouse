using Lighthouse.Relay.Configuration;

namespace Lighthouse.Relay;

class Program
{
  static void Main(string[] args)
  {
    Config.Initialise();
    
    RelayListener listener = new RelayListener();
    listener.Start();
  }
}