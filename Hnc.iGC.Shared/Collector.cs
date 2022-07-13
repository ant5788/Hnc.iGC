using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Refit;

using Serilog;

namespace Hnc.iGC
{
    public static class Collector
    {
        public static IHostBuilder CreateHostBuilder<TCollector, TDto>(string[] args)
            where TCollector : Collector<TDto>, new()
            where TDto : BaseDto, new()
            => Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<DeviceSettingsOptions>(hostContext.Configuration.GetSection(DeviceSettingsOptions.DeviceSettings));
                services.Configure<HnciGCApiOptions>(hostContext.Configuration.GetSection(HnciGCApiOptions.HnciGCApi));
                using var scope = services.BuildServiceProvider().CreateScope();

                var iGCApiOptions = scope.ServiceProvider.GetRequiredService<IOptions<HnciGCApiOptions>>();
                services.AddRefitClient<IHnciGCApiCNC>().ConfigureHttpClient(c => c.BaseAddress = new Uri(iGCApiOptions.Value.BaseAddress));
                services.AddRefitClient<IHnciGCApiBalancer>().ConfigureHttpClient(c => c.BaseAddress = new Uri(iGCApiOptions.Value.BaseAddress));

                services.AddHostedService<CollectorWorker<TCollector, TDto>>();

#if !DEBUG
                services.AddSingleton<IDongleValidator, ViKeyValidator>();
                services.AddHostedService<DongleWatcher>();
#endif
            });
    }

    public abstract class Collector<TDto> : ICollector<TDto>
            where TDto : BaseDto, new()
    {
        public abstract string Protocal { get; }

        protected bool isConnected;

        public virtual bool IsConnected => isConnected;

        public virtual bool Connect(string ip, ushort port)
        {
            isConnected = true;
            return true;
        }

        public virtual bool Disconnect()
        {
            isConnected = false;
            return true;
        }

        public abstract void SetDataTo(TDto dto);

    }
}
