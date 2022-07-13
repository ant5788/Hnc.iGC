using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Hnc.iGC.Worker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Collector.CreateHostBuilder<ModbusTCP_DistributedIO, CNCDto>(args).RunConsoleAsync();
        }

    }
}
