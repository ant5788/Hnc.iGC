using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Text.Json;

namespace Hnc.iGC
{
    public class CollectorWorker<TCollector, TDto> : BackgroundService
        where TCollector : Collector<TDto>, new()
        where TDto : BaseDto, new()
    {
        public CollectorWorker(ILogger<CollectorWorker<TCollector, TDto>> logger, IOptionsSnapshot<DeviceSettingsOptions> devicesOptions,
            IHnciGCApiCNC hnciGCApiCNC, IHnciGCApiBalancer hnciGCApiBalancer)
        {
            Logger = logger;
            this.hnciGCApiCNC = hnciGCApiCNC;
            this.hnciGCApiBalancer = hnciGCApiBalancer;
            DevicesOptions = devicesOptions.Value;
        }

        public ILogger<CollectorWorker<TCollector, TDto>> Logger { get; }
        public DeviceSettingsOptions DevicesOptions { get; }

        private readonly TCollector collector = new();
        private readonly IHnciGCApiCNC hnciGCApiCNC;
        private readonly IHnciGCApiBalancer hnciGCApiBalancer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("{worker} 后台任务启动", typeof(TCollector).FullName);
                List<Task> collectTasks = new();
                foreach (var deviceSetting in DevicesOptions.Devices.Where(p => p.EnableDataCollect && p.Protocal == collector.Protocal))
                {
                    collectTasks.Add(CollectTask(deviceSetting, stoppingToken));
                }
                await Task.WhenAll(collectTasks);
                Logger.LogInformation("{worker} 后台任务取消", typeof(TCollector).FullName);
                Logger.LogInformation("{worker} 等待5秒后重启", typeof(TCollector).FullName);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private Task CollectTask(DeviceSettingsOptions.DeviceSetting deviceSetting, CancellationToken stoppingToken) => Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (collector.Connect(deviceSetting.IP, deviceSetting.Port))
                    {
                        Logger.LogInformation("{name}-{ip}:{port}连接成功", deviceSetting.Name, deviceSetting.IP, deviceSetting.Port);
                    }
                    else
                    {
                        Logger.LogError("{name}-{ip}:{port}无法连接，中断此次任务", deviceSetting.Name, deviceSetting.IP, deviceSetting.Port);
                        return;
                    }
                    TDto model = new()
                    {
                        DeviceId = deviceSetting.DeviceId,
                        Name = deviceSetting.Name,
                        Description = deviceSetting.Description,
                        Protocal = deviceSetting.Protocal,
                        IP = deviceSetting.IP,
                        Port = deviceSetting.Port
                    };
                    collector.SetDataTo(model);
                    switch (model)
                    {
                        case CNCDto cnc:
                            if (await hnciGCApiCNC.PostCNC(cnc) != null)
                            {
                                var jsonStr = JsonSerializer.Serialize(cnc, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                    PropertyNamingPolicy = null,
                                    WriteIndented = true,
                                    MaxDepth = 64
                                });
                                Logger.LogDebug("----------{time}----------{newLine}{json}", DateTime.Now, Environment.NewLine, jsonStr);
                            }
                            break;
                        case BalancerDto balancer:
                            if (await hnciGCApiBalancer.PostBalancer(balancer) != null)
                            {
                                var jsonStr = JsonSerializer.Serialize(balancer, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                    PropertyNamingPolicy = null,
                                    WriteIndented = true,
                                    MaxDepth = 64
                                });
                                Logger.LogDebug("----------{time}----------{newLine}{json}", DateTime.Now, Environment.NewLine, jsonStr);
                            }
                            break;
                    }
                    if (collector.Disconnect())
                    {
                        Logger.LogInformation("{name}-{ip}:{port}断开成功", deviceSetting.Name, deviceSetting.IP, deviceSetting.Port);
                    }
                    else
                    {
                        Logger.LogError("{name}-{ip}:{port}断开失败", deviceSetting.Name, deviceSetting.IP, deviceSetting.Port);
                    }
                    Task.Delay(deviceSetting.IntervalInMilliseconds).Wait();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "{name}-{ip}:{port}数据采集任务抛出异常", deviceSetting.Name, deviceSetting.IP, deviceSetting.Port);
                    //throw;
                }
            }
            Logger.LogInformation("{name}-{ip}:{port}数据采集任务已结束", deviceSetting.Name, deviceSetting.IP, deviceSetting.Port);
        }, stoppingToken);

    }
}
