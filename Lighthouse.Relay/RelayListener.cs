using System.Net;
using Lighthouse.Relay.Configuration;
using Lighthouse.Relay.WebSockets;

namespace Lighthouse.Relay;

public class RelayListener
{
  // private readonly WebSocketManager _webSocketManager = new();

  public async Task Start()
  {
    Console.WriteLine("Starting Relay listener");
    var hostName = $"http://{Config.RelayPostEndpoint}/";
    var listener = new HttpListener();
    listener.Prefixes.Add(hostName);
    listener.Start();
    Console.WriteLine($"Relay listening on {hostName}");

    while (listener.IsListening)
    {
      var context = await listener.GetContextAsync();
      await ProcessPostRequest(context);
    }

    listener.Stop();
    listener.Close();
  }

  private async Task ProcessPostRequest(HttpListenerContext context)
  {
    if (context.Request.HttpMethod == "POST")
    {
      byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes("OK");
      context.Response.ContentType = "text/plain";
      context.Response.ContentLength64 = responseBytes.Length;
      Stream output = context.Response.OutputStream;
      await output.WriteAsync(responseBytes);
      output.Close();
      
      Console.WriteLine("Broadcasting to clients");
      // await _webSocketManager.BroadcastAsync(GetRequestPostData(context.Request));
    }
  }

  private static string GetRequestPostData(HttpListenerRequest request)
  {
    if (!request.HasEntityBody)
      return "[Empty Request]";

    using var body = request.InputStream;
    using var reader = new StreamReader(body, request.ContentEncoding);
    return reader.ReadToEnd();
  }
  
}
