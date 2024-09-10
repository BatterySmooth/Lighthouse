//Implementing: https://github.com/MazyModz/CSharp-WebSocket-Server

using Lighthouse.Relay.Configuration;
using Lighthouse.Relay.WebSockets;

namespace Lighthouse.Relay;

class Program
{
  static async Task Main(string[] args)
  {
    Config.Initialise();

    var websocketServer = new WebSocketService();
    _ = websocketServer.Start();
    
    var listener = new RelayListener();
    await listener.Start();
  }
}