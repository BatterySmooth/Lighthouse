using System.Net;
using Lighthouse.Relay.Configuration;
using Lighthouse.Relay.WebSockets;

namespace Lighthouse.Relay;

class Program
{
  static async Task Main(string[] args)
  {
    Config.Initialise();
    
    RelayListener listener = new RelayListener();
    await listener.Start();
  }
}