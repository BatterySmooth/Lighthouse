using System.Collections.Concurrent;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace Lighthouse.API.Controllers.Relay;

public class RelayController : ControllerBase
{
  private static readonly ConcurrentDictionary<string, WebSocket> _connections = new();
  private const string ConnectionIdPrefix = "connection_";
    
  [Route("/relay")]
  public async Task Get()
  {
    if (HttpContext.WebSockets.IsWebSocketRequest)
    {
      using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
      var clientid = $"{ConnectionIdPrefix}{Guid.NewGuid():N}";
      _connections.TryAdd(clientid, webSocket);
      Console.WriteLine($"Client Connected: {clientid}");
      await ProcessSocket(webSocket);
    }
    else
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
  }
  
  private static async Task ProcessSocket(WebSocket webSocket)
  {
    var buffer = new byte[1024 * 4];
    var receiveResult = await webSocket.ReceiveAsync(
      new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!receiveResult.CloseStatus.HasValue)
    {
      // await webSocket.SendAsync(
      //   new ArraySegment<byte>(buffer, 0, receiveResult.Count),
      //   receiveResult.MessageType,
      //   receiveResult.EndOfMessage,
      //   CancellationToken.None);
      //
      // receiveResult = await webSocket.ReceiveAsync(
      //   new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
  }
  
  public static async Task Broadcast(byte[] buffer, int offset, int count)
  {
    foreach (var connection in _connections)
    {
      try
      {
        await connection.Value.SendAsync(new ArraySegment<byte>(buffer, offset, count), WebSocketMessageType.Text, true, CancellationToken.None);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error broadcasting to connection {connection.Key}: {ex}");
        await connection.Value.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Closed due to inactivity", CancellationToken.None);
        _connections.TryRemove(connection.Key, out _);
      }
    }
  }
  
  [Route("/post")]
  public async Task Post()
  {
    
  }
  
}