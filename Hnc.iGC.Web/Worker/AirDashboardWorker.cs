using Hnc.iGC.Models;
using Hnc.NcLink;

namespace Hnc.iGC.Web.Worker
{
    public class AirDashboardWorker : BackgroundService
    {
        public AirDashboardWorker(ILogger<AirDashboardWorker> logger, NcLinkService2 ncLink)
        {
            Logger = logger;
            NcLink = ncLink;
        }

        public ILogger<AirDashboardWorker> Logger { get; }
        public NcLinkService2 NcLink { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.CompletedTask;
            try
            {
                var cncs = new List<AirDashboard_Cnc>
                    {
                        new AirDashboard_Cnc{NcLinkName="ShanDongWeiDa-fanuc-002", System_Model= AirDashboard_Cnc.SystemModel.fanuc, Machine_Name= "OP02"},
                        new AirDashboard_Cnc{NcLinkName="ShanDongWeiDa-fanuc-002", System_Model= AirDashboard_Cnc.SystemModel.fanuc, Machine_Name= "OP03"},
                        new AirDashboard_Cnc{NcLinkName="ShanDongWeiDa-fanuc-002", System_Model= AirDashboard_Cnc.SystemModel.fanuc, Machine_Name= "OP04"},
                    };
                var index = 0;
                var WorkTime = 120;
                var random = new Random();
                foreach (var cnc in cncs)
                {
                    await NcLink.AddIfNotExistsAsync(cnc.NcLinkName);
                }
                while (true)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    index++;
                    var flag = index / WorkTime > 0;
                    foreach (var cnc in cncs)
                    {
                        cnc.Work_Mode = (AirDashboard_Cnc.WorkMode)random.Next(2);
                        cnc.Work_Status = (AirDashboard_Cnc.WorkStatus)random.Next(4);
                        cnc.FEED_OVERRIDE = AirDashboard_Cnc.FEED_OVERRIDE_List[random.Next(AirDashboard_Cnc.FEED_OVERRIDE_List.Count)];
                        cnc.SPINDLE_OVERRIDE = AirDashboard_Cnc.SPINDLE_OVERRIDE_List[random.Next(AirDashboard_Cnc.SPINDLE_OVERRIDE_List.Count)];
                        if (flag)
                        {
                            cnc.Process_Count++;
                        }
                        cnc.Progress = (byte)(index * 100.0 / WorkTime);
                        cnc.Remaining_Hour = 0;
                        cnc.Remaining_Minute = (byte)((WorkTime - index) / 60);
                        cnc.Remaining_Second = (byte)((WorkTime - index) % 60);

                        cnc.Work_Status = (AirDashboard_Cnc.WorkStatus)(int.TryParse(NcLink.Query(cnc.NcLinkName, "010102")?.ToString(), out var ws) ? ws : 0);

                        cnc.ProgramName = NcLink.Query(cnc.NcLinkName, "01012020")?.ToString() ?? "";

                        cnc.ProgramLineNumber = ushort.TryParse(NcLink.Query(cnc.NcLinkName, "01012022")?.ToString(), out ushort pln) ? pln : (ushort)0;

                        cnc.Current = float.TryParse(NcLink.Query(cnc.NcLinkName, "0101100503")?.ToString(), out float cu) ? cu : (float)0;

                        cnc.Process_Count = ushort.TryParse(NcLink.Query(cnc.NcLinkName, "010106")?.ToString(), out var pc) ? pc : (ushort)0;
                        ;
                        //cnc.AlarmNumber = (ushort)(((JsonElement)service2.GetResponseValue(cnc.NcLinkName, "01012023"))[0]["number"]);
                    }
                    index %= WorkTime;
                    //await Task.Delay(500, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{name}抛出异常，任务已取消", nameof(AirDashboardWorker));
            }
        }
    }
}
