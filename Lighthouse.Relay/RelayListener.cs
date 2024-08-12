using System.Net;
using Lighthouse.Relay.Configuration;

namespace Lighthouse.Relay;

public class RelayListener
{
  private HttpListener _listener;
  public void Start()
  {
    _listener = new HttpListener();
    _listener.Prefixes.Add($"http://{Config.RelayPostEndpoint}/");
    _listener.Start();
    Console.WriteLine("Listening...");

    while (_listener.IsListening)
    {
      var context = _listener.GetContext();
      ProcessPostRequest(context);
    }

    _listener.Stop();
    _listener.Close();
  }

  private void ProcessPostRequest(HttpListenerContext context)
  {
    // Check if the request method is POST
    if (context.Request.HttpMethod == "POST")
    {
      Console.WriteLine("Received Relay Data");
      Console.WriteLine(GetRequestPostData(context.Request));

      byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes("OK");
      context.Response.ContentType = "text/plain";
      context.Response.ContentLength64 = responseBytes.Length;
      Stream output = context.Response.OutputStream;
      output.Write(responseBytes, 0, responseBytes.Length);
      output.Close();
    }
  }
  
  public static string GetRequestPostData(HttpListenerRequest request)
  {
    if (!request.HasEntityBody)
    {
      return null;
    }
    using (System.IO.Stream body = request.InputStream) // here we have data
    {
      using (var reader = new System.IO.StreamReader(body, request.ContentEncoding))
      {
        return reader.ReadToEnd();
      }
    }
  }
  
  
  // private List<ClientWebSocket> _clients = new();
  //
  // public async Task Start()
  // {
  //   var relayPrefix = $"http://localhost:{Config.RelayPort}/";
  //   Logger.LogSync($"Starting Relay server on {relayPrefix}");
  //   
  //   var listener = new HttpListener();
  //   listener.Prefixes.Add(relayPrefix);
  //   listener.Start();
  //
  //   while (true)
  //   {
  //     var context = await listener.GetContextAsync();
  //     var request = context.Request;
  //
  //     if (request.HttpMethod == "GET")
  //     {
  //       var socket = new ClientWebSocket();
  //       await socket.ConnectAsync(request.Url, CancellationToken.None);
  //       _clients.Add(socket);
  //       Logger.LogAsync("Relay Client connected");
  //
  //       // // Listen for messages from this client
  //       // await ReceiveMessages(socket);
  //     }
  //     else
  //     {
  //       context.Response.StatusCode = 400;
  //       context.Response.Close();
  //     }
  //   }
  // }
  //
  // // private async Task ReceiveMessages(ClientWebSocket socket)
  // // {
  // //   try
  // //   {
  // //     while (socket.State == WebSocketState.Open)
  // //     {
  // //       var buffer = new byte[1024 * 4];
  // //       var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
  // //
  // //       if (result.MessageType == WebSocketMessageType.Text)
  // //       {
  // //         var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
  // //         Console.WriteLine($"Received: {message}");
  // //
  // //         // Broadcast the message to all clients
  // //         Broadcast(message);
  // //       }
  // //       else if (result.MessageType == WebSocketMessageType.Close)
  // //       {
  // //         await socket.CloseOutputAsync(result.CloseStatus.Value, result.CloseStatusDescription,
  // //           CancellationToken.None);
  // //         _clients.Remove(socket);
  // //         Console.WriteLine("Client disconnected.");
  // //       }
  // //     }
  // //   }
  // //   catch (Exception ex)
  // //   {
  // //     Console.WriteLine($"An error occurred: {ex}");
  // //   }
  // // }
  //
  // public async Task Broadcast(string message)
  // {
  //   foreach (var client in _clients)
  //   {
  //     if (client.State == WebSocketState.Open)
  //     {
  //       var bytes = Encoding.UTF8.GetBytes(message);
  //       var segment = new ArraySegment<byte>(bytes);
  //       await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
  //     }
  //     else
  //     {
  //       await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
  //       _clients.Remove(client);
  //     }
  //   }
  // }
}