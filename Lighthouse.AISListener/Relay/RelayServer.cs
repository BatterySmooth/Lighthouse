using System.Net;
using System.Net.WebSockets;
using System.Text;
using Lighthouse.AISListener.Configuration;
using Lighthouse.AISListener.Logging;

namespace Lighthouse.AISListener.Relay;

public class RelayServer
{
  private List<ClientWebSocket> _clients = new();

  public async Task Start()
  {
    Logger.LogSync("Starting Relay server");
    
    var listener = new HttpListener();
    listener.Prefixes.Add($"http://*: {Config.RelayPort}/relay");
    listener.Start();

    while (true)
    {
      var context = await listener.GetContextAsync();
      var request = context.Request;

      if (request.HttpMethod == "GET")
      {
        var socket = new ClientWebSocket();
        await socket.ConnectAsync(request.Url, CancellationToken.None);
        _clients.Add(socket);
        Logger.LogAsync("Relay Client connected");

        // // Listen for messages from this client
        // await ReceiveMessages(socket);
      }
      else
      {
        context.Response.StatusCode = 400;
        context.Response.Close();
      }
    }
  }

  // private async Task ReceiveMessages(ClientWebSocket socket)
  // {
  //   try
  //   {
  //     while (socket.State == WebSocketState.Open)
  //     {
  //       var buffer = new byte[1024 * 4];
  //       var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
  //
  //       if (result.MessageType == WebSocketMessageType.Text)
  //       {
  //         var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
  //         Console.WriteLine($"Received: {message}");
  //
  //         // Broadcast the message to all clients
  //         Broadcast(message);
  //       }
  //       else if (result.MessageType == WebSocketMessageType.Close)
  //       {
  //         await socket.CloseOutputAsync(result.CloseStatus.Value, result.CloseStatusDescription,
  //           CancellationToken.None);
  //         _clients.Remove(socket);
  //         Console.WriteLine("Client disconnected.");
  //       }
  //     }
  //   }
  //   catch (Exception ex)
  //   {
  //     Console.WriteLine($"An error occurred: {ex}");
  //   }
  // }

  public async Task Broadcast(string message)
  {
    foreach (var client in _clients)
    {
      if (client.State == WebSocketState.Open)
      {
        var bytes = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(bytes);
        await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
      }
      else
      {
        await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        _clients.Remove(client);
      }
    }
  }
}