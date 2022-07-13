using FluentFTP;

using Hnc.iGC.Models;
using Hnc.NcLink;

using Microsoft.EntityFrameworkCore;

namespace Hnc.iGC.Web.Worker
{
    public class CNCStatisticWorker : BackgroundService
    {
        public CNCStatisticWorker(ILogger<CNCStatisticWorker> logger,
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            NcLinkService2 ncLink,
            IWebHostEnvironment environment)
        {
            Logger = logger;
            DbContextFactory = dbContextFactory;
            NcLink = ncLink;
            Environment = environment;
        }

        public ILogger<CNCStatisticWorker> Logger { get; }
        public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; }
        public NcLinkService2 NcLink { get; }
        public IWebHostEnvironment Environment { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var dbContext = DbContextFactory.CreateDbContext();

                var cncs = await dbContext.CNCs.Select(s => s.DeviceId).Distinct().Select(deviceId =>
                     dbContext.CNCs.Where(p => p.DeviceId == deviceId).OrderByDescending(p => p.CreationTime).First())
                    .OrderBy(o => o.Name).AsNoTracking().ToListAsync(stoppingToken);

                foreach (var cncDest in cncs)
                {
                    //await ncLink.AddMachineAsync(cncDest.DeviceId);
                    await NcLink.AddIfNotExistsAsync("1D87AFA4597089E");

                    var random = new Random();

                    var item = new CNC_Statistic();
                    for (int i = 0; i < 1440; i++)
                    {
                        item.StateToday.Add((TimeSpan.FromMinutes(1), (CNC_Statistic.States)random.Next(0, 5)));
                    }
                    //item.StateToday.Add((TimeSpan.FromMinutes(100), CNC_Statistic.States.开机));
                    //item.StateToday.Add((TimeSpan.FromMinutes(150), CNC_Statistic.States.加工));
                    //item.StateToday.Add((TimeSpan.FromMinutes(288), CNC_Statistic.States.待机));
                    //item.StateToday.Add((TimeSpan.FromMinutes(426), CNC_Statistic.States.离线));
                    //item.StateToday.Add((TimeSpan.FromMinutes(476), CNC_Statistic.States.报警));

                    foreach (CNC_Statistic.States state in Enum.GetValues<CNC_Statistic.States>())
                    {
                        var minutes = item.StateToday.Sum((t) => t.state == state ? t.time.TotalMinutes : 0);
                        item.LastWeekPie[state] = item.LastWeekPie[state].Add(TimeSpan.FromMinutes(minutes) * 7);
                    }
                    for (DateTime date = DateTime.Today.AddDays(-7); date < DateTime.Today; date = date.AddDays(1))
                    {
                        item.LastWeekBar.Add((date, (uint)(date.DayOfYear * ((int)date.DayOfWeek + random.Next(5, 50)))));
                    }
                    item.ProductCountToday = (uint)random.Next(9999, 9999999);
                    var tempFileName = Path.GetTempFileName();
                    await System.IO.File.WriteAllBytesAsync(tempFileName, item.Get_iGSData_Bytes(), stoppingToken);

                    if (NcLink.GetRegister("1D87AFA4597089E", HncRegType.R, 1940) != 0)
                    {
                        continue;
                    }

                    try
                    {
                        //FtpClient client = new(cncDest.IP, 10021, "root", "111111");
                        FtpClient client = new("192.168.30.30", 10021, "root", "111111");

                        await client.ConnectAsync(stoppingToken);
                        await client.UploadFileAsync(tempFileName, "/h/lnc8/tmp/iGS/iGSData.SV", createRemoteDir: true, token: stoppingToken);
                        if (true || !await client.FileExistsAsync("/h/lnc8/tmp/iGS/macInfo.SV", stoppingToken))
                        {
                            var tempFileName2 = Path.GetTempFileName();
                            await System.IO.File.WriteAllBytesAsync(
                                tempFileName2,
                                CNC_Statistic.Get_macInfo_Bytes(cncDest.DeviceId, cncDest.Description, cncDest.Name),
                                stoppingToken);
                            await client.UploadFileAsync(tempFileName2, "/h/lnc8/tmp/iGS/macInfo.SV", createRemoteDir: true, token: stoppingToken);
                        }
                        if (true || !await client.FileExistsAsync("/h/lnc8/tmp/iGS/mac.png", stoppingToken))
                        {
                            var macFilePath = Environment.WebRootFileProvider.GetFileInfo("images/mac.png").PhysicalPath;
                            if (DateTime.Now.Second % 2 == 0)
                            {
                                macFilePath = Environment.WebRootFileProvider.GetFileInfo("images/广告位.png").PhysicalPath;
                            }
                            await client.UploadFileAsync(macFilePath, "/h/lnc8/tmp/iGS/mac.png", createRemoteDir: true, token: stoppingToken);
                        }
                        await client.DisconnectAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "FTP操作失败！|设备名称：{name}|设备IP：{ip}", cncDest.Name, cncDest.IP);
                    }

                    if (NcLink.SetRegister("1D87AFA4597089E", HncRegType.R, new List<(int, int)> { (1940, 1) }) != true)
                    {
                        Logger.LogWarning("设置失败");
                    }
                    break;

                }
            }
            await Task.Delay(2000, stoppingToken);
        }

    }
}
