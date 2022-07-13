using Hnc.iGC.Models;
using Hnc.iGC.Web.Hubs;
using Hnc.iGC.Web.Options;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System.Collections.Concurrent;
using System.IO.Ports;

namespace Hnc.iGC.Web.Worker
{
    public class SerialPortWorker : BackgroundService
    {
        public SerialPortWorker(ILogger<CNCProgWorker> logger,
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IOptions<SerialPortOptions> options,
            IHostApplicationLifetime hostApplicationLifetime,
            IHubContext<ControlHub, IControlClient> hubContext)
        {
            Logger = logger;
            DbContextFactory = dbContextFactory;
            SerialPortOptions = options.Value;
            HostApplicationLifetime = hostApplicationLifetime;
            HubContext = hubContext;
        }

        public static readonly ConcurrentQueue<byte[]> WriteData = new();
        public static readonly ConcurrentQueue<byte[]> ReadData = new();

        public ILogger<CNCProgWorker> Logger { get; }
        public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; }
        public IHostApplicationLifetime HostApplicationLifetime { get; }
        public IHubContext<ControlHub, IControlClient> HubContext { get; }
        public SerialPortOptions SerialPortOptions { get; }
        public SerialPort? SerialPort { get; set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                Logger.LogError("本机未发现可用的串口，请检查！");
#if DEBUG1
                HostApplicationLifetime.StopApplication();
#endif
            }
            else if (!ports.Contains(SerialPortOptions.PortName))
            {
                Logger.LogError("本机可用的串口有[{PortName}]，请检查配置文件是否正确！", string.Join(",", ports));
#if DEBUG1
                HostApplicationLifetime.StopApplication();
#endif
            }
            else
            {
                SerialPort = new SerialPort(SerialPortOptions.PortName, SerialPortOptions.BaudRate, SerialPortOptions.Parity, SerialPortOptions.DataBits, SerialPortOptions.StopBits)
                {
                    Handshake = SerialPortOptions.Handshake,
                    ReadTimeout = SerialPortOptions.ReadTimeout,
                    WriteTimeout = SerialPortOptions.WriteTimeout
                };
                SerialPort.DataReceived += SerialPort_DataReceived;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (SerialPort?.IsOpen == false)
                    {
                        SerialPort?.Open();
                    }
                    if (WriteData.TryDequeue(out byte[]? data))
                    {
                        try
                        {
                            SerialPort?.Write(data, 0, data.Length);
                            await HubContext.Clients.All.MessageSuccess($"{DateTime.Now:HH:mm:ss.fff}, 发送成功, [{Convert.ToHexString(data)}].");
                            Logger.LogInformation("串口{port}写入数据{data}成功", SerialPortOptions.PortName, Convert.ToHexString(data));
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e, "串口{port}写入数据{data}失败", SerialPortOptions.PortName, Convert.ToHexString(data));
                            await HubContext.Clients.All.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 串口{SerialPortOptions.PortName}写入数据{Convert.ToHexString(data)}失败.");
                        }
                    }
                    await Task.Delay(100, stoppingToken);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "串口{port}操作异常", SerialPortOptions.PortName);
                    await HubContext.Clients.All.MessageError($"{DateTime.Now:HH:mm:ss.fff}, 串口{SerialPortOptions.PortName}操作异常.");
                }
            }

            SerialPort?.Close();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (SerialPort?.BytesToRead > 0)
            {
                byte[] data = new byte[SerialPort.BytesToRead];
                SerialPort.Read(data, 0, data.Length);
                ReadData.Enqueue(data);
            }
        }
    }
}
