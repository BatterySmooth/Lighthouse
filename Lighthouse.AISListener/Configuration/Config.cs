using System.Diagnostics.CodeAnalysis;
using Lighthouse.AISListener.Logging;
using Microsoft.Extensions.Configuration;

namespace Lighthouse.AISListener.Configuration;

public class Config
{
  private static IConfiguration _configuration;
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static string AISKey => GetValueOrDefault<string>("AISKey", "No configuration for AISKey");
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static string SagaBlueMMSI => GetValueOrDefault<string>("SagaBlueMMSI", "No configuration for SagaBlueMMSI");
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static string DBConnectionString => GetValueOrDefault<string>("DBConnectionString", "No configuration for DBConnectionString");
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static string DBPositionReportTable => GetValueOrDefault<string>("DBPositionReportTable", "No configuration for DBPositionReportTable");
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static int RelayPort => GetValueOrDefault<int>("RelayPort", "No configuration for RelayPort");
  
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