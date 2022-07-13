using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Hnc.iGC
{
    public class DongleWatcher : BackgroundService
    {
        public DongleWatcher(IHostApplicationLifetime hostApplicationLifetime, IDongleValidator dongle)
        {
            HostApplicationLifetime = hostApplicationLifetime;
            Dongle = dongle;
        }

        public IHostApplicationLifetime HostApplicationLifetime { get; }
        public IDongleValidator Dongle { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Dongle.Validate();
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
#if DEBUG1
            Console.WriteLine("加密狗检测失败，程序发布后此处应会退出程序，目前处于调试状态，不退出程序。");
#else
            Console.WriteLine("加密狗检测失败，退出程序");
            HostApplicationLifetime.StopApplication();
#endif
        }
    }
}
