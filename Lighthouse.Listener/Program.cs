using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using Lighthouse.Listener.data;
using Lighthouse.Listener.Logging;
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
      Logger.LogSync("Setting up Configuration...");
      Configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
        .Build();
      
      Logger.LogSync("Connecting to Database...");
      var dbContext = new LighthouseDbContext();

      Logger.LogSync("Configuring AIS Connection Request...");
      var connectionMsg = new ConnectionRequest
      {
        APIKey = Configuration["AISKey"] ?? throw new Exception("No AIS API Key has been supplied"),
        BoundingBoxes = new List<List<List<double>>> { new() { new List<double> { -180, -90 }, new List<double> { 180, 90 } } },
        FiltersShipMMSI = new List<string> { SagaBlueMmsi, "229490000", "235088985" },
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
          Logger.LogSync("Connecting to AIS...");
          await ws.ConnectAsync(new Uri("wss://stream.aisstream.io/v0/stream"), token);
          await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonString)), WebSocketMessageType.Text, true, token);
          byte[] buffer = new byte[4096];
          
          Logger.LogSync("Awaiting messages");
          while (ws.State == WebSocketState.Open)
          {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
              await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
              break;
            }
            
            var msg = JsonConvert.DeserializeObject<ReceivedMessage>(Encoding.Default.GetString(buffer, 0, result.Count));
            
            try
            {
              // Save to DB
              var dbPositionReport = new DbPositionReport()
              {
                PositionReportID = new Guid(),
                ReceivedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                MMSI = msg.Message.PositionReport.UserID,
                RateOfTurn = msg.Message.PositionReport.RateOfTurn,
                Latitude = msg.Message.PositionReport.Latitude,
                Longitude = msg.Message.PositionReport.Longitude,
                TrueHeading = msg.Message.PositionReport.TrueHeading
              };
              dbContext.PositionReports.Add(dbPositionReport);
              await dbContext.SaveChangesAsync(token);
              Logger.LogAsync($"RECEIVED MESSAGE | Ship: {msg.MetaData.ShipName} | Pos: {msg.Message.PositionReport.Latitude}, {msg.Message.PositionReport.Longitude} | Head: {msg.Message.PositionReport.TrueHeading}");
            }
            catch (Exception e)
            {
              Logger.LogAsync($"FAILED MESSAGE   | Ship: {msg.MetaData.ShipName} | Pos: {msg.Message.PositionReport.Latitude}, {msg.Message.PositionReport.Longitude} | Head: {msg.Message.PositionReport.TrueHeading}");
              Logger.LogAsync($"Failed to write to database: {e}");
            }
          }
        }
        catch (Exception ex)
        {
          Logger.LogAsync($"General error: {ex.Message}");
          Logger.LogAsync("Reconnecting...");
          await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
        }

        // Wait before retrying connection
        await DelayWithExponentialBackOff(attempt++);
      }
    }

    private static async Task DelayWithExponentialBackOff(int attempt)
    {
      // Start at 2 seconds, double each time up to 30 seconds
      int delay = Math.Min(2000 * (int)Math.Pow(2, attempt), 30000);
      Logger.LogSync($"Waiting {delay} ms before retrying...");
      await Task.Delay(delay);
    }
  }
}