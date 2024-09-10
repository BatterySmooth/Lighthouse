// using Lighthouse.Relay.Configuration;
//
// namespace Lighthouse.Relay.WebSockets
// {
//   public class WebSocketManager
//   {
//     private Server _server;
//     private List<Client> _connectedClients = new();
//     public WebSocketManager()
//     {
//       _server = new Server(Config.WebSocketEndpoint);
//       
//       _server.OnClientConnected += (sender, e) =>
//       {
//         _connectedClients.Add(e.GetClient());
//         Console.WriteLine($"Client with GUID {e.GetClient().GetGuid()} connected!");
//       };
//       _server.OnClientDisconnected += (sender, e) =>
//       {
//         _connectedClients.Remove(e.GetClient());
//         Console.WriteLine($"Client with GUID {e.GetClient().GetGuid()} disconnected!");
//       };
//       
//     }
//
//     public async Task BroadcastAsync(string message)
//     {
//       foreach (var client in _connectedClients)
//       {
//         await _server.SendMessage(client, message);
//       }
//     }
//     
//   }
// }