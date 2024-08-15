using System.Net.WebSockets;
using System.Text;
using Lighthouse.AISListener.AIS.IncomingMessages;
using Lighthouse.AISListener.AIS.OutgoingMessages;
using Lighthouse.AISListener.AIS.Subscription;
using Lighthouse.AISListener.Configuration;
using Lighthouse.AISListener.Data;
using Lighthouse.AISListener.Data.Models;
using Lighthouse.AISListener.Logging;
using Newtonsoft.Json;

namespace Lighthouse.AISListener.AIS;

public class AISService
{
  private readonly ClientWebSocket _webSocket;
  private readonly HttpClient _httpClient;
  private CancellationToken Token { get; }
  private List<string> _relayDataQueue;
  private bool _isRunning;
  private int _attempt;

  public AISService()
  {
    var cancellationTokenSource = new CancellationTokenSource();
    Token = cancellationTokenSource.Token;
    _webSocket = new ClientWebSocket();
    _httpClient = new HttpClient();
    _relayDataQueue = new List<string>();
  }

  public async Task InitializeAsync()
  {
    Logger.LogSync("Starting AIS Service");
    _isRunning = true;
    _attempt = 0;
    await Connect();
    await ListenAndProcess();
  }

  private async Task Connect()
  {
    var subscription = new SubscriptionBuilder(Config.AISKey)
      .AddBoundingBox((-180, -90), (180, 90))
      .AddShipMMSIFilter(Config.SagaBlueMMSI)
      .AddMessageTypeFilter(MessageType.PositionReport)
      .Build()
      .Serialise();
    
    await _webSocket.ConnectAsync(new Uri("wss://stream.aisstream.io/v0/stream"), Token);
    await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(subscription)), WebSocketMessageType.Text, true, Token);
  }

  private async Task ListenAndProcess()
  {
    byte[] buffer = new byte[4096];
    Logger.LogSync("Awaiting AIS messages");

    while (_isRunning && _webSocket.State == WebSocketState.Open)
    {
      var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), Token);
      if (result.MessageType == WebSocketMessageType.Close)
      {
        await HandleWebsocketClose();
        break;
      }

      var msgRaw = Encoding.Default.GetString(buffer, 0, result.Count);
      var msg = JsonConvert.DeserializeObject<ReceivedMessage>(msgRaw);

      Logger.LogAsync(msgRaw);
      // Save to Database
      try
      {
        await Database.Save(new PositionReportRecord()
        {
          PositionReportID = new Guid(),
          ReceivedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
          MMSI = msg.Message.PositionReport.UserID,
          RateOfTurn = msg.Message.PositionReport.RateOfTurn,
          Latitude = msg.Message.PositionReport.Latitude,
          Longitude = msg.Message.PositionReport.Longitude,
          TrueHeading = msg.Message.PositionReport.TrueHeading
        });
        Logger.LogAsync($"RECEIVED MESSAGE | Ship: {msg.MetaData.ShipName} | Pos: {msg.Message.PositionReport.Latitude}, {msg.Message.PositionReport.Longitude} | Head: {msg.Message.PositionReport.TrueHeading}");
      }
      catch (Exception e)
      {
        Logger.LogAsync($"FAILED MESSAGE   | Ship: {msg.MetaData.ShipName} | Pos: {msg.Message.PositionReport.Latitude}, {msg.Message.PositionReport.Longitude} | Head: {msg.Message.PositionReport.TrueHeading}");
        Logger.LogAsync($"Failed to write to database: {e}");
      }

      // Send relay message
      // _relayDataQueue.Add(msgRaw);
      // await SendRelayMessage();
    }
  }
  
  private async Task SendRelayMessage()
  {
    var exceptions = new List<HttpRequestException>();
    foreach (var msg in _relayDataQueue.ToList())
    {
      try
      {
        var httpContent = new StringContent(msg, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync($"http://{Config.RelayEndpoint}/", httpContent, Token);
        if (response.IsSuccessStatusCode)
        {
          await response.Content.ReadAsStringAsync(Token);
          _relayDataQueue.Remove(msg);
        }
        else
        {
          Logger.LogAsync($"Error sending message to http://{Config.RelayEndpoint}/ : {(int)response.StatusCode} {response.ReasonPhrase}");
        }
      }
      catch (HttpRequestException e)
      {
        exceptions.Add(e);
      }
      catch (Exception e)
      {
        Logger.LogAsync($"Generic error when trying to post: {e}");
      }
    }
    
    // Notify of failed bundles
    if (exceptions.Count > 0)
      Logger.LogAsync($"Failed to send {exceptions.Count} bundles to the relay:{Environment.NewLine}{String.Join(Environment.NewLine, exceptions.Select(e => e.Message))}");
  }

  private async Task HandleWebsocketClose()
  {
    Logger.LogAsync("Websocket connection closed. Reconnecting...");
    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, Token);
    await Connect();
    await ListenAndProcess();
  }
  
  private static async Task DelayWithExponentialBackOff(int attempt)
  {
    // Start at 2 seconds, double each time up to 30 seconds
    int delay = Math.Min(2000 * (int)Math.Pow(2, attempt), 30000);
    Logger.LogSync($"Waiting {delay} ms before retrying...");
    await Task.Delay(delay);
  }
  
}