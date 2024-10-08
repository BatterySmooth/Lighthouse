﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lighthouse.AISListener.AIS.IncomingMessages;
using Lighthouse.AISListener.AIS.Subscription;
using Lighthouse.Tower.Logging;
using Lighthouse.Tower.Configuration;
using Lighthouse.Tower.Data;
using Lighthouse.Tower.Data.Models;
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

    while (_isRunning)
    {
      try
      {
        if (_webSocket.State != WebSocketState.Open)
        {
          await HandleWebsocketClose();
          break;
        }
        
        var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), Token);
        if (result.MessageType == WebSocketMessageType.Close)
        {
          await HandleWebsocketClose();
          break;
        }
        
        var msgRaw = Encoding.Default.GetString(buffer, 0, result.Count);
        var msg = JsonConvert.DeserializeObject<ReceivedMessage>(msgRaw);
        
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
      catch (WebSocketException ex)
      {
        Logger.LogAsync($"WebSocket exception occurred: {ex.Message}");
        await HandleWebsocketClose();
      }
      catch (Exception ex)
      {
        Logger.LogAsync($"Unexpected exception occurred: {ex.Message}");
        await HandleWebsocketClose();
      }
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
        HttpResponseMessage response = await _httpClient.PostAsync($"http://{Config.RelayPostEndpoint}/", httpContent, Token);
        if (response.IsSuccessStatusCode)
        {
          await response.Content.ReadAsStringAsync(Token);
          _relayDataQueue.Remove(msg);
        }
        else
        {
          Logger.LogAsync($"Error sending message to http://{Config.RelayPostEndpoint}/ : {(int)response.StatusCode} {response.ReasonPhrase}");
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
    try
    {
      await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, Token);
    }
    catch (Exception ex)
    {
      Logger.LogAsync($"Error closing WebSocket: {ex.Message}");
    }

    _attempt++;
    await DelayWithExponentialBackOff(_attempt);
    
    try
    {
      await Connect();
      await ListenAndProcess();
    }
    catch (Exception ex)
    {
      Logger.LogAsync($"Failed to reconnect: {ex.Message}");
      await DelayWithExponentialBackOff(_attempt);
    }
  }
  
  private static async Task DelayWithExponentialBackOff(int attempt)
  {
    // Start at 2 seconds, double each time up to 30 seconds
    int delay = Math.Min(Math.Abs(2000 * (int)Math.Pow(2, attempt)), 30000);
    Logger.LogSync($"Waiting {delay} ms before retrying...");
    await Task.Delay(delay);
  }
  
}