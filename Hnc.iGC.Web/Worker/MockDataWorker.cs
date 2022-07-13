using Hnc.iGC.Models;

using Microsoft.EntityFrameworkCore;

using System.Text;

namespace Hnc.iGC.Web.Worker
{
    public class MockDataWorker : BackgroundService
    {
        public MockDataWorker(ILogger<AirDashboardWorker> logger,
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            Logger = logger;
            DbContextFactory = dbContextFactory;
        }

        public ILogger<AirDashboardWorker> Logger { get; }
        public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; }

        private readonly Random random = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var dbContext = DbContextFactory.CreateDbContext();
            var cncs = await InitAirDashboardDbAsync(dbContext, stoppingToken);

            var index = 0;
            var WorkTime = 120;
            while (!stoppingToken.IsCancellationRequested)
            {
                index++;
                var flag = index / WorkTime > 0;
                foreach (var cnc in cncs)
                {
                    //cnc.Work_Mode = (CncData.WorkMode)random.Next(2);
                    //cnc.Work_Status = (CncData.WorkStatus)random.Next(4);
                    //cnc.FEED_OVERRIDE = CncData.FEED_OVERRIDE_List[random.Next(CncData.FEED_OVERRIDE_List.Count)];
                    //cnc.SPINDLE_OVERRIDE = CncData.SPINDLE_OVERRIDE_List[random.Next(CncData.SPINDLE_OVERRIDE_List.Count)];

                    if (flag)
                    {
                        cnc.Process_Count++;
                        cnc.ProgramLineNumber = 1;
                    }
                    cnc.Progress = (byte)(index * 100.0 / WorkTime);
                    cnc.Remaining_Hour = 0;
                    cnc.Remaining_Minute = (byte)((WorkTime - index) / 60);
                    cnc.Remaining_Second = (byte)((WorkTime - index) % 60);

                    if (string.IsNullOrWhiteSpace(cnc.ProgramName))
                    {
                        cnc.ProgramName = $"O{random.Next(0, 9999):D4}";
                    }

                    cnc.ProgramLineNumber++;

                    cnc.Current = (float)(random.NextDouble() * 20);

                    cnc.LastModificationTime = DateTime.Now;
                }
                index %= WorkTime;
                await dbContext.SaveChangesAsync(stoppingToken);

                await Task.Delay(500, stoppingToken);
            }

        }

        private async Task<List<AirDashboard_Cnc>> InitAirDashboardDbAsync(ApplicationDbContext dbContext, CancellationToken stoppingToken)
        {
            if (!dbContext.AirDashboard_Cncs.Any())
            {
                await dbContext.AddRangeAsync(Enumerable.Range(1, 30).Select(index => new AirDashboard_Cnc
                {
                    NcLinkName = $"{index}",
                    System_Model = (AirDashboard_Cnc.SystemModel)random.Next(0, Enum.GetValues<AirDashboard_Cnc.SystemModel>().Length),
                    Machine_Name = $"OP{index:D2}"
                }).ToList(), stoppingToken);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            var cncs = await dbContext.AirDashboard_Cncs.ToListAsync(stoppingToken);

            static List<AppFile> GetTempFiles(string directoryName)
            {
                var files = new List<AppFile>();
                for (int i = 0; i < 10; i++)
                {
                    var filename = Path.GetRandomFileName();
                    var appFile = new AppFile()
                    {
                        Content = Encoding.UTF8.GetBytes(filename),
                        DirectoryName = Path.DirectorySeparatorChar + directoryName,
                        UntrustedName = filename,
                        Note = string.Empty,
                        Size = filename.Length,
                        UploadDT = DateTime.Now
                    };
                    files.Add(appFile);
                }
                return files;
            }
            foreach (var cnc in cncs)
            {
                cnc.GCode.AddRange(GetTempFiles(nameof(AirDashboard_Cnc.GCode)));
                await dbContext.SaveChangesAsync(stoppingToken);
            }

            return cncs;
        }
    }
}
