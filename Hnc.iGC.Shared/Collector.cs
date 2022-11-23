using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Refit;

using Serilog;

namespace Hnc.iGC
{
    public static class Collector
    {
        // 如果我们创建的是一个 Web API 项目，在 Program 类中会有一个 CreateHostBuilder 的静态方法来返回 IHostBuilder 对象：
        public static IHostBuilder CreateHostBuilder<TCollector, TDto>(string[] args)
            where TCollector : Collector<TDto>, new()
            where TDto : BaseDto, new()
            => Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>      // ConfigureServices：用来注册服务；
            {
                // Configure：用来加载中间件
                services.Configure<DeviceSettingsOptions>(hostContext.Configuration.GetSection(DeviceSettingsOptions.DeviceSettings));
                services.Configure<HnciGCApiOptions>(hostContext.Configuration.GetSection(HnciGCApiOptions.HnciGCApi));
                using var scope = services.BuildServiceProvider().CreateScope();

                var iGCApiOptions = scope.ServiceProvider.GetRequiredService<IOptions<HnciGCApiOptions>>();

                // (AddRefitClient) Refit是一个自动类型安全的REST库，是RESTful架构的.NET客户端实现，它基于Attribute，
                // 提供了把REST API返回的数据转化为(Plain Ordinary C# Object,简单C#对象)，POCO to JSON，网络请求(POST，GET,PUT，DELETE等)封装，
                // 内部封装使用HttpClient,前者专注于接口的封装，后者专注于网络请求的高效，二者分工协作。
                // 我们的应用程序通过 refit请求网络，实际上是使用 refit接口层封装请求参数、Header、Url 等信息，之后由HttpClient完成后续的请求操作
                // ，在服务端返回数据之后，HttpClient将原始的结果交给 refit，后者根据用户的需求对结果进行解析的过程。
                services.AddRefitClient<IHnciGCApiCNC>().ConfigureHttpClient(c => c.BaseAddress = new Uri(iGCApiOptions.Value.BaseAddress));
                services.AddRefitClient<IHnciGCApiBalancer>().ConfigureHttpClient(c => c.BaseAddress = new Uri(iGCApiOptions.Value.BaseAddress));
                services.AddRefitClient<IHnciGCApiTemperBox>().ConfigureHttpClient(c => c.BaseAddress = new Uri(iGCApiOptions.Value.BaseAddress));

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

        public virtual bool Connect(string ip, ushort port,string type)
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
