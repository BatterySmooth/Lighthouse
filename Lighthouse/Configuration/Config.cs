using Lighthouse.Logging;
using Microsoft.Extensions.Configuration;

namespace Lighthouse.Configuration;

public class Config
{
  public static IConfiguration Configuration { get; set; }
  
  public static string AISKey => Configuration["AISKey"] ?? throw new Exception("No configuration for AISKey");
  public static string SQLConnectionString => Configuration["SQLConnectionString"] ?? throw new Exception("No configuration for SQLConnectionString");
  public static string SagaBlueMMSI => Configuration["SagaBlueMMSI"] ?? throw new Exception("No configuration for SagaBlueMMSI");
  
  public static void Initialise()
  {
    Logger.LogSync("Setting up Configuration...");
    
    Configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("secrets.json", optional: false, reloadOnChange: true)
      .Build();
  }
}