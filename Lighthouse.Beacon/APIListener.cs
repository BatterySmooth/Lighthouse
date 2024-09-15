using System.Collections.Specialized;
using System.Net;
using Lighthouse.Beacon.Configuration;
using Lighthouse.Beacon.Data;
using Lighthouse.Beacon.Data.Models;
using Newtonsoft.Json;

namespace Lighthouse.Beacon;

public class APIListener
{
  public APIListener() { }
  
  public static async Task Start()
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
      await ProcessRequest(context);
    }

    listener.Stop();
    listener.Close();
  }
  
  private static Task ProcessRequest(HttpListenerContext context)
  {
    try
    {
      var requestMethod = context.Request.HttpMethod;
      var urlPath = context.Request.Url.AbsolutePath;

      if (requestMethod != "GET")
        CloseConnection(context, HttpStatusCode.BadRequest, "Only GET requests are supported");

      switch (urlPath.ToLower())
      {
        case "/PositionReports":
          var queryString = context.Request.QueryString;
          var startDateString = queryString["startDate"];
          var endDateString = queryString["endDate"];
          if (startDateString == null || endDateString == null)
          {
            CloseConnection(context, HttpStatusCode.BadRequest, "Start date and End date are required");
            break;
          }
          var startDate = DateTime.Parse(startDateString);
          var endDate = DateTime.Parse(endDateString);
          if (startDate > endDate)
          {
            CloseConnection(context, HttpStatusCode.BadRequest, "Start date must be before End date");
            break;
          }
          var dateRange = new DateRange(startDate, endDate);
          GetPositionReports(context, dateRange);
          break;
        case "/teapot":
          CloseConnection(context, 418, "The server refuses to brew coffee because it is, permanently, a teapot");
          break;
        default:
          CloseConnection(context, HttpStatusCode.NotFound, "Endpoint not found");
          break;
      }
    }
    catch (Exception e)
    {
      CloseConnection(context, HttpStatusCode.InternalServerError, "Internal Server Error");
      Console.WriteLine($"Internal Server Error: {e.Message}");
      throw;
    }

    return Task.CompletedTask;
  }
  
  private static void GetPositionReports(HttpListenerContext context, DateRange dateRange)
  {
    if (dateRange.StartDate > dateRange.EndDate)
    {
      Console.WriteLine($"GET FAILED | GetPositionReports {dateRange.StartDate} - {dateRange.EndDate}: Start date must be before end date");
      CloseConnection(context, HttpStatusCode.BadRequest, "Start date must be before end date");
    }
    
    var dbRecords = Database.GetPositionReportsBetweenDates(dateRange.StartDate, dateRange.EndDate);
    var jsonResponse = JsonConvert.SerializeObject(dbRecords);
    var buffer = System.Text.Encoding.UTF8.GetBytes(jsonResponse);
    context.Response.ContentType = "application/json";
    context.Response.ContentLength64 = buffer.Length;
    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
    context.Response.OutputStream.Close();
  }
  
  private static void CloseConnection(HttpListenerContext context, HttpStatusCode statusCode, string reasonPhrase)
  {
    context.Response.StatusCode = (int) statusCode;
    context.Response.StatusDescription = reasonPhrase;
    context.Response.OutputStream.Close();
  }
  private static void CloseConnection(HttpListenerContext context, int statusCode, string reasonPhrase)
  {
    context.Response.StatusCode = statusCode;
    context.Response.StatusDescription = reasonPhrase;
    context.Response.OutputStream.Close();
  }
  private byte[] GetRequestBodyAsBuffer(HttpListenerContext context)
  {
    using var reader = new StreamReader(context.Request.InputStream, leaveOpen: true);
    var content = reader.ReadToEndAsync().Result;
    return System.Text.Encoding.UTF8.GetBytes(content);
  }
}