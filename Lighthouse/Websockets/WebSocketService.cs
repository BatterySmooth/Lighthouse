using System.Net.WebSockets;
using System.Text;

namespace Lighthouse.Websockets;

public class WebSocketService
{
  private readonly ClientWebSocket _webSocket;

  public WebSocketService(ClientWebSocket webSocket)
  {
    _webSocket = webSocket;
  }

  public async Task SendMessageAsync(string message)
  {
    var bytesToSend = Encoding.UTF8.GetBytes(message);
    await _webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
  }

  public async Task<string> ReceiveMessageAsync()
  {
    var buffer = new byte[1024 * 4];
    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    return Encoding.UTF8.GetString(buffer, 0, result.Count);
  }
}