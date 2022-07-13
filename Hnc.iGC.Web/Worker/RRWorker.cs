using Hnc.iGC.Web.Controllers;
using Hnc.iGC.Web.Hubs;
using Hnc.iGC.Web.Models;
using Hnc.iGC.Web.Options;
using Hnc.NcLink;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

using System.Text.Json;

using static Hnc.iGC.Web.Hubs.ControlHub;

namespace Hnc.iGC.Web.Worker
{
    public class RRWorker : BackgroundService
    {
        public RRWorker(ILogger<RRWorker> logger,
            IOptions<MQTTOptions> options,
            IHubContext<ControlHub, IControlClient> hubContext,
            NcLinkService2 ncLink,
            RRController rrController)
        {
            Logger = logger;
            MQTTOptions = options.Value;
            HubContext = hubContext;
            NcLink = ncLink;
            RRController = rrController;
        }

        public ILogger<RRWorker> Logger { get; }
        public MQTTOptions MQTTOptions { get; }
        public IHubContext<ControlHub, IControlClient> HubContext { get; }
        public NcLinkService2 NcLink { get; }
        public RRController RRController { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Random random = new();
            await NcLink.AddIfNotExistsAsync(MQTTOptions.RR_NcLink_Id);
            List<RRLayoutItem> layoutItems = JsonSerializer.Deserialize<List<RRLayoutItem>>(await RRController.GetLayout()) ?? new();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (MQTTOptions.EnableMock)
                {
                    string[] status = { "AUTO", "MDI", "MPG", "INC", "JOG", "ZRN", "DNC", "EDIT" };
                    await HubContext.Clients.All.ShowSTATUS(MQTTOptions.RR_NcLink_Id, status[random.Next(0, 8)]);
                    await HubContext.Clients.All.ShowFEEDSPEED(MQTTOptions.RR_NcLink_Id, random.NextDouble() * 1000);

                    await HubContext.Clients.All.ShowFEEDOVERRIDE(MQTTOptions.RR_NcLink_Id, random.Next(0, 16) * 10);
                    await HubContext.Clients.All.ShowSPDLOVERRIDE(MQTTOptions.RR_NcLink_Id, random.Next(5, 13) * 10);
                    await HubContext.Clients.All.ShowRAPIDOVERRIDE(MQTTOptions.RR_NcLink_Id, random.Next(0, 5) * 25);

                    await HubContext.Clients.All.ShowPARTCOUNT(MQTTOptions.RR_NcLink_Id, random.Next(0, 100));
                    await HubContext.Clients.All.ShowMAGTOOLNO(MQTTOptions.RR_NcLink_Id, random.Next(0, 20));
                    await HubContext.Clients.All.ShowSPDTOOLNO(MQTTOptions.RR_NcLink_Id, random.Next(0, 20));

                    var axis0 = new AxisLinear { NAME = "X", NUMBER = 0 };
                    axis0.SERVO_DRIVER.POSITION = random.Next(0, 1000);
                    axis0.SERVO_DRIVER.SPEED = random.Next(0, 1000);
                    axis0.MOTOR.CURRENT = random.Next(0, 1000);
                    axis0.SCREW.POSITION = random.Next(0, 1000);
                    axis0.SCREW.SPEED = random.Next(0, 1000);
                    var axis1 = new AxisLinear { NAME = "Y", NUMBER = 1 };
                    axis1.SERVO_DRIVER.POSITION = random.Next(0, 1000);
                    axis1.SERVO_DRIVER.SPEED = random.Next(0, 1000);
                    axis1.MOTOR.CURRENT = random.Next(0, 1000);
                    axis1.SCREW.POSITION = random.Next(0, 1000);
                    axis1.SCREW.SPEED = random.Next(0, 1000);
                    var axis2 = new AxisLinear { NAME = "Z", NUMBER = 2 };
                    axis2.SERVO_DRIVER.POSITION = random.Next(0, 1000);
                    axis2.SERVO_DRIVER.SPEED = random.Next(0, 1000);
                    axis2.MOTOR.CURRENT = random.Next(0, 1000);
                    axis2.SCREW.POSITION = random.Next(0, 1000);
                    axis2.SCREW.SPEED = random.Next(0, 1000);
                    var axis5 = new AxisRotary { NAME = "C", NUMBER = 5 };
                    axis5.SERVO_DRIVER.POSITION = random.Next(0, 1000);
                    axis5.SERVO_DRIVER.SPEED = random.Next(0, 1000);
                    axis5.MOTOR.CURRENT = random.Next(0, 1000);
                    axis5.MOTOR.POSITION = random.Next(0, 1000);
                    axis5.MOTOR.SPEED = random.Next(0, 1000);
                    var axisValue = $"[{axis0},{axis1},{axis2},{axis5}]";
                    await HubContext.Clients.All.ShowAXIS(MQTTOptions.RR_NcLink_Id, axisValue);
                    await HubContext.Clients.All.ShowMacroVar(MQTTOptions.RR_NcLink_Id, random.Next(0, 99999), random.NextDouble() * 100);
                    await HubContext.Clients.All.ShowWARNING(MQTTOptions.RR_NcLink_Id,
                        "[{\"number\":\"800000004\",\"text\":\"上次断电数据保存失效,请检查UPS电源" + random.Next(0, 10) + "\"");

                    foreach (var item in MQTTOptions.Register.Split(';'))
                    {
                        var strs = item.Split('_');
                        if (Enum.TryParse(strs[0], out HncRegType regType)
                            && int.TryParse(strs[1], out int offset)
                            && int.TryParse(strs[2], out int indexMin)
                            && int.TryParse(strs[3], out int indexMax))
                        {
                            for (int i = indexMin; i <= indexMax; i++)
                            {
                                var value = random.Next(0, int.MaxValue);
                                await HubContext.Clients.All.RegisterValueChanged(MQTTOptions.RR_NcLink_Id, $"{$"{strs[0]}{i}"}={value}");
                                for (int j = 0; j < offset; j++)
                                {
                                    var register = $"{strs[0]}{i}.{j}={(value >> j) & 1}";
                                    if (layoutItems.Any(p => p.LightOn == register || p.LightOff == register))
                                    {
                                        await HubContext.Clients.All.RegisterValueChanged(MQTTOptions.RR_NcLink_Id, register);
                                        await Task.Delay(100, stoppingToken);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in MQTTOptions.Register.Split(';'))
                    {
                        var strs = item.Split('_');
                        if (Enum.TryParse(strs[0], out HncRegType regType)
                            && int.TryParse(strs[1], out int offset)
                            && int.TryParse(strs[2], out int indexMin)
                            && int.TryParse(strs[3], out int indexMax))
                        {
                            IReadOnlyList<uint>? data = NcLink.GetRegisters(MQTTOptions.RR_NcLink_Id, regType, indexMin, indexMax);
                            if (data == null) continue;
                            for (int i = 0; i < data.Count; i++)
                            {
                                // like "D0=10";
                                uint value = data[i];
                                var key = $"{strs[0]}{indexMin + i}";
                                RegisterValues.AddOrUpdate(key,
                                    k =>
                                    {
                                        HubContext.Clients.All.RegisterValueChanged(MQTTOptions.RR_NcLink_Id, $"{key}={value}");
                                        return value;
                                    },
                                    (k, v) =>
                                    {
                                        if (v != value)
                                        {
                                            HubContext.Clients.All.RegisterValueChanged(MQTTOptions.RR_NcLink_Id, $"{key}={value}");
                                        }
                                        return value;
                                    });
                                for (int j = 0; j < offset; j++)
                                {
                                    // like "D0.0=0";
                                    uint offsetValue = (value >> j) & 1;
                                    var offsetKey = $"{strs[0]}{indexMin + i}.{j}";
                                    RegisterValues.AddOrUpdate(offsetKey,
                                        k =>
                                        {
                                            HubContext.Clients.All.RegisterValueChanged(MQTTOptions.RR_NcLink_Id, $"{offsetKey}={offsetValue}");
                                            return offsetValue;
                                        },
                                        (k, v) =>
                                        {
                                            if (v != offsetValue)
                                            {
                                                HubContext.Clients.All.RegisterValueChanged(MQTTOptions.RR_NcLink_Id, $"{offsetKey}={offsetValue}");
                                            }
                                            return offsetValue;
                                        });
                                }
                            }
                        }
                    }
                    //await HubContext.Clients.All.ShowFEEDOVERRIDE(MQTTOptions.RR_NcLink_Id, NcLink.FEED_OVERRIDE(MQTTOptions.RR_NcLink_Id) ?? 100);
                    //await HubContext.Clients.All.ShowSPDLOVERRIDE(MQTTOptions.RR_NcLink_Id, NcLink.SPDL_OVERRIDE(MQTTOptions.RR_NcLink_Id) ?? 100);

                    await HubContext.Clients.All.ShowAXIS(MQTTOptions.RR_NcLink_Id, NcLink.Axis(MQTTOptions.RR_NcLink_Id));
                    await HubContext.Clients.All.ShowWARNING(MQTTOptions.RR_NcLink_Id, NcLink.Warning(MQTTOptions.RR_NcLink_Id)??"[]");
                }

                await Task.Delay(100, stoppingToken);
            }
        }
    }

}
