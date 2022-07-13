using Hnc.iGC.Models;
using Hnc.NcLink;

using Microsoft.EntityFrameworkCore;

namespace Hnc.iGC.Web.Worker
{
    public class CNCNeighborWorker : BackgroundService
    {
        public CNCNeighborWorker(ILogger<CNCNeighborWorker> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory, NcLinkService2 ncLink)
        {
            Logger = logger;
            DbContextFactory = dbContextFactory;
            NcLink = ncLink;
        }

        public ILogger<CNCNeighborWorker> Logger { get; }
        public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; }
        public NcLinkService2 NcLink { get; }

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
                    //await NcLink.AddMachineAsync(cncDest.DeviceId);
                    await NcLink.AddIfNotExistsAsync("1D87AFA4597089E");

                    for (int i = 0; i < cncs.Count && i < 20; i++)
                    {
                        var cnc = cncs[i];
                        var item = new CNC_Neighbor
                        {
                            SystemModel = cnc.Description,
                            Name = cnc.Name,
                            WorkMode = Enum.TryParse(cnc.WorkMode, out CNC_Neighbor.WorkModes r1) ? r1 : CNC_Neighbor.WorkModes.自动,
                            RunState = Enum.TryParse(cnc.RunState, out CNC_Neighbor.RunStates r2) ? r2 : CNC_Neighbor.RunStates.循环启动,
                            FeedSpeed = (float)cnc.FeedSpeed,
                            SpindleSpeed = (float)cnc.SpindleSpeed,
                            ProgramName = cnc.CurrentProgramName,
                            PartsCount = (ushort)cnc.PartsCount,
                            TimeOperating = (uint)(cnc.TimeOperating?.TotalSeconds ?? 0),
                            Utilization = (byte)cnc.FeedOverride,
                        };
                        var data = item.GetDataBytes();

                        var values = data.Select((value, index) => (1000 + i * data.Length + index, (int)value)).ToList();

                        //if (!NcLink.SetRegister(cncDest.DeviceId, HncRegType.R, values))
                        //{
                        //    Logger.LogWarning("设置失败");
                        //}

                        //Debug
                        if (NcLink.SetRegister("1D87AFA4597089E", HncRegType.R, values) != true)
                        {
                            Logger.LogWarning("设置失败");
                        }
                        //Debug

                    }

                    if (cncs.Count < 20)
                    {
                        var values2 = Enumerable.Range(1000 + (cncs.Count * 47), 20 - cncs.Count)
                            .Select(index => (index, 0)).ToList();
                        //if (!NcLink.SetRegister(cncDest.DeviceId, HncRegType.R, values2))
                        //{
                        //    Logger.LogWarning("设置失败");
                        //}
                        if (NcLink.SetRegister("1D87AFA4597089E", HncRegType.R, values2) != true)
                        {
                            Logger.LogWarning("设置失败");
                        }
                    }
                    break;
                }

                await Task.Delay(2000, stoppingToken);
            }
        }

        private Task SendNeighborData(IList<CNC> cncs) => Task.Run(async () =>
         {
             await Task.CompletedTask;
             //TODO:并发任务
         });

    }
}
