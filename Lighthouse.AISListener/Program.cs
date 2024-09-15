using System.Threading.Tasks;
using Lighthouse.AISListener.AIS;
using Lighthouse.Tower.Configuration;
using Lighthouse.Tower.Data;

namespace Lighthouse.AISListener
{
  internal class Program
  {
    public static async Task Main(string[] args)
    {
      var config = new Config();
      var db = new Database();

      AISService aisService = new AISService();
      await aisService.InitializeAsync();
      
      await Task.Delay(-1);
    }
    
  }
}