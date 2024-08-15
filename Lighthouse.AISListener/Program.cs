using Lighthouse.AISListener.AIS;
using Lighthouse.AISListener.AIS.Subscription;
using Lighthouse.AISListener.Configuration;
using Lighthouse.AISListener.Data;

namespace Lighthouse.AISListener
{
  internal class Program
  {
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