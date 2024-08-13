using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace Lighthouse.Relay.Configuration;

public class Config
{
  private static IConfiguration _configuration;
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static string RelayPostEndpoint => GetValueOrDefault<string>("RelayPostEndpoint", "No configuration for RelayPostEndpoint");
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static string WebsocketEndpoint => GetValueOrDefault<string>("WebsocketEndpoint", "No configuration for WebsocketEndpoint");
  
  public static void Initialise()
  {
    Console.WriteLine("Setting up Configuration");
    
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