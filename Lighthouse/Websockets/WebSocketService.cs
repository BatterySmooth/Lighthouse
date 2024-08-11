// using System.Collections.Concurrent;
// using System.Net;
// using System.Net.WebSockets;
// using System.Text;
//
// namespace Lighthouse.Websockets;
//
// public class WebSocketService
// {
//   private ConcurrentDictionary<Guid, ClientWebSocket> _connections = new();
//
//   public async Task ProcessWebSocketConnectionRequest(HttpListenerContext context)
//   {
//     var webSocket = new ClientWebSocket();
//     Uri requestUri = new Uri(context.Request.RawUrl);
//     await webSocket.ConnectAsync(requestUri, CancellationToken.None);
//   }
//   
//   public async Task Connect(Guid clientId, ClientWebSocket webSocket)
//   {
//     _connections.TryAdd(clientId, webSocket);
//   }
//
//   public async Task Disconnect(Guid clientId)
//   {
//     var removed = false;
//     _connections.TryRemove(clientId, out _);
//     removed = true;
//   }
//
//   public async Task SendMessageAll(string message)
//   {
//     foreach (var pair in _connections)
//     {
//       if (pair.Value.State == WebSocketState.Open)
//       {
//         await pair.Value.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
//       }
//     }
//   }
//   
// }
