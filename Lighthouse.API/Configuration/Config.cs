using Microsoft.Extensions.Configuration;

namespace Lighthouse.Configuration;

public class Config
{
  private static IConfiguration _configuration;
  
  public static string DBConnectionString => GetValueOrDefault<string>("DBConnectionString", "No configuration for DBConnectionString");
  public static string DBPositionReportTable => GetValueOrDefault<string>("DBPositionReportTable", "No configuration for DBPositionReportTable");

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