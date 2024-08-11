using Lighthouse.Logging;
using Microsoft.Extensions.Configuration;

namespace Lighthouse.Configuration;

public class Config
{
  private static IConfiguration _configuration;
  
  public static string AISKey => GetValueOrDefault<string>("AISKey", "No configuration for AISKey");
  public static string DBConnectionString => GetValueOrDefault<string>("DBConnectionString", "No configuration for DBConnectionString");
  public static string DBPositionReportTable => GetValueOrDefault<string>("DBConnectionString", "No configuration for DBPositionReportTable");
  public static string SagaBlueMMSI => GetValueOrDefault<string>("SagaBlueMMSI", "No configuration for SagaBlueMMSI");
  public static string APIHost => GetValueOrDefault<string>("APIHost", "No configuration for APIHost");
  public static string APIPort => GetValueOrDefault<string>("APIPort", "No configuration for APIPort");
  
  public static void Initialise()
  {
    Logger.LogSync("Setting up Configuration");
    
    _configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("secrets.json", optional: false, reloadOnChange: true)
      .Build();
  }
  
  private static T GetValueOrDefault<T>(string key, string errorMessage)
  {
    var value = _configuration[key];
    if (value == null)
    {
      throw new InvalidOperationException(errorMessage);
    }
    return (T)Convert.ChangeType(value, typeof(T));
  }
}