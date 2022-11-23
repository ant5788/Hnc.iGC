using Serilog;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Hnc.iGC.Worker.TemperBox
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false, true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", true, true)
               .Build();

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();

            try
            {
                Log.Information("温箱数据采集启动");
                await Collector.CreateHostBuilder<TemperBox, TemperBoxDto>(args).RunConsoleAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息："+ex.ToString());
                Log.Fatal(ex, "温箱数据采集异常");
                Console.ReadKey();
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
