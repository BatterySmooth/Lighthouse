using System.Net;
using System.Net.Sockets;

namespace Lighthouse.Relay.WebSockets;

public class WebSocketService
{
  private TcpListener _server;
  private bool _running;

  public WebSocketService()
  {
    _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
  }
  
  public async Task Start()
  {
    _server.Start();
    _running = true;
    // Console.WriteLine("Server has started on 127.0.0.1:80.{0}Waiting for a connection…", Environment.NewLine);
    Console.WriteLine($"Listening on port 8080");

    while (_running)
    {
      var client = await _server.AcceptTcpClientAsync();
      Console.WriteLine("A client connected.");
    }
    
  }
}