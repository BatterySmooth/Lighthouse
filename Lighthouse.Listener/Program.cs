using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using Lighthouse.Listener.Models.IncomingMessages;
using Lighthouse.Listener.Models.OutgoingMessages;

namespace Lighthouse.Listener
{
  internal class Program
  {
    public const string SagaBlueMmsi = "235018216";
    public static IConfiguration? Configuration;

    public static async Task Main(string[] args)
    {
      Configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
        .Build();

      var connectionMsg = new ConnectionRequest
      {
        APIKey = Configuration["AISKey"] ?? throw new Exception("No AIS API Key has been supplied"),
        BoundingBoxes = new List<List<List<double>>> { new() { new List<double> { -180, -90 }, new List<double> { 180, 90 } } },
        FiltersShipMMSI = new List<string> { SagaBlueMmsi, "311045300" },
        FilterMessageTypes = new List<string> { "PositionReport" }
      };

      string jsonString = JsonConvert.SerializeObject(connectionMsg);

      CancellationTokenSource source = new CancellationTokenSource();
      CancellationToken token = source.Token;

      int attempt = 0;
      while (!token.IsCancellationRequested)
      {
        using var ws = new ClientWebSocket();
        try
        {
          Console.WriteLine("Connecting...");
          await ws.ConnectAsync(new Uri("wss://stream.aisstream.io/v0/stream"), token);
          await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonString)), WebSocketMessageType.Text, true, token);

          byte[] buffer = new byte[4096];
          while (ws.State == WebSocketState.Open)
          {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
              await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
              break;
            }
            
            var msg = JsonConvert.DeserializeObject<ReceivedMessage>(Encoding.Default.GetString(buffer, 0, result.Count));
            Console.WriteLine("RECIEVED MESSAGE:");
            Console.WriteLine($"Ship: {msg.MetaData.ShipName}|");
            Console.WriteLine($"Pos: {msg.Message.PositionReport.Latitude}, {msg.Message.PositionReport.Longitude}");
            Console.WriteLine($"Head: {msg.Message.PositionReport.TrueHeading}");
            Console.WriteLine("=====");
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Connection error: {ex.Message}");
        }

        // Wait before retrying connection
        await DelayWithExponentialBackOff(attempt++);
      }
    }

    private static async Task DelayWithExponentialBackOff(int attempt)
    {
      // Start at 2 seconds, double each time up to 30 seconds
      int delay = Math.Min(2000 * (int)Math.Pow(2, attempt), 30000);
      Console.WriteLine($"Waiting {delay} ms before retrying...");
      await Task.Delay(delay);
    }
  }
}