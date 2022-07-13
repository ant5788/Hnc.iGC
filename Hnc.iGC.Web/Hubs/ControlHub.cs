using Hnc.iGC.Web.Options;
using Hnc.NcLink;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

using System.Collections.Concurrent;

namespace Hnc.iGC.Web.Hubs
{
    public class ControlHub : Hub<IControlClient>
    {
        public NcLinkService2 NcLink { get; }
        public ILogger<ControlHub> Logger { get; }
        public MQTTOptions MQTTOptions { get; }

        // like "D0=10; D0.0=0; D0.1=1; D0.2=0; D0.3=1;"
        public static readonly ConcurrentDictionary<string, uint> RegisterValues = new();

        public ControlHub(NcLinkService2 ncLink, ILogger<ControlHub> logger, IOptions<MQTTOptions> options)
        {
            NcLink = ncLink;
            Logger = logger;
            MQTTOptions = options.Value;
        }

        public override async Task OnConnectedAsync()
        {
            foreach (var item in RegisterValues)
            {
                await Clients.Caller.RegisterValueChanged(MQTTOptions.RR_NcLink_Id, $"{item.Key}={item.Value}");
            }
        }

        public class KeyByteRequest
        {
            public byte[] Data => new byte[] { BYTE1, BYTE2 };

            public byte BYTE1 { get; set; }

            public byte BYTE2 { get; set; }
        }

        public async Task KeyByte(KeyByteRequest request)
        {
            await Clients.Caller.MessageInfo($"{DateTime.Now:HH:mm:ss.fff}, 后台收到面板请求, 按钮点击, [{Convert.ToHexString(request.Data)}].");
            Worker.SerialPortWorker.WriteData.Enqueue(request.Data);
        }

        public class SetRegisterRequest
        {
            public SetRegisterRequest(string deviceId, string setRegister)
            {
                DeviceId = deviceId;
                SetRegister = setRegister;
            }

            public string DeviceId { get; set; }

            public string SetRegister { get; set; }
        }

        public async Task<bool?> SetRegister(SetRegisterRequest request)
        {
            var t1 = DateTime.Now;
            await Clients.Caller.MessageInfo($"{t1:HH:mm:ss.fff}, 后台收到面板请求, [{request.SetRegister}]");
            if (string.IsNullOrWhiteSpace(request.DeviceId))
            {
                await Clients.Caller.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 面板请求参数错误, [{request.DeviceId}, 设备Id不能为空].");
                return null;
            }
            await NcLink.AddIfNotExistsAsync(request.DeviceId);
            var strs = request.SetRegister?.Split('.', '=') ?? Array.Empty<string>();
            switch (strs.Length)
            {
                default:
                    await Clients.Caller.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 面板请求参数错误, [{request.SetRegister}, 指令错误].");
                    return null;
                case 3:// "R100.1=1"
                    {
                        if (Enum.TryParse(strs[0][..1], out HncRegType regType)
                            && int.TryParse(strs[0][1..], out int index)
                            && int.TryParse(strs[1], out int offset)
                            && int.TryParse(strs[2], out int value))
                        {
                            var setString = $"{Enum.GetName(regType)}{index}.{offset}={value}";
                            var t2 = DateTime.Now;
                            //await Clients.Caller.MessageInfo($"后台解析请求耗时: {t2 - t1:s\\.fff}秒"); //0.000秒
                            await Clients.Caller.MessageInfo($"{t2:HH:mm:ss.fff}, 后台向CNC请求, [{setString}].");
                            var result = NcLink.SetRegisterOffset(request.DeviceId, regType, index, offset, value == 1);
                            var t3 = DateTime.Now;
                            await Clients.Caller.MessageInfo($"CNC处理请求耗时: {t3 - t2:s\\.fff}秒");
                            Logger.LogInformation("设置寄存器[{parameter}], CNC处理请求耗时{time}秒, CNC处理请求{result}.",
                                request.SetRegister,
                                (t3 - t2).ToString("s\\.fff"),
                                result.HasValue ? (result.Value ? "成功" : "失败") : "超时");
                            if (result.HasValue)
                            {
                                if (result.Value)
                                {
                                    await Clients.Caller.MessageSuccess($"{t3:HH:mm:ss.fff}, CNC处理请求成功, [{setString}].");
                                    return true;
                                }
                                else
                                {
                                    await Clients.Caller.MessageWarning($"{t3:HH:mm:ss.fff}, CNC处理请求失败, [{setString}].");
                                    return false;
                                }
                            }
                            else
                            {
                                await Clients.Caller.MessageError($"{t3:HH:mm:ss.fff}, CNC处理请求超时, [{setString}].");
                            }
                        }
                        return null;
                    }
                case 2:// "R101=7"
                    {
                        if (Enum.TryParse(strs[0][..1], out HncRegType regType)
                            && int.TryParse(strs[0][1..], out int index)
                            && int.TryParse(strs[1], out int value))
                        {
                            var setString = $"{Enum.GetName(regType)}{index}={value}";
                            var t2 = DateTime.Now;
                            //await Clients.Caller.MessageInfo($"后台解析请求耗时: {t2 - t1:s\\.fff}秒"); //0.000秒
                            await Clients.Caller.MessageInfo($"{t2:HH:mm:ss.fff}, 后台向CNC请求, [{setString}].");
                            var result = NcLink.SetRegister(request.DeviceId, regType, index, value);
                            var t3 = DateTime.Now;
                            await Clients.Caller.MessageInfo($"CNC处理请求耗时: {t3 - t2:s\\.fff}秒");
                            Logger.LogInformation("设置寄存器[{parameter}], CNC处理请求耗时{time}秒, CNC处理请求{result}.",
                                request.SetRegister,
                                (t3 - t2).ToString("s\\.fff"),
                                result.HasValue ? "成功" : "失败");
                            if (result.HasValue)
                            {
                                if (result.Value)
                                {
                                    await Clients.Caller.MessageSuccess($"{t3:HH:mm:ss.fff}, CNC处理请求成功, [{setString}].");
                                    return true;
                                }
                                else
                                {
                                    await Clients.Caller.MessageWarning($"{t3:HH:mm:ss.fff}, CNC处理请求失败, [{setString}].");
                                    return false;
                                }
                            }
                            else
                            {
                                await Clients.Caller.MessageError($"{t3:HH:mm:ss.fff}, CNC处理请求超时, [{setString}].");
                            }
                        }
                        return null;
                    }
            }
        }

        public class GetRegisterRequest
        {
            public GetRegisterRequest(string deviceId, string getRegister)
            {
                DeviceId = deviceId;
                GetRegister = getRegister;
            }

            public string DeviceId { get; set; }

            public string GetRegister { get; set; }
        }

        public async Task<uint?> GetRegister(GetRegisterRequest request)
        {
            var t1 = DateTime.Now;
            await Clients.Caller.MessageInfo($"{t1:HH:mm:ss.fff}, 后台收到面板请求, [{request.GetRegister}]");
            if (string.IsNullOrWhiteSpace(request.DeviceId))
            {
                await Clients.Caller.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 面板请求参数错误, [{request.DeviceId}, 设备Id不能为空].");
                return null;
            }
            await NcLink.AddIfNotExistsAsync(request.DeviceId);
            var strs = request.GetRegister?.Split('.') ?? Array.Empty<string>();
            switch (strs.Length)
            {
                default:
                    await Clients.Caller.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 面板请求参数错误, [{request.GetRegister}, 指令错误].");
                    return null;
                case 2:// "R100.1"
                    {
                        if (Enum.TryParse(strs[0][..1], out HncRegType regType)
                            && int.TryParse(strs[0][1..], out int index)
                            && int.TryParse(strs[1], out int offset))
                        {
                            var getString = $"{Enum.GetName(regType)}{index}.{offset}";
                            var t2 = DateTime.Now;
                            //await Clients.Caller.MessageInfo($"后台解析请求耗时: {t2 - t1:s\\.fff}秒"); //0.000秒
                            await Clients.Caller.MessageInfo($"{t2:HH:mm:ss.fff}, 后台向CNC请求, [{getString}].");
                            var result = NcLink.GetRegisterOffset(request.DeviceId, regType, index, offset);
                            var t3 = DateTime.Now;
                            await Clients.Caller.MessageInfo($"CNC处理请求耗时: {t3 - t2:s\\.fff}秒");
                            Logger.LogInformation("读取寄存器[{parameter}], CNC处理请求耗时{time}秒, CNC处理请求{result}.",
                                request.GetRegister,
                                (t3 - t2).ToString("s\\.fff"),
                                result.HasValue ? "成功" : "失败");
                            if (result.HasValue)
                            {
                                await Clients.Caller.MessageSuccess($"{t3:HH:mm:ss.fff}, CNC处理请求成功, [{getString}={(result.Value ? 1 : 0)}].");
                                return (uint)(result.Value ? 1 : 0);
                            }
                            else
                            {
                                await Clients.Caller.MessageError($"{t3:HH:mm:ss.fff}, CNC处理请求超时, [{getString}].");
                                return null;
                            }
                        }
                        return null;
                    }
                case 1:// "R101"
                    {
                        if (Enum.TryParse(strs[0][..1], out HncRegType regType)
                            && int.TryParse(strs[0][1..], out int index))
                        {
                            var getString = $"{Enum.GetName(regType)}{index}";
                            var t2 = DateTime.Now;
                            //await Clients.Caller.MessageInfo($"后台解析请求耗时: {t2 - t1:s\\.fff}秒"); //0.000秒
                            await Clients.Caller.MessageInfo($"{t2:HH:mm:ss.fff}, 后台向CNC请求, [{getString}].");
                            var result = NcLink.GetRegister(request.DeviceId, regType, index);
                            var t3 = DateTime.Now;
                            await Clients.Caller.MessageInfo($"CNC处理请求耗时: {t3 - t2:s\\.fff}秒");
                            Logger.LogInformation("读取寄存器[{parameter}], CNC处理请求耗时{time}秒, CNC处理请求{result}.",
                                request.GetRegister,
                                (t3 - t2).ToString("s\\.fff"),
                                result.HasValue ? "成功" : "失败");
                            if (result.HasValue)
                            {
                                await Clients.Caller.MessageSuccess($"{t3:HH:mm:ss.fff}, CNC处理请求成功, [{getString}={result.Value}].");
                                return result.Value;
                            }
                            else
                            {
                                await Clients.Caller.MessageError($"{t3:HH:mm:ss.fff}, CNC处理请求超时, [{getString}].");
                                return null;
                            }
                        }
                        return null;
                    }
            }
        }

        public async Task<uint?> GetFEEDOVERRIDE(string deviceId)
        {
            await NcLink.AddIfNotExistsAsync(deviceId);
            var result = NcLink.FEED_OVERRIDE(deviceId);
            if (result.HasValue)
            {
                await Clients.All.ShowFEEDOVERRIDE(deviceId, (int)result.Value);
                await Clients.Caller.MessageSuccess($"{DateTime.Now:HH:mm:ss.fff}, 进给倍率为[{result.Value}%].");
            }
            else
            {
                await Clients.Caller.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 进给倍率获取失败.");
            }
            return result;
        }

        public async Task<uint?> GetSPDLOVERRIDE(string deviceId)
        {
            await NcLink.AddIfNotExistsAsync(deviceId);
            var result = NcLink.SPDL_OVERRIDE(deviceId);
            if (result.HasValue)
            {
                await Clients.All.ShowSPDLOVERRIDE(deviceId, (int)result.Value);
                await Clients.Caller.MessageSuccess($"{DateTime.Now:HH:mm:ss.fff}, 主轴倍率为[{result.Value}%].");
            }
            else
            {
                await Clients.Caller.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 主轴倍率获取失败.");
            }
            return result;
        }

        public async Task<uint?> GetRAPIDOVERRIDE(string deviceId)
        {
            await NcLink.AddIfNotExistsAsync(deviceId);
            var result = NcLink.RAPID_OVERRIDE(deviceId);
            if (result.HasValue)
            {
                await Clients.All.ShowRAPIDOVERRIDE(deviceId, (int)result.Value);
                await Clients.Caller.MessageSuccess($"{DateTime.Now:HH:mm:ss.fff}, 快移倍率为[{result.Value}%].");
            }
            else
            {
                await Clients.Caller.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 快移倍率获取失败.");
            }
            return result;
        }

        public async Task<double?> GetMacroVar(string deviceId, int index)
        {
            var t1 = DateTime.Now;
            await Clients.Caller.MessageInfo($"{t1:HH:mm:ss.fff}, 后台收到面板请求, [Var{index}]");
            await NcLink.AddIfNotExistsAsync(deviceId);

            var t2 = DateTime.Now;
            //await Clients.Caller.MessageInfo($"后台解析请求耗时: {t2 - t1:s\\.fff}秒"); //0.000秒
            await Clients.Caller.MessageInfo($"{t2:HH:mm:ss.fff}, 后台向CNC请求, [Var{index}].");
            var result = NcLink.GetMacroVar(deviceId, index);
            var t3 = DateTime.Now;
            await Clients.Caller.MessageInfo($"CNC处理请求耗时: {t3 - t2:s\\.fff}秒");
            Logger.LogInformation("读取宏变量[{parameter}], CNC处理请求耗时{time}秒, CNC处理请求{result}.",
                $"[Var{index}]",
                (t3 - t2).ToString("s\\.fff"),
                result.HasValue ? "成功" : "失败");
            if (result.HasValue)
            {
                await Clients.Caller.MessageSuccess($"{t3:HH:mm:ss.fff}, CNC处理请求成功, [Var{index}={result.Value}].");
                await Clients.All.ShowMacroVar(deviceId, index, result.Value);
                return result.Value;
            }
            else
            {
                await Clients.Caller.MessageError($"{t3:HH:mm:ss.fff}, CNC处理请求超时或返回值为null, [Var{index}].");
                return null;
            }
        }

        public async Task<bool?> SetMacroVar(string deviceId, int index, double value)
        {
            var t1 = DateTime.Now;
            await Clients.Caller.MessageInfo($"{t1:HH:mm:ss.fff}, 后台收到面板请求, [Var{index}={value}]");
            await NcLink.AddIfNotExistsAsync(deviceId);

            var t2 = DateTime.Now;
            //await Clients.Caller.MessageInfo($"后台解析请求耗时: {t2 - t1:s\\.fff}秒"); //0.000秒
            await Clients.Caller.MessageInfo($"{t2:HH:mm:ss.fff}, 后台向CNC请求, [Var{index}={value}].");
            var result = NcLink.SetMacroVar(deviceId, index, value);
            var t3 = DateTime.Now;
            await Clients.Caller.MessageInfo($"CNC处理请求耗时: {t3 - t2:s\\.fff}秒");
            Logger.LogInformation("设置宏变量[{parameter}], CNC处理请求耗时{time}秒, CNC处理请求{result}.",
                $"[Var{index}={value}]",
                (t3 - t2).ToString("s\\.fff"),
                result.HasValue ? "成功" : "失败");
            if (result.HasValue)
            {
                await Clients.Caller.MessageSuccess($"{t3:HH:mm:ss.fff}, CNC处理请求成功, [Var{index}={value}].");
                await Clients.All.ShowMacroVar(deviceId, index, value);
                return result.Value;
            }
            else
            {
                await Clients.Caller.MessageError($"{t3:HH:mm:ss.fff}, CNC处理请求超时, [Var{index}={value}].");
                return null;
            }
        }
    }
}
