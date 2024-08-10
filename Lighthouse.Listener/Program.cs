using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using Lighthouse.Listener.Messages;
using Microsoft.CSharp.RuntimeBinder;

namespace Lighthouse.Listener
{
  internal class Program
  {
    public static IConfiguration? Configuration;
    
    public static async Task Main(string[] args)
    {
      // Get secrets config
      Configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
        .Build();
      
      var connectionMsg = new ConnectionMessage
      {
        APIKey = Configuration["AISKey"] ?? throw new Exception("No AIS API Key has been supplied"),
        BoundingBoxes = new List<List<List<double>>> { new() { new List<double> {-180, -90}, new List<double> {180, 90} } },
        FiltersShipMMSI = new List<string> { "235018216", "311045300" },
        FilterMessageTypes = new List<string> { "PositionReport" }
      };
      
      string jsonString = JsonConvert.SerializeObject(connectionMsg);
      
      Console.WriteLine(jsonString);
      
      CancellationTokenSource source = new CancellationTokenSource();
      CancellationToken token = source.Token;
      
      using var ws = new ClientWebSocket();
      await ws.ConnectAsync(new Uri("wss://stream.aisstream.io/v0/stream"), token);
      await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonString)), WebSocketMessageType.Text, true, token);
      byte[] buffer = new byte[4096];
      while (ws.State == WebSocketState.Open)
      {
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
        if (result.MessageType == WebSocketMessageType.Close)
        {
          Console.WriteLine("Websocket closed. Attempting to reconnect...");
          await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
        }
        else
        {
          Console.WriteLine($"Received {Encoding.Default.GetString(buffer, 0, result.Count)}");
        }
      }
    }
    
    private static async Task DelayWithExponentialBackOff(int attempt)
    {
      int delay = Math.Min(2000 * (int)Math.Pow(2, attempt), 30000); // Start at 2 seconds, double each time up to 30 seconds
      Console.WriteLine($"Waiting {delay} ms before retrying...");
      await Task.Delay(delay);
    }
  }
}