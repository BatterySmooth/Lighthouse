using System.Net;
using System.Net.WebSockets;
using System.Text;
using Lighthouse.Configuration;
using Lighthouse.Data;
using Lighthouse.Logging;
using Lighthouse.Websockets;
using Newtonsoft.Json;

namespace Lighthouse.API;

public class APIService
{
  public static async Task Initialise()
  {
    var host = $"http://{Config.APIHost}:{Config.APIPort}/";
    Logger.LogSync($"Starting API Service on {host}");
    HttpListener httpListener = new HttpListener();
    httpListener.Prefixes.Add(host);
    httpListener.Start();

    while (true)
    {
      HttpListenerContext context = await httpListener.GetContextAsync();
      await HandleHttpRequest(context);
    }
  }

  private static async Task HandleHttpRequest(HttpListenerContext context)
  {
    HttpListenerRequest request = context.Request;
    HttpListenerResponse response = context.Response;
    string route = GetRouteFromUrl(request.Url.PathAndQuery);
    
    switch (route.ToLower())
    {
      case "/api/getdata":
        await HandleGetDataRequestAsync(context);
        break;
      default:
        await RespondWithError(response, 404, "Endpoint not found");
        return;
    }
  }

  private static async Task HandleGetDataRequestAsync(HttpListenerContext context)
  {
    HttpListenerRequest request = context.Request;
    HttpListenerResponse response = context.Response;
    
    string? startParam = request.QueryString["start"];
    string? endParam = request.QueryString["end"];

    DateTime startDateTime;
    DateTime endDateTime;

    // Null check
    if (startParam == null || endParam == null)
    {
      await RespondWithError(response, 400, "Both 'start' and 'end' parameters are required");
      return;
    }

    // Try parse DateTime
    try
    {
      startDateTime = DateTime.Parse(startParam);
      endDateTime = DateTime.Parse(endParam);
    }
    catch (Exception e)
    {
      await RespondWithError(response, 400, $"Error when parsing params: {e.Message}");
      return;
    }

    // Check chronology order
    if (startDateTime > endDateTime)
    {
      await RespondWithError(response, 400, "Start time must be before the End time");
      return;
    }
    
    // Check if date is too early
    if (startDateTime < new DateTime(2024, 1, 1))
    {
      await RespondWithError(response, 400, "Start time is too early");
      return;
    }
    
    // Query the database
    var results = Database.GetPositionReportsBetweenDates(startDateTime, endDateTime);
    
    if (results == null)
    {
      await RespondWithError(response, 500, "Failed to get records from database");
      return;
    }
    
    var resultsJson = JsonConvert.SerializeObject(results);

    var buffer = Encoding.UTF8.GetBytes(resultsJson);
    response.ContentLength64 = buffer.Length;
    response.ContentType = "application/json";
    response.StatusCode = 200;
    
    var output = response.OutputStream;
    await output.WriteAsync(buffer, 0, buffer.Length);
    await output.FlushAsync();
    
    output.Close();
  }

  // private static async Task HandleWebsocketRequestAsync(HttpListenerContext context)
  // {
  //   HttpListenerRequest request = context.Request;
  //   if (request.Headers["Upgrade"]?.ToLower() == "websocket" && request.Headers["Connection"]?.ToLower() == "upgrade")
  //   {
  //     var clientWebSocket = new ClientWebSocket();
  //     await clientWebSocket.ConnectAsync(context.Request, CancellationToken.None);
  //
  //     var webSocketService = new WebSocketService(clientWebSocket);
  //
  //     // Now you can use webSocketService to send and receive messages
  //     // For example, to send a welcome message:
  //     await webSocketService.SendMessageAsync("Welcome to the WebSocket service!");
  //
  //     // Remember to properly close the WebSocket connection when done
  //     clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
  //   }
  //   else
  //   {
  //     RespondWithError
  //   }
  // }

  private static async Task RespondWithError(HttpListenerResponse response, int statusCode, string message)
  {
    response.StatusCode = statusCode;
    response.ContentLength64 = message.Length;
    response.ContentType = "text/plain";

    byte[] buffer = Encoding.UTF8.GetBytes(message);
    Stream output = response.OutputStream;
    await output.WriteAsync(buffer, 0, buffer.Length);
    await output.FlushAsync();
    output.Close();
  }
  
  private static string GetRouteFromUrl(string urlPath)
  {
    var segments = urlPath.Split('/');
    return segments.Length > 1 ? segments[1] : "";
  }


}