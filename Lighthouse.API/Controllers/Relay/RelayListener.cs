using System.Net;
using Lighthouse.API.Configuration;

namespace Lighthouse.API.Controllers.Relay;

public class RelayListener
{
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
      var buffer = GetRequestBodyAsBuffer(context);
      await RelayController.Broadcast(buffer, 0, buffer.Length);
    }
  }
  
  public static byte[] GetRequestBodyAsBuffer(HttpListenerContext context)
  {
    using var reader = new StreamReader(context.Request.InputStream, leaveOpen: true);
    var content = reader.ReadToEndAsync().Result;
    return System.Text.Encoding.UTF8.GetBytes(content);
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