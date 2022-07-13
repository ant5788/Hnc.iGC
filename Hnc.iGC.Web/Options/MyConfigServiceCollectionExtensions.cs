using Hnc.iGC.Web.Options;

namespace Microsoft.Extensions.Configuration
{
    public static class MyConfigServiceCollectionExtensions
    {
        public static IServiceCollection AddiGCOptions(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MQTTOptions>(config.GetSection(MQTTOptions.MQTT));
            services.Configure<SerialPortOptions>(config.GetSection(SerialPortOptions.SerialPort));
            return services;
        }
    }
}
