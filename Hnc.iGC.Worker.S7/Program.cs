using Serilog;

namespace Hnc.iGC.Worker.S7
{
    internal class Program
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
                Log.Information("采集程序启动");
                await Collector.CreateHostBuilder<S7_CSharp, BalancerDto>(args).RunConsoleAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "采集程序中断");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
