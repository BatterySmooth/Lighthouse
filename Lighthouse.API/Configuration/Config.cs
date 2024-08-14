using System.Diagnostics.CodeAnalysis;

namespace Lighthouse.API.Configuration;

public class Config
{
  private static IConfiguration _configuration;
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static string DBConnectionString => GetValueOrDefault<string>("DBConnectionString", "No configuration for DBConnectionString");
  
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static string DBPositionReportTable => GetValueOrDefault<string>("DBPositionReportTable", "No configuration for DBPositionReportTable");

  public static void Initialise()
  {
    Console.WriteLine("Setting up Configuration");
    
    _configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("secrets.test.json", optional: false, reloadOnChange: true)
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