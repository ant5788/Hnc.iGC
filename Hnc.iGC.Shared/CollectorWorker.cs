using Microsoft.Extensions.DependencyInjection;
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
            IHnciGCApiCNC hnciGCApiCNC, IHnciGCApiTemperBox hnciGCApiTemperBox, IHnciGCApiBalancer hnciGCApiBalancer, IServiceScopeFactory factory)
        {
            Logger = logger;
            //this.hnciGCApiCNC = hnciGCApiCNC;
            //this.hnciGCApiBalancer = hnciGCApiBalancer;
            //this.hnciGCApiTemperBox = hnciGCApiTemperBox;
            DevicesOptions = devicesOptions.Value;
            this.hnciGCApiCNC = factory.CreateScope().ServiceProvider.GetRequiredService<IHnciGCApiCNC>();
            this.hnciGCApiBalancer = factory.CreateScope().ServiceProvider.GetRequiredService<IHnciGCApiBalancer>();
            this.hnciGCApiTemperBox = factory.CreateScope().ServiceProvider.GetRequiredService<IHnciGCApiTemperBox>();
            // Console.WriteLine(DevicesOptions.Devices[1].Name);
        }

        public ILogger<CollectorWorker<TCollector, TDto>> Logger { get; }
        public DeviceSettingsOptions DevicesOptions { get; }

        private readonly TCollector collector = new();
        private readonly IHnciGCApiCNC hnciGCApiCNC;
        private readonly IHnciGCApiBalancer hnciGCApiBalancer;
        private readonly IHnciGCApiTemperBox hnciGCApiTemperBox;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("{worker} 后台任务启动", typeof(TCollector).FullName);
                List<Task> collectTasks = new();
                foreach (var deviceSetting in DevicesOptions.Devices.Where(p => p.EnableDataCollect && p.Protocal == collector.Protocal))
                {
                    // Logger.LogInformation(deviceSetting.IP);
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
                    Console.WriteLine("!collector.IsConnected===" + !collector.IsConnected);
                    if (!collector.IsConnected)
                    {
                        if (collector.Connect(deviceSetting.IP, deviceSetting.Port,deviceSetting.Description))
                        {
                            Logger.LogInformation("{name}-{ip}:{port}连接成功", deviceSetting.Name, deviceSetting.IP, deviceSetting.Port);
                        }
                        else
                        {
                            Logger.LogError("{name}-{ip}:{port}无法连接，中断此次任务", deviceSetting.Name, deviceSetting.IP, deviceSetting.Port);
                            return;
                        }
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
                    // 通过传入的<TCollector, TDto>，绑定到TCollector的SetDataTo，传入model属性
                    collector.SetDataTo(model);
                    Console.WriteLine(" model.IP==========" + model.IP);

                    // 根据model类型，post数据到web后台
                    switch (model)
                    {
                        case CNCDto cnc:
                            Console.WriteLine("RunState----------------" + cnc.RunState);
                            /*Console.WriteLine("State----------------" + cnc.State);
                            Console.WriteLine("WorkMode----------------" + cnc.WorkMode);
                            Console.WriteLine("Emergency----------------" + cnc.Emergency);
                            Console.WriteLine("Alarm----------------" + cnc.Alarm);
                            Console.WriteLine("CurrentProgramName----------------" + cnc.CurrentProgramName);
                            Console.WriteLine("CurrentProgramNumber----------------" + cnc.CurrentProgramNumber);
                            Console.WriteLine("CurrentProgramLineNumber----------------" + cnc.CurrentProgramLineNumber);
                            Console.WriteLine("CurrentProgramContent----------------" + cnc.CurrentProgramContent);
                            Console.WriteLine("CurrentCutterNumber----------------" + cnc.CurrentCutterNumber);
                            Console.WriteLine("FeedSpeed----------------" + cnc.FeedSpeed);
                            Console.WriteLine("FeedSpeedUnit----------------" + cnc.FeedSpeedUnit);
                            Console.WriteLine("FeedOverride----------------" + cnc.FeedOverride);
                            Console.WriteLine("SpindleSpeed----------------" + cnc.SpindleSpeed);
                            Console.WriteLine("SpindleSpeedUnit----------------" + cnc.SpindleSpeedUnit);
                            Console.WriteLine("SpindleOverride----------------" + cnc.SpindleOverride);
                            Console.WriteLine("SpindleLoad----------------" + cnc.SpindleLoad);
                            Console.WriteLine("RapidOverride----------------" + cnc.RapidOverride);
                            Console.WriteLine("SystemTime----------------" + cnc.SystemTime);
                            Console.WriteLine("PartsCount----------------" + cnc.PartsCount);
                            Console.WriteLine("PartsTotal----------------" + cnc.PartsTotal);
                            Console.WriteLine("PartsRequired----------------" + cnc.PartsRequired);
                            Console.WriteLine("TimePowerOn----------------" + cnc.TimePowerOn);
                            Console.WriteLine("TimeOperating----------------" + cnc.TimeOperating);
                            Console.WriteLine("TimeCutting----------------" + cnc.TimeCutting);
                            Console.WriteLine("TimeCycle----------------" + cnc.TimeCycle);
                            Console.WriteLine("TimeCycle----------------" + cnc.TimeCycle);
                            Console.WriteLine("cnc.Axes----------------" + cnc.Axes[0].Name);
                            Console.WriteLine("cnc.Axes.Load.x-============" + cnc.Axes[0].Load);
                            Console.WriteLine("cnc.Axes.Load.y-============" + cnc.Axes[1].Load);
                            Console.WriteLine("cnc.Axes.Load.z-============" + cnc.Axes[2].Load);
                            Console.WriteLine("SpindleLoad=========" + cnc.SpindleLoad);
                            Console.WriteLine("cnc.Spindles[0].Speed=========" + cnc.Spindles[0].Speed);
                            Console.WriteLine("cnc.Spindles[0].Load=========" + cnc.Spindles[0].Load);*/
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
                        case TemperBoxDto temper:
                            if (await hnciGCApiTemperBox.PostTemperBox(temper) != null)
                            {
                                Console.WriteLine("RunState=======" + temper.RunState);
                                Console.WriteLine("State=======" + temper.State);
                                Console.WriteLine("PV_TMP=======" + temper.PV_TMP);
                                Console.WriteLine("PV_TMP=======" + temper.PV_HUM);
                                Console.WriteLine("SV_TMP=======" + temper.SV_TMP);
                                Console.WriteLine("SV_HUM=======" + temper.SV_HUM);
                                Console.WriteLine("Alarmstate=======" + temper.Alarmstate);
                                Console.WriteLine("AlarmData=======" + temper.AlarmData);

                                var jsonStr = JsonSerializer.Serialize(temper, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                    PropertyNamingPolicy = null,
                                    WriteIndented = true,
                                    MaxDepth = 64
                                });
                                Logger.LogDebug("----------{time}----------{newLine}{json}", DateTime.Now, Environment.NewLine, jsonStr);
                            }
                            break;
                        default:
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
