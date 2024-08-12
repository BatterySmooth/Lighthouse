using Lighthouse.AISListener.AIS;
using Lighthouse.AISListener.Configuration;
using Lighthouse.AISListener.Data;
using Lighthouse.AISListener.Relay;

namespace Lighthouse.AISListener
{
  internal class Program
  {
    public static async Task Main(string[] args)
    {
      Config.Initialise();
      Database.Initialise();
      
      var relayServer = new RelayServer();
      await relayServer.Start();
      
      AISService aisService = new AISService(relayServer);
      await aisService.InitializeAsync();

      await Task.Delay(-1);
    }
    
  }
}