using Lighthouse.AISListener;
using Lighthouse.Configuration;
using Lighthouse.Data;

namespace Lighthouse
{
  internal class Program
  {
    public static CancellationToken Token;
    public static async Task Main(string[] args)
    {
      Config.Initialise();
      Database.Initialise();
      
      AISService aisService = new AISService();
      await aisService.InitializeAsync();
      
      await Task.Delay(-1);
    }
    
  }
}