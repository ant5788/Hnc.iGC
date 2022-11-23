using AutoMapper;
using Hnc.iGC.Models;
using Hnc.iGC.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text;
using System.Text.Json;
// HSSF适用2007以前的版本,XSSF适用2007版本及其以上的。
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Collections;

namespace Hnc.iGC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CNCController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CNCController> logger;

        public StatusTotalDAL statusTotalDAL = new StatusTotalDAL();
        public CutterTotalDAL cutterTotalDAL = new CutterTotalDAL();
        public PartsTotalDAL partsTotalDAL = new PartsTotalDAL();
        public DeviceListDAL deviceListDAL = new DeviceListDAL();
        public DeviceDetailDAL deviceDetailDAL = new DeviceDetailDAL();
        public DeviceArchivesDAL archivesDAL = new DeviceArchivesDAL();
        public DeviceMaintainDAL maintainDAL = new DeviceMaintainDAL();
        public CheckPointDAL checkPointDAL = new CheckPointDAL();
        public PassRateDAL passRateDAL = new PassRateDAL();
        public OutsourcingDAL outsourcingDAL = new OutsourcingDAL();
        public DeviceRepairDAL deviceRepairDAL = new DeviceRepairDAL();
        public AlarmListDAL alarmListDAL = new AlarmListDAL();
        public ImagesMesDAL imagesMesDAL = new ImagesMesDAL();
        public EmployeeDAL employeeDAL = new EmployeeDAL();
        public MaintainRecordDAL maintainRecordDAL = new MaintainRecordDAL();
        public DictCodeDAL dictCodeDAL = new DictCodeDAL();
        public DictCodeGroupDAL dictCodeGroupDAL = new DictCodeGroupDAL();


        //全局机床状态
        private static string RunState = "9999";
        //全局刀号
        private static int CutterNumber = 9999;
        //全局镁工件数量（小时）
        private static int MagnesiomNumber = 0;
        //全局铝工件数量（小时）
        private static int AluminumNimber = 0;
        // 报警状态
        private static Boolean AlarmStatus = false;

        //急停状态
        private static Boolean emergencyStatus = false;

        // 图片选项
        //public PictureOptions _pictureOptions { get; }
        private readonly PictureOptions _pictureOptions;
        public CNCController(ApplicationDbContext context, IMapper mapper, ILogger<CNCController> logger, IOptions<PictureOptions> pictureOptions)
        {
            _context = context;
            _mapper = mapper;
            this.logger = logger;
            _pictureOptions = pictureOptions.Value;
        }


        /// <summary>
        /// 读取CNC数据
        /// </summary>
        /// <returns>设备Dto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CNCDto>>> GetCNCs()
        {
            try
            {
                var dData = await _context.CNCs.Where(p => p.CreationTime < DateTime.Now.AddHours(-1)).ToListAsync();
                _context.CNCs.RemoveRange(dData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除一小时前的CNC数据出错");
            }

            var ids = await _context.CNCs.Select(s => s.DeviceId).Distinct().ToListAsync();
            var entities = ids.Select(deviceId =>
                    _context.CNCs
                       .Include(p => p.AlarmMessages)
                       .Include(p => p.Spindles)
                       .Include(p => p.Axes)
                       .Include(p => p.Files)
                       .Include(p => p.CutterInfos)
                       .Where(p => p.DeviceId == deviceId)
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefault())
                    .OrderBy(o => o?.Name).ToList();
            List<CNCDto> listCNC = _mapper.Map<List<CNCDto>>(entities);
            return listCNC;
        }


        public async Task<List<CNCDto>> GetCNCs1()
        {
            try
            {
                var dData = await _context.CNCs.Where(p => p.CreationTime < DateTime.Now.AddHours(-1)).ToListAsync();
                _context.CNCs.RemoveRange(dData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除一小时前的CNC数据出错");
            }

            var ids = await _context.CNCs.Select(s => s.DeviceId).Distinct().ToListAsync();
            var entities = ids.Select(deviceId =>
                    _context.CNCs
                       .Include(p => p.AlarmMessages)
                       .Include(p => p.Spindles)
                       .Include(p => p.Axes)
                       .Include(p => p.Files)
                       .Include(p => p.CutterInfos)
                       .Where(p => p.DeviceId == deviceId)
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefault())
                    .OrderBy(o => o?.Name).ToList();
            List<CNCDto> listCNC = _mapper.Map<List<CNCDto>>(entities);
            return listCNC;
        }


        /// <summary>
        /// 读取CNC数据
        /// </summary>
        /// <param name="deviceId">设备Id</param>
        /// <returns>设备Dto</returns>
        [HttpGet("{deviceId}")]
        public async Task<ActionResult<CNCDto>> GetCNC(string deviceId)
        {
            var entity = await _context.CNCs
                       .Include(p => p.AlarmMessages)
                       .Include(p => p.Spindles)
                       .Include(p => p.Axes)
                       .Include(p => p.Files)
                       .Include(p => p.CutterInfos)
                       .Where(p => p.DeviceId == deviceId)
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefaultAsync();
            return _mapper.Map<CNCDto>(entity);
        }

        public async Task<CNCDto> GetCNC1(string deviceId)
        {
            var entity = await _context.CNCs
                       .Include(p => p.AlarmMessages)
                       .Include(p => p.Spindles)
                       .Include(p => p.Axes)
                       .Include(p => p.Files)
                       .Include(p => p.CutterInfos)
                       .Where(p => p.DeviceId == deviceId)
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefaultAsync();
            return _mapper.Map<CNCDto>(entity);
        }

        /// <summary>
        /// 创建或更新CNC数据（覆盖）
        /// </summary>
        /// <param name="dto">设备Dto</param>
        /// <returns>设备Dto</returns>
        [HttpPost]
        public async Task<ActionResult<CNCDto>> PostCNC(CNCDto dto)
        {
            var entity = await _context.CNCs
                       .Include(p => p.AlarmMessages)
                       .Include(p => p.Spindles)
                       .Include(p => p.Axes)
                       .Include(p => p.Files)
                       .Include(p => p.CutterInfos)
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefaultAsync(p => p.DeviceId == dto.DeviceId);
            entity = _mapper.Map<CNC>(dto);
            _context.CNCs.Add(entity);

            await _context.SaveChangesAsync();

            //处理统计机床状态方法统计
            SetStateProcess(dto);
            //处理刀号使用时长统计
            SetCuuterNumber(dto);
            //处理加工件数
            SetPartsTotal(dto);
            //全部删除所有的数据
            //deviceListDAL.DeleteAll();
            //处理保存设备列表信息
            if (deviceListDAL.getOneById(dto.DeviceId))
            {
                deviceListDAL.Add(deviceListDAL.SetDeviceList(dto));
                /* DeviceDetail deviceDetail = new DeviceDetail();
                 deviceDetail.DeviceId = dto.DeviceId;
                 deviceDetail.DeviceName = dto.Name;
                 deviceDetail.DeviceType = dto.Description;
                 deviceDetailDAL.Add(deviceDetailDAL.SetDeviceDetail(deviceDetail));*/
            }
            //处理保存报警信息
            SetAlarmMessage(dto);
            return CreatedAtAction(nameof(GetCNC), new { deviceId = dto.DeviceId }, dto);
        }

        /// <summary>
        /// 删除CNC
        /// </summary>
        /// <param name="deviceId">设备Id</param>
        /// <returns></returns>
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteCNC(string deviceId)
        {
            var entities = await _context.CNCs
                       .Include(p => p.AlarmMessages)
                       .Include(p => p.Spindles)
                       .Include(p => p.Axes)
                       .Include(p => p.Files)
                       .Include(p => p.CutterInfos)
                       .Where(p => p.DeviceId == deviceId)
                       .ToListAsync();

            if (entities == null)
            {
                return NotFound();
            }

            _context.CNCs.RemoveRange(entities);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// 设置保存报警信息
        /// </summary>
        /// <param name="dto"></param>
        private void SetAlarmMessage(CNCDto dto)
        {
            if (dto.Alarm == true && dto.AlarmMessages.Count > 0)
            {
                List<AlarmMessageDto> alarmList = (List<AlarmMessageDto>)dto.AlarmMessages;

                foreach (AlarmMessageDto alarmDto in alarmList)
                {
                    AlarmList alarm = alarmListDAL.SetAlarmList(dto);
                    alarm.AlarmMessage = alarmDto.Message;
                    alarm.AlarmNumber = alarmDto.Number;
                    alarm.StartAt = alarmDto.StartAt;
                    alarm.EndAt = alarmDto.EndAt;
                    alarmListDAL.Add(alarm);
                }
            }
        }
        private static int temperState = 9999;

        /// <summary>
        /// 设置状态数据
        /// </summary>
        /// <param name="dto"></param>
        private void SetStateProcess(CNCDto dto)
        {
            Console.WriteLine("temperState====" + temperState);
            Console.WriteLine("RunState====" + RunState);
            Console.WriteLine("dto.State======" + dto.State);

            StatusTotal status = statusTotalDAL.GetModelByParameters(dto.DeviceId);
            if (status == null)
            {
                //新增保存数据
                statusTotalDAL.Add(statusTotalDAL.SetStatusTotal(dto));
                RunState = dto.RunState;
                //temperState = dto.State;
            }
            //如果状态不为9999 查询通过查询runstate和当前结束时间和市场为空的数据。
            if (dto.State != status.DeviceStatus)
            {
                status.EndTime = DateTime.Now;
                status.Duration = GetHourDifference(status.StartTime, status.EndTime);
                statusTotalDAL.Update(status);
                //新增一条最新的状态数据
                statusTotalDAL.Add(statusTotalDAL.SetStatusTotal(dto));
            }

            //保存报警状态及时长
            if (dto.Alarm && dto.Alarm != AlarmStatus)
            {
                StatusTotal statusAlarm = statusTotalDAL.GetModelByParameters(dto.DeviceId, 99);
                if (null == statusAlarm)
                {
                    StatusTotal statusTotal = statusTotalDAL.SetStatusTotal(dto);
                    statusTotal.DeviceStatus = 99;
                    statusTotal.DeviceStatusName = "ALARM";
                    statusTotalDAL.Add(statusTotal);
                }
                else
                {
                    statusAlarm.EndTime = DateTime.Now;
                    statusAlarm.Duration = GetHourDifference(status.StartTime, status.EndTime);
                    statusTotalDAL.Update(statusAlarm);
                }
            }
            //保存急停状态机时长
            if (dto.Emergency && dto.Emergency != emergencyStatus)
            {
                StatusTotal statusEmergency = statusTotalDAL.GetModelByParameters(dto.DeviceId, 98);
                if (null == statusEmergency)
                {
                    StatusTotal statusTotal = statusTotalDAL.SetStatusTotal(dto);
                    statusTotal.DeviceStatus = 98;
                    statusTotal.DeviceStatusName = "EMERGENCY";
                    statusTotalDAL.Add(statusTotal);
                }
                else
                {
                    statusEmergency.EndTime = DateTime.Now;
                    statusEmergency.Duration = GetHourDifference(status.StartTime, status.EndTime);
                    statusTotalDAL.Update(statusEmergency);
                }
            }

        }




        /// <summary>
        /// 处理保存刀号使用时长方法
        /// </summary>
        /// <param name="dto"></param>
        private void SetCuuterNumber(CNCDto dto)
        {
            //TODO 找换刀的时候还要判断是否 机床状态是否在循环启动。
            //TODO 判断切换刀号了并且机床状态是循环启动状态。
            //TODO  当前刀号没有切换，但是机床状态是有其他状态切换为循环启动状态。
            //（机床状态保存全局变量，并且这次机床返回的数据机床状态是循环启动状态。和全局变量的机床状态不同。）
            CutterTotal cutterTotal = cutterTotalDAL.GetModelByParameters(dto.DeviceId);
            if (null == cutterTotal)
            {
                //新增保存数据
                cutterTotalDAL.Add(cutterTotalDAL.SetCutterTotal(dto));
                CutterNumber = dto.CurrentCutterNumber;
            }

            //如果刀号不是上次的刀号并且机床状态是循环启动状态
            if (dto.CurrentCutterNumber != cutterTotal.CutterNumber)
            {
                //修改上一个刀号的使用结束时间及使用市场
                cutterTotal.EndTime = DateTime.Now;
                cutterTotal.UseDuration = cutterTotal.UseDuration + GetHourDifference(cutterTotal.StartTime, cutterTotal.EndTime);
                cutterTotalDAL.Update(cutterTotal);
                //保存记录新的刀号开始使用时间
                cutterTotalDAL.Add(cutterTotalDAL.SetCutterTotal(dto));
                CutterNumber = dto.CurrentCutterNumber;
            }

        }


        /// <summary>
        /// 处理加工件数
        /// </summary>
        /// <param name="dto"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SetPartsTotal(CNCDto dto)
        {
            DateTime dt = DateTime.Now;
            string hoursStart = dt.ToString("yyy-MM-dd HH") + ":00:00";
            string hoursEnd = dt.ToString("yyy-MM-dd HH") + ":59:59";
            //当前小时的                                                                                                                                                     数据
            PartsTotal partsTotal = partsTotalDAL.GetModelByParameters(dto.DeviceId, hoursStart, hoursEnd);
            if (null == partsTotal)
            {
                PartsTotal parts = partsTotalDAL.SetPartsTotal(dto);
                //如果使用的加工镁的程序
                if (dto.CurrentProgramName == "")
                {
                    MagnesiomNumber++;
                    parts.MagnesiumTotal = MagnesiomNumber;
                }

                //如果使用的加工铝的程序
                if (dto.CurrentProgramName == "")
                {
                    AluminumNimber++;
                    parts.AluminumTotal = AluminumNimber;
                }
                partsTotalDAL.Add(parts);
            }
            else
            {
                //如果使用的加工镁的程序
                if (dto.CurrentProgramName == "")
                {
                    MagnesiomNumber++;
                    partsTotal.MagnesiumTotal = MagnesiomNumber;
                }
                //如果使用的加工铝的程序
                if (dto.CurrentProgramName == "")
                {
                    AluminumNimber++;
                    partsTotal.AluminumTotal = AluminumNimber;
                }
                partsTotalDAL.Update(partsTotal);
            }
        }






        /// <summary>
        /// 3.1机床状态占比统计
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("StateRatio")]
        public string StateRatio(string deviceId, string startTime, string endTime)
        {
            try
            {
                string start = startTime + " 00:00:00";
                string end = endTime + " 23:59:59";
                StringBuilder strSql = new StringBuilder();
                strSql.Append("create_time between '" + start + "' and  '" + end + "'");
                List<PartsTotal> partList = partsTotalDAL.GetList(strSql.ToString());
                List<StatusTotal> statusList = statusTotalDAL.GetList(strSql.ToString());
                //计算所有状态的总时间
                var DurationCount = 0.00;
                var RunSet = 0.00;
                var STOP = 0.00;
                var HOLD = 0.00;
                var START = 0.00;
                var MSTR = 0.00;
                var AlarmCount = 0;
                if (null != statusList)
                {
                    statusList.ForEach(status =>
                    {
                        DurationCount += status.Duration;
                        if (status.DeviceStatus == 0)
                        {
                            RunSet += status.Duration;
                        }
                        if (status.DeviceStatus == 1)
                        {
                            STOP += status.Duration;
                        }
                        if (status.DeviceStatus == 2)
                        {
                            HOLD += status.Duration;
                        }
                        if (status.DeviceStatus == 3)
                        {
                            START += status.Duration;
                        }
                        if (status.DeviceStatus == 4)
                        {
                            MSTR += status.Duration;
                        }
                        if (status.DeviceStatus == 99)
                        {
                            AlarmCount++;
                        }
                    });

                }
                Dictionary<string, double> hashMap = new Dictionary<string, double>();
                hashMap.Add("DurationCount", DurationCount);
                hashMap.Add("RunSet", RunSet);
                hashMap.Add("STOP", STOP);
                hashMap.Add("HOLD", HOLD);
                hashMap.Add("START", START);
                hashMap.Add("MSTR", MSTR);
                hashMap.Add("AlarmCount", AlarmCount);
                return ResultUtil.Success(hashMap, "查询机床状态占比成功");
            }
            catch (Exception ex)
            {
                return ResultUtil.Fail("查询机床状态占比异常：" + ex);
            }
        }

        /// <summary>
        /// 3.2当天每小时产量
        /// </summary>
        /// <returns></returns>
        [HttpGet("PartsDailyOutput")]
        public Task<string> PartsDailyOutput(string deviceId, string time)
        {
            try
            {
                List<Dictionary<string, object>> listMap = partsTotalDAL.PartsDailyOutput(deviceId, time);
                var text = JsonSerializer.Serialize(listMap, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true,
                });
                return Task.FromResult(text);
            }
            catch (Exception ex)
            {
                return Task.FromResult("当日产量统计异常：" + ex.ToString());

            }
        }

        /// <summary>
        /// 3.4 故障持续时间
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="time">某天</param>
        /// <returns></returns>
        [HttpGet("FaultDuration")]
        public string FaultDuration(string deviceId, string time)
        {
            try
            {
                List<Dictionary<string, object>> listMap = statusTotalDAL.FaultDuration(deviceId, time);
                return ResultUtil.Success(listMap, "故障持续时间查询成功");
            }
            catch (Exception ex)
            {
                return ResultUtil.Fail("故障持续时间查询异常" + ex.ToString());
            }
        }



        /// <summary>  TODO 需要重新编写逻辑   运行时间/开机时间 要的是 比例 每班每天的 给出时间段。
        /// 3.5 单台设备看板  （访问返回数据）
        /// 单台设备运行时间统计
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("RunTimeStatistics")]
        public string RunTimeStatistics(string deviceId, long startTime, long endTime)
        {
            try
            {   //0 复位  1 停止 
                List<Dictionary<string, object>> statusTotalList = statusTotalDAL.StatusTimeStatistics(deviceId, ToTime(startTime), ToTime(endTime));
                return ResultUtil.Success(statusTotalList, "单台设备运行时间统计查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError("单台设备运行时间统计查询异常:" + ex.ToString());
                return ResultUtil.Fail("单台设备运行时间统计查询异常:" + ex.ToString());
            }
        }

        /// <summary>
        /// 运行时间/开始时间  设备效率分析
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public string DeviceEfficiency(string deviceId, long startTime, long endTime)
        {
            try
            {
                List<StatusTotal> statusList = statusTotalDAL.EfficiencyAnalysisById(deviceId, ToTime(startTime), ToTime(endTime));
                decimal totalTime = 0.00m;
                decimal runTime = 0.00m;
                foreach (StatusTotal item in statusList)
                {
                    if (item.DeviceStatus == 3)
                    {
                        runTime += (decimal)item.Duration;
                    }
                    totalTime += (decimal)item.Duration;
                }

                decimal Ratio = Math.Round(decimal.Parse((runTime / totalTime).ToString("0.000")), 2) * 100;

                return ResultUtil.Success(Ratio, "设备使用效率查询成功");
            }
            catch (Exception ex)
            {
                return ResultUtil.Fail("设备使用效率查询失败：" + ex.Message);
            }

        }

        /// <summary>
        /// 加工材料区分，输出不同材质设备嫁动率
        /// </summary>
        /// <returns></returns>
        public string materialRationing(string devicdId)
        {
            try
            {
                //查询某个机器执行某个程序程序的总的时长
                List<StatusTotal> TotalDurationList = statusTotalDAL.TotalProgramDuration(devicdId);
                //查询某个机器执行某个程序循环启动状态的时长。
                List<StatusTotal> CycleStartDurtionList = statusTotalDAL.CycleStartDurtion(devicdId);
                foreach (StatusTotal total in TotalDurationList)
                {
                    foreach (StatusTotal cycle in TotalDurationList)
                    {
                        if (total.CurrentProgramNumber.Equals(cycle.CurrentProgramNumber))
                        {

                        }
                    }

                }

                return ResultUtil.Success(0, "材质嫁动率查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError("材质嫁动率查询异常:" + ex);
                return ResultUtil.Fail("材质嫁动率查询异常：" + ex.Message);
            }

        }


        /// <summary>
        /// 4.0所有设备设备效率分析 多台设备运行日志 已测试（访问返回数据）//TODO 方法重写
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("EfficiencyAnalysis")]
        public string EfficiencyAnalysis(long startTime, long endTime)
        {
            try
            {
                List<StatusTotal> mapList = statusTotalDAL.EfficiencyAnalysis(ToTime(startTime), ToTime(endTime));
                Dictionary<string, object> map = new Dictionary<string, object>();
                string name = "";
                if (null == mapList)
                {
                    return ResultUtil.Fail("没有查询到相关数据");
                }
                foreach (StatusTotal status in mapList)
                {
                    if (name == "" || name != status.DeviceName)
                    {
                        name = status.DeviceName;
                        map.Add(name, null);
                    }
                }
                Dictionary<string, object> resuleMap = new Dictionary<string, object>();
                List<List<StatusTotal>> statusTotals = new List<List<StatusTotal>>();
                List<string> listName = new List<string>();
                foreach (var m in map)
                {
                    List<StatusTotal> list = new List<StatusTotal>();
                    foreach (StatusTotal status in mapList)
                    {
                        if (m.Key.Equals(status.DeviceName))
                        {
                            list.Add(status);
                        }
                    }
                    resuleMap.Add(m.Key, list);
                    statusTotals.Add(list);
                }

                List<Dictionary<string, object>> listDictionary = new List<Dictionary<string, object>>();
                List<Dictionary<string, object>> timeReusle = new List<Dictionary<string, object>>();
                foreach (var item in resuleMap)
                {
                    List<StatusTotal> statusTotalList = (List<StatusTotal>)item.Value;
                    decimal ReSetDuration = 0.00m;
                    decimal StopDuration = 0.00m;
                    decimal HoldDuration = 0.00m;
                    decimal CycleStartDurtion = 0.00m;
                    decimal MstrDuration = 0.00m;
                    decimal ExceptionDuration = 0.00m;
                    decimal EmergencyStop = 0.00m;
                    decimal totalDuration = 0.00m;

                    if (null != statusTotalList && statusTotalList.Count > 0)
                    {
                        statusTotalList.ForEach(item =>
                        {
                            //复位状态
                            if (item.DeviceStatus == 0)
                            {
                                ReSetDuration += (decimal)item.Duration;
                            }
                            //停止状态
                            if (item.DeviceStatus == 1)
                            {
                                StopDuration += (decimal)item.Duration;
                            }
                            // 进给保持
                            if (item.DeviceStatus == 2)
                            {
                                HoldDuration += (decimal)item.Duration;
                            }
                            //循环启动
                            if (item.DeviceStatus == 3)
                            {
                                CycleStartDurtion += (decimal)item.Duration;
                            }

                            //
                            if (item.DeviceStatus == 4)
                            {
                                MstrDuration += (decimal)item.Duration;
                            }

                            //急停状态
                            if (item.DeviceStatus == 98)
                            {
                                EmergencyStop += (decimal)item.Duration;
                            }

                            //报警状态
                            if (item.DeviceStatus == 99)
                            {
                                ExceptionDuration += (decimal)item.Duration;
                            }
                            totalDuration += (decimal)item.Duration;
                        });

                        decimal runRatio = 0.00m;
                        if (CycleStartDurtion != 0)
                        {
                            //利用率
                            runRatio = Math.Round(decimal.Parse((CycleStartDurtion / totalDuration).ToString("0.000")), 2) * 100;
                        }
                        decimal ExceptionRatio = 0.00m;
                        if (ExceptionRatio != 0)
                        {
                            //報警率
                            ExceptionRatio = Math.Round(decimal.Parse((ExceptionDuration / totalDuration).ToString("0.000")), 2) * 100;
                        }
                        decimal stopRatio = 0.00m;
                        if (stopRatio != 0)
                        {
                            //停机率
                            stopRatio = Math.Round(decimal.Parse((StopDuration / totalDuration).ToString("0.000")), 2) * 100;
                        }
                        Dictionary<string, object> rationResult = new Dictionary<string, object>();
                        rationResult.Add("runRatio", runRatio);
                        rationResult.Add("ExceptionRatio", ExceptionRatio);
                        rationResult.Add("stopRatio", stopRatio);
                        listDictionary.Add(rationResult);
                        Dictionary<string, object> timeResult = new Dictionary<string, object>();
                        timeResult.Add("CycleStartDurtion", CycleStartDurtion);
                        timeResult.Add("ExceptionDuration", ExceptionDuration);
                        timeResult.Add("StopDuration", StopDuration);
                        timeReusle.Add(timeResult);
                    }
                }
                Dictionary<string, object> resule1 = new Dictionary<string, object>();
                resule1.Add("statusTotals", statusTotals);
                resule1.Add("ration", listDictionary);
                resule1.Add("timeReusle", timeReusle);
                return ResultUtil.Success(resule1, "查询所有设备某个时段全部状态成功");
            }
            catch (Exception ex)
            {
                logger.LogError("查询所有设备某个时段全部状态异常:" + ex);
                return ResultUtil.Fail("查询所有设备某个时段全部状态异常" + ex.ToString());
            }
        }



        /// <summary>
        /// 4.1 查询单个设备某个时间段的状态分布状况 单台设备运行日志 已测试
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("EfficiencyAnalysisById")]
        public string EfficiencyAnalysisById(string deviceId, long startTime, long endTime)
        {
            try
            {
                List<StatusTotal> statusTotalList = statusTotalDAL.EfficiencyAnalysisById(deviceId, ToTime(startTime), ToTime(endTime));
                decimal ReSetDuration = 0.00m;
                decimal StopDuration = 0.00m;
                decimal HoldDuration = 0.00m;
                decimal CycleStartDurtion = 0.00m;
                decimal MstrDuration = 0.00m;
                decimal ExceptionDuration = 0.00m;
                decimal EmergencyStop = 0.00m;
                decimal totalDuration = 0.00m;
                if (null != statusTotalList && statusTotalList.Count > 0)
                {
                    statusTotalList.ForEach(item =>
                    {
                        //复位状态
                        if (item.DeviceStatus == 0)
                        {
                            ReSetDuration += (decimal)item.Duration;
                        }
                        //停止状态
                        if (item.DeviceStatus == 1)
                        {
                            StopDuration += (decimal)item.Duration;
                        }
                        // 进给保持
                        if (item.DeviceStatus == 2)
                        {
                            HoldDuration += (decimal)item.Duration;
                        }
                        //循环启动
                        if (item.DeviceStatus == 3)
                        {
                            CycleStartDurtion += (decimal)item.Duration;
                        }

                        //
                        if (item.DeviceStatus == 4)
                        {
                            MstrDuration += (decimal)item.Duration;
                        }

                        //急停状态
                        if (item.DeviceStatus == 98)
                        {
                            EmergencyStop += (decimal)item.Duration;
                        }

                        //报警状态
                        if (item.DeviceStatus == 99)
                        {
                            ExceptionDuration += (decimal)item.Duration;
                        }

                        totalDuration += (decimal)item.Duration;
                    });

                    //用时：
                    Dictionary<string, object> TimeCost = new Dictionary<string, object>();
                    TimeCost.Add("CycleStartDurtion", CycleStartDurtion);
                    TimeCost.Add("ExceptionDuration", ExceptionDuration);
                    TimeCost.Add("StopDuration", StopDuration);

                    //利用率
                    var runRatio = Math.Round(decimal.Parse((CycleStartDurtion / totalDuration).ToString("0.000")), 2) * 100;
                    // 报警率
                    var ExceptionRatio = Math.Round(decimal.Parse((ExceptionDuration / totalDuration).ToString("0.000")), 2) * 100;
                    //停机率
                    var stopRatio = Math.Round(decimal.Parse((StopDuration / totalDuration).ToString("0.000")), 2) * 100;
                    Dictionary<string, object> percentage = new Dictionary<string, object>();
                    percentage.Add("runRatio", runRatio);
                    percentage.Add("ExceptionRatio", ExceptionRatio);
                    percentage.Add("stopRatio", stopRatio);

                    Dictionary<string, object> total = new Dictionary<string, object>();
                    total.Add("TimeCost", TimeCost);
                    total.Add("percentage", percentage);
                    total.Add("list", statusTotalList);
                    return ResultUtil.Success(total, "查询所有设备某个时段全部状态成功");
                }
                return ResultUtil.Success(null, "查询所有设备某个时段全部状态成功数据为空");
            }
            catch (Exception ex)
            {
                logger.LogError("设备ID:" + deviceId + "查询单个设备某个时段全部状态异常:" + ex);
                return ResultUtil.Fail("查询单个设备某个时段全部状态异常" + ex.ToString());
            }
        }

        /// <summary>
        /// 4.2 设备嫁动率分析
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("MarriageRate")]
        public string MarriageRate(string deviceId, long startTime, long endTime)
        {
            try
            {
                List<StatusTotal> totalList = statusTotalDAL.EfficiencyAnalysisById(deviceId, ToTime(startTime), ToTime(endTime));
                if (null == totalList)
                {
                    return ResultUtil.Success(null, "选择时间段运动率/嫁动率查询为空");
                }
                decimal ReSetDuration = 0.00m;
                decimal StopDuration = 0.00m;
                decimal HoldDuration = 0.00m;
                decimal CycleStartDurtion = 0.00m;
                decimal MstrDuration = 0.00m;
                decimal ExceptionDuration = 0.00m;
                decimal EmergencyStop = 0.00m;
                decimal totalDuration = 0.00m;
                totalList.ForEach(item =>
                {
                    //复位状态
                    if (item.DeviceStatus == 0)
                    {
                        ReSetDuration += (decimal)item.Duration;
                    }
                    //停止状态
                    if (item.DeviceStatus == 1)
                    {
                        StopDuration += (decimal)item.Duration;
                    }
                    // 进给保持
                    if (item.DeviceStatus == 2)
                    {
                        HoldDuration += (decimal)item.Duration;
                    }
                    //循环启动
                    if (item.DeviceStatus == 3)
                    {
                        CycleStartDurtion += (decimal)item.Duration;
                    }

                    //指令启动状态 
                    if (item.DeviceStatus == 4)
                    {
                        MstrDuration += (decimal)item.Duration;
                    }

                    //急停状态
                    if (item.DeviceStatus == 98)
                    {
                        EmergencyStop += (decimal)item.Duration;
                    }

                    //报警状态
                    if (item.DeviceStatus == 99)
                    {
                        ExceptionDuration += (decimal)item.Duration;
                    }

                    totalDuration += (decimal)item.Duration;
                });

                var count = ReSetDuration + StopDuration + HoldDuration + ExceptionDuration + CycleStartDurtion;

                var ExerciseRate = Math.Round(decimal.Parse((count / totalDuration).ToString("0.000")), 2) * 100;
                var MarriageRate = Math.Round(decimal.Parse((CycleStartDurtion / totalDuration).ToString("0.000")), 2) * 100;
                //var Shutdown = Math.Round(decimal.Parse((ShutdownDuration / totalDuration).ToString("0.000")), 2) * 100;
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                Dictionary<string, object> ExerciseMap = new Dictionary<string, object>
                {
                    { "name", "ExerciseRate"},
                    { "value", ExerciseRate }
                };

                Dictionary<string, object> MarriageMap = new Dictionary<string, object>
                {
                    { "name", "MarriageRate"},
                    { "value", MarriageRate }
                };
                listMap.Add(ExerciseMap);
                listMap.Add(MarriageMap);
                return ResultUtil.Success(listMap, "运动率/嫁动率查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError("设备ID:" + deviceId + "运动率/嫁动率查询查询异常:" + ex);
                return ResultUtil.Fail("运动率/嫁动率查询查询异常" + ex.ToString());
            }
        }





        /// <summary>
        /// 首页-获取所有可联网机床的实时状态 	实时设备状态监控
        /// </summary>
        /// <returns></returns>
        [HttpGet("DeviceRealTimeStatus")]
        public string DeviceRealTimeStatus()
        {
            try
            {
                //查询所有设备列表
                Task<List<CNCDto>> tesk = GetCNCs1();
                List<CNCDto> listCNC = tesk.Result;
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                foreach (var cnc in listCNC)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    map.Add("name", cnc.Name);
                    map.Add("id", cnc.DeviceId);
                    if (true == cnc.Alarm)
                    {
                        map.Add("State", 99);
                    }
                    if (true == cnc.Emergency)
                    {
                        map.Add("State", 98);
                    }
                    map.Add("State", cnc.State);
                    listMap.Add(map);
                }
                return ResultUtil.Success(listMap, "查询所有可联网机床设备实时状态成功");
            }
            catch (Exception ex)
            {
                logger.LogError("查询所有可联网机床设备状态异常：" + ex);
                return ResultUtil.Fail("查询所有可联网机床设备状态异常：" + ex.Message);
            }
        }



        /// <summary>
        /// 首页-实时报警信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("RealTimeAlarmMessage")]
        public string RealTimeAlarmMessage()
        {
            try
            {
                //调用CNC接口中方法在处理出来实时的报警信息
                Task<List<CNCDto>> task = GetCNCs1();
                List<CNCDto> listCnc = task.Result;
                List<AlarmMessageDto> listAlarm = new List<AlarmMessageDto>();
                foreach (var cnc in listCnc)
                {
                    if (cnc.Alarm)
                    {
                        IList<CNCDto.AlarmMessageDto> alarmMessages = cnc.AlarmMessages;
                        foreach (var alarm in alarmMessages)
                        {
                            AlarmMessageDto alarmMessage = new AlarmMessageDto();
                            alarmMessage.DeviceName = cnc.Name;
                            alarmMessage.Number = alarm.Number;
                            alarmMessage.Message = alarm.Message;
                            alarmMessage.StartAt = alarm.StartAt;
                            alarmMessage.EndAt = alarm.EndAt;
                            listAlarm.Add(alarmMessage);
                        }
                    }
                }
                return ResultUtil.Success(listAlarm, "查询实时报警信息成功");
            }
            catch (Exception ex)
            {
                return ResultUtil.Fail("查询实时报警信息异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 首页-报警时长TOP5
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTop5")]
        public string GetTop5()
        {
            try
            {
                List<Dictionary<string, object>> mapList = statusTotalDAL.GetTop5();
                if (mapList.Count > 5)
                {
                    mapList = mapList.GetRange(0, 5);
                }
                return ResultUtil.Success(mapList, "查询报警时长TOP5成功");
            }
            catch (Exception ex)

            {
                return ResultUtil.Fail("报警时长TOP5：" + ex.Message);
            }
        }


        /// <summary>
        /// 首页-状态分布 设备状态分布
        /// </summary>
        /// <returns></returns>
        [HttpGet("StatusDistributed")]
        public string StatusDistributed()
        {
            try
            {
                Task<List<CNCDto>> task = GetCNCs1();
                List<CNCDto> listCNC = task.Result;
                decimal count = listCNC.Count;
                decimal ReSet = 0.00m;
                decimal Stop = 0.00m;
                decimal Hold = 0.00m;
                decimal Start = 0.00m;
                decimal Mstr = 0.00m;
                decimal Alarm = 0.00m;
                decimal Emergency = 0.00m;
                foreach (var cnc in listCNC)
                {
                    if (cnc.RunState == "RESET")
                    {
                        ReSet++;
                    }
                    if (cnc.RunState == "STOP")
                    {
                        Stop++;
                    }
                    if (cnc.RunState == "HOLD")
                    {
                        Hold++;
                    }
                    if (cnc.RunState == "START")
                    {
                        Start++;
                    }
                    if (cnc.RunState == "MSTR")
                    {
                        Mstr++;
                    }
                    if (cnc.Emergency == true)
                    {
                        Emergency++;
                    }
                    if (cnc.Alarm == true)
                    {
                        Alarm++;
                    }
                }
                var ReSetRate = Math.Round(decimal.Parse((ReSet / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> ReSetRateMap = new Dictionary<string, object>();
                ReSetRateMap.Add("name", 0);
                ReSetRateMap.Add("value", ReSetRate);

                var StopRate = Math.Round(decimal.Parse((Stop / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> StoptRateMap = new Dictionary<string, object>();
                StoptRateMap.Add("name", 1);
                StoptRateMap.Add("value", StopRate);

                var HoldRate = Math.Round(decimal.Parse((Hold / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> HoldRateMap = new Dictionary<string, object>();
                HoldRateMap.Add("name", 2);
                HoldRateMap.Add("value", HoldRate);

                var StartRate = Math.Round(decimal.Parse((Start / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> StartRateMap = new Dictionary<string, object>();
                StartRateMap.Add("name", 3);
                StartRateMap.Add("value", StartRate);

                var MstrRate = Math.Round(decimal.Parse((Mstr / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> MstrRateMap = new Dictionary<string, object>();
                MstrRateMap.Add("name", 4);
                MstrRateMap.Add("value", MstrRate);

                var EmergencyRate = Math.Round(decimal.Parse((Emergency / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> EmergencyRateMap = new Dictionary<string, object>();
                EmergencyRateMap.Add("name", 99);
                EmergencyRateMap.Add("value", EmergencyRate);

                var AlarmRate = Math.Round(decimal.Parse((Alarm / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> AlarmRateMap = new Dictionary<string, object>();
                AlarmRateMap.Add("name", 98);
                AlarmRateMap.Add("value", AlarmRate);
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                listMap.Add(ReSetRateMap);
                listMap.Add(StoptRateMap);
                listMap.Add(HoldRateMap);
                listMap.Add(StartRateMap);
                listMap.Add(MstrRateMap);
                listMap.Add(EmergencyRateMap);
                listMap.Add(AlarmRateMap);
                return ResultUtil.Success(listMap, "查询设备实时状态成功");
            }
            catch (Exception ex)
            {
                logger.LogError("查询状态分布异常:" + ex);
                return ResultUtil.Fail("查询状态分布异常：" + ex);
            }
        }


        /// <summary>
        /// 首页-查询所有设备列表(联网的设备)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDeviceList")]
        public string GetDeviceList()
        {
            try
            {
                List<DeviceList> deviceList = deviceListDAL.getAllDevice();
                return ResultUtil.Success(deviceList, "查询设备列表成功");
            }
            catch (Exception ex)
            {
                return ResultUtil.Fail("查询设备列表异常：" + ex.Message);
            }
        }







        /// <summary>
        /// 单台设备看板-加工零件数目
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="mark"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("GetMakeNumber")]
        public string GetMakeNumber(string deviceId, int mark, long startTime, long endTime)
        {
            try
            {
                List<Dictionary<string, object>> mapList = new List<Dictionary<string, object>>();
                //如果是按班或者是天这返回本班次或本天每小时的加工个数。
                //如果是按月或者是俺时间段这返回这个月或者是这个时间段每天的生产数量
                //按班次和天
                if (0 == mark)
                {
                    mapList = partsTotalDAL.TimeinTervalProduction(deviceId, ToTime(startTime), ToTime(endTime));
                }
                //按月或是时间段
                if (1 == mark)
                {
                    mapList = partsTotalDAL.CumulativeProduction(deviceId, ToTime(startTime), ToTime(endTime));
                }
                return ResultUtil.Success(mapList, "查询设备列表成功");
            }
            catch (Exception ex)
            {
                string message = $"获取加工件数列表数据异常：{ex}";
                logger.LogError(message);
                return ResultUtil.Fail("获取加工件数列表数据异常：" + ex.Message);
            }
        }


        /// <summary>
        /// 单台设备看板-加工零件编码
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("PartCode")]
        public string PartCode(string deviceId)
        {
            try
            {
                //获取程序名称
                string currentName = statusTotalDAL.getCurrentName(deviceId);
                string partCode = currentName; //零件规则制定之后在具体添加
                Dictionary<string, object> map = new Dictionary<string, object>();
                map.Add("code", currentName);
                return ResultUtil.Success(map, "查询当前在正在加工的零件编码成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前在正在加工的零件编码异常:{ex}");
                return ResultUtil.Fail("查询当前在正在加工的零件编码异常" + ex.ToString());
            }
        }



        /// <summary>
        /// 单台设备看板-刀具信息展示
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("GetCutterInfo")]
        public string GetCutterInfo(string deviceId)
        {
            try
            {
                Task<CNCDto> tesk = GetCNC1(deviceId);
                CNCDto cnc = tesk.Result;
                if (cnc == null)
                {
                    return ResultUtil.Success(null, "未查询当前设备刀具信息");
                }
                Dictionary<string, object> map = null; ;

                var CurrentCutterNumber = cnc.CurrentCutterNumber;
                var FeedSpeed = cnc.FeedSpeed;
                var FeedSpeedUnit = cnc.FeedSpeedUnit;
                var FeedOverride = cnc.FeedOverride;
                var SpindleSpeed = cnc.SpindleSpeed;
                var CurrentDuration = 0.00;
                //通过设备ID和刀号查询当前刀具的使用时长
                List<CutterTotal> cutterTotalList = cutterTotalDAL.GETCurrentDuration(deviceId, cnc.CurrentCutterNumber);
                foreach (var cutterTotal in cutterTotalList)
                {
                    CurrentDuration += cutterTotal.UseDuration;
                }
                map = new Dictionary<string, object>()
                        {
                            {"CurrentCutterNumber",CurrentCutterNumber},
                            {"FeedSpeed",FeedSpeed },
                            {"FeedSpeedUnit",FeedSpeedUnit },
                            {"FeedOverride", FeedOverride},
                            {"CurrentDuration", CurrentDuration}
                        };

                return ResultUtil.Success(map, "查询当前设备刀具信息成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前设备刀具信息异常:{ex}");
                return ResultUtil.Fail("查询当前设备刀具信息异常" + ex.ToString());
            }
        }

        /// <summary>
        /// 单台设备看板-查询主轴负载
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSpindleLoad")]
        public string GetSpindleLoad(string deviceId)
        {
            try
            {
                Task<CNCDto> task = GetCNC1(deviceId);
                CNCDto cnc = task.Result;
                //增加判断未获取到主轴负载数据
                if (null == cnc)
                {
                    Console.WriteLine("cnc.Axes====未获取到");
                    ResultUtil.Success(null, "未查询当前设备主轴负载信息");
                }
                IList<CNCDto.AxisDto> list = cnc.Axes;
                List<Dictionary<string, object>> mapList = new List<Dictionary<string, object>>();
                foreach (var dto in list)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>()
                    {
                        {"Name", dto.Name},
                        {"Load",dto.Load}
                    };
                    mapList.Add(map);
                }
                return ResultUtil.Success(mapList, "查询当前设备主轴负载信息成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前设备主轴负载信息异常:{ex}");
                return ResultUtil.Fail("查询当前设备主轴负载信息异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 单台设备看板-加工零件百分比
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("GetPartPercentage")]
        public string GetPartPercentage(string deviceId)
        {
            try
            {
                //TODO 没有查询到获取 程序总行的方法
                return ResultUtil.Success("", "查询当前零件加工百分比成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前零件加工百分比异常:{ex}");
                return ResultUtil.Fail("查询当前零件加工百分比：" + ex.ToString());
            }
        }

        /// <summary>
        /// 单台设备看板-状态分布百分比
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("GetStatusPercentage")]
        public string GetStatusPercentage(string deviceId, long startTime, long endTime)
        {
            try
            {
                List<StatusTotal> statusList = statusTotalDAL.EfficiencyAnalysisById(deviceId, ToTime(startTime), ToTime(endTime));
                decimal ReSet = 0.00m;
                decimal Stop = 0.00m;
                decimal Hold = 0.00m;
                decimal Start = 0.00m;
                decimal Mstr = 0.00m;
                decimal Alarm = 0.00m;
                decimal Emergency = 0.00m;

                if (null == statusList)
                {
                    return ResultUtil.Success(null, "未查询当前时间段内设备状态分布百分比");
                }
                foreach (var total in statusList)
                {
                    if (total.DeviceStatus == 0)
                    {
                        ReSet++;
                    }
                    if (total.DeviceStatus == 1)
                    {
                        Stop++;
                    }
                    if (total.DeviceStatus == 2)
                    {
                        Hold++;
                    }
                    if (total.DeviceStatus == 3)
                    {
                        Start++;
                    }
                    if (total.DeviceStatus == 4)
                    {
                        Mstr++;
                    }
                    if (total.DeviceStatus == 98)
                    {
                        Emergency++;
                    }
                    if (total.DeviceStatus == 99)
                    {
                        Alarm++;
                    }
                }
                //所有状态总数
                var count = statusList.Count;
                var ReSetRate = Math.Round(decimal.Parse((ReSet / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> ReSetRateMap = new Dictionary<string, object>();
                ReSetRateMap.Add("name", 0);
                ReSetRateMap.Add("value", ReSetRate);

                var StopRate = Math.Round(decimal.Parse((Stop / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> StoptRateMap = new Dictionary<string, object>();
                StoptRateMap.Add("name", 1);
                StoptRateMap.Add("value", StopRate);

                var HoldRate = Math.Round(decimal.Parse((Hold / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> HoldRateMap = new Dictionary<string, object>();
                HoldRateMap.Add("name", 2);
                HoldRateMap.Add("value", HoldRate);

                var StartRate = Math.Round(decimal.Parse((Start / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> StartRateMap = new Dictionary<string, object>();
                StartRateMap.Add("name", 3);
                StartRateMap.Add("value", StartRate);

                var MstrRate = Math.Round(decimal.Parse((Mstr / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> MstrRateMap = new Dictionary<string, object>();
                MstrRateMap.Add("name", 4);
                MstrRateMap.Add("vaule", MstrRate);

                var EmergencyRate = Math.Round(decimal.Parse((Emergency / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> EmergencyRateMap = new Dictionary<string, object>();
                EmergencyRateMap.Add("name", 98);
                EmergencyRateMap.Add("value", EmergencyRate);

                Console.WriteLine("Alarm====" + Alarm);
                var AlarmRate = Math.Round(decimal.Parse((Alarm / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> AlarmRateMap = new Dictionary<string, object>();
                AlarmRateMap.Add("name", 99);
                AlarmRateMap.Add("value", AlarmRate);
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                listMap.Add(ReSetRateMap);
                listMap.Add(StoptRateMap);
                listMap.Add(HoldRateMap);
                listMap.Add(StartRateMap);
                listMap.Add(MstrRateMap);
                listMap.Add(EmergencyRateMap);
                listMap.Add(AlarmRateMap);
                return ResultUtil.Success(listMap, "查询当前设备状态分布百分比成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前设备状态分布百分比异常:{ex}");
                return ResultUtil.Fail("查询当前设备状态分布百分比异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 档案管理-获取设备档案管理列表 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetArchivesList")]
        public string GetArchivesList(int pageNo, int pageSize)
        {
            try
            {
                List<DeviceArchives> archiversList = archivesDAL.GetList("1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                Dictionary<string, object> map = new Dictionary<string, object>();
                int tatol = archivesDAL.GetRecordCount("");
                map.Add("total", tatol);
                map.Add("list", archiversList);
                return ResultUtil.Success(map, "查询设备档案管理列表成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备档案管理列表异常:{ex}");
                return ResultUtil.Fail("查询设备档案管理列表异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 档案管理-通过ID删除一条设备档案管理的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteArchives")]
        public string DeleteArchives(string id)
        {
            try
            {
                Boolean flag = archivesDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除设备档案管理数据成功");
                }
                return ResultUtil.Fail("删除设备档案管理数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"删除设备档案管理数据异常:{ex}");
                return ResultUtil.Fail("删除设备档案管理数据：" + ex.ToString());
            }
        }

        /// <summary>
        /// 档案管理-通过ID修改数据
        /// </summary>
        /// <param name="archives"></param>
        /// <returns></returns>
        [HttpPost("UpdateArchives")]
        public string UpdateArchives(DeviceArchives archives)
        {
            try
            {
                archives.UpdateTime = DateTime.Now;
                Boolean flag = archivesDAL.Update(archives);
                if (flag)
                {
                    return ResultUtil.Success("修改设备档案数据成功");
                }
                return ResultUtil.Fail("修改设备档案数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"修改设备档案数据异常:{ex}");
                return ResultUtil.Fail("修改设备档案数据：" + ex.ToString());
            }
        }

        /// <summary>
        /// 档案管理-增加设备管理的数据
        /// </summary>
        /// <param name="archives"></param>
        /// <returns></returns>
        [HttpPost("AddArchives")]
        public string AddArchives(DeviceArchives archives)
        {
            try
            {
                Boolean flag = archivesDAL.Add(archivesDAL.SetDeviceArchives(archives));
                if (flag)
                {
                    return ResultUtil.Success("增加设备档案数据成功");
                }
                return ResultUtil.Fail("增加设备档案数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"增加设备档案数据异常:{ex}");
                return ResultUtil.Fail("增加设备档案数据：" + ex.ToString());
            }
        }

        /// <summary>
        /// 档案管理-查询档案详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetArchiversById")]
        public string GetArchiversById(string id)
        {
            try
            {
                DeviceArchives archives = archivesDAL.GetById(id);
                return ResultUtil.Success(archives, "查询设备档案数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备档案详情数据异常:{ex}");
                return ResultUtil.Fail("查询设备档案详情数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 维保计划-获取维保计划列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMaintainList")]
        public string GetMaintainList(int pageNo, int pageSize)
        {
            try
            {
                List<DeviceMaintain> maintainsList = maintainDAL.GetList("1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                Dictionary<string, object> map = new Dictionary<string, object>();
                int tatol = maintainDAL.GetRecordCount("");
                map.Add("total", tatol);
                map.Add("list", maintainsList);
                return ResultUtil.Success(map, "查询设备维保列表数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备维保列表数据异常:{ex}");
                return ResultUtil.Fail("查询设备维保列表数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 维保计划-增加维保
        /// </summary>
        /// <param name="maintain"></param>
        /// <returns></returns>
        [HttpPost("AddMaintain")]
        public string AddMaintain(DeviceMaintain maintain)
        {
            try
            {
                maintain.UpdateTime = DateTime.Now;
                var falg = maintainDAL.Add(maintainDAL.SetDeviceMaintain(maintain));
                if (falg)
                {
                    return ResultUtil.Success("增加设备维保数据成功");
                }
                return ResultUtil.Fail("增加设备维保数据成功");

            }
            catch (Exception ex)
            {
                logger.LogError($"增加设备维保数据异常:{ex}");
                return ResultUtil.Fail("增加设备维保数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 维保计划-删除数据设备维保数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteMaintain")]
        public string DeleteMaintain(string id)
        {
            try
            {
                Boolean flag = maintainDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除设备维保数据成功");
                }
                return ResultUtil.Fail("删除设备维保数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"删除设备维保数据异常:{ex}");
                return ResultUtil.Fail("删除设备维保数据异常：" + ex.ToString());
            }
        }


        /// <summary>
        /// 维保计划-通过ID修改数据设备维保
        /// </summary>
        /// <param name="maintain"></param>
        /// <returns></returns>
        [HttpPost("UpdateMaintain")]
        public string UpdateMaintain(DeviceMaintain maintain)
        {
            try
            {
                maintain.UpdateTime = DateTime.Now;
                Boolean flag = maintainDAL.Update(maintain);
                if (flag)
                {
                    return ResultUtil.Success("修改设备维保数据成功");
                }
                return ResultUtil.Fail("修改设备维保数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"修改设备维保数据异常:{ex}");
                return ResultUtil.Fail("修改设备维保数据：" + ex.ToString());
            }
        }

        /// <summary>
        /// 维保计划-查询设备维保详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetMaintainById")]
        public string GetMaintainById(string id)
        {
            try
            {
                DeviceMaintain maintain = maintainDAL.GetById(id);
                return ResultUtil.Success(maintain, "查询设备维保数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备维保详情数据异常:{ex}");
                return ResultUtil.Fail("查询设备维保详情数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 完成维保计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CompleteMaintain")]
        public string CompleteMaintain(string id)
        {
            try
            {
                DeviceMaintain maintain = maintainDAL.GetById(id);

                maintainDAL.AddHis(maintainDAL.SetDeviceMaintain(maintain));
                //保存歷史數據
                maintain.MaintainState = 1;
                maintain.LastTime = DateTime.Now;
                maintain.ActualTime = DateTime.Now;
                maintain.MaintainState = 1;
                //PlannedTime 计算机化保养时间
                //0 半月检查
                if (0 == maintain.Cycle)
                {
                    maintain.PlannedTime = DateTime.Now.Date.AddDays(15);
                }
                //1 年检
                if (1 == maintain.Cycle)
                {
                    maintain.PlannedTime = DateTime.Now.Date.AddYears(1);
                }
                maintain.UpdateTime = DateTime.Now;
                maintainDAL.Update(maintain);

                return ResultUtil.Fail("完成设备维保成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"完成设备维保异常:{ex}");
                return ResultUtil.Fail("完成设备维保异常：" + ex.ToString());
            }

        }







        /// <summary>
        /// 设备检点-设备检点列表查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCheckPointList")]
        public string GetCheckPointList(int pageNo, int pageSize)
        {
            try
            {
                List<CheckPoint> checkPointList = checkPointDAL.GetList("1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                Dictionary<string, object> map = new Dictionary<string, object>();
                int tatol = checkPointDAL.GetRecordCount("");
                map.Add("total", tatol);
                map.Add("list", checkPointList);
                return ResultUtil.Success(map, "查询设备检点数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备检点数据异常:{ex}");
                return ResultUtil.Fail("查询设备检点数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 设备检点-增加设备检点数据
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [HttpPost("AddCheckPoint")]
        public string AddCheckPoint(CheckPoint point)
        {
            try
            {
                Boolean flag = checkPointDAL.Add(checkPointDAL.SetCheckPoint(point));
                if (flag)
                {
                    return ResultUtil.Success("增加设备检点数据成功");
                }
                else
                {
                    return ResultUtil.Fail("增加设备检点数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"增加设备检点数据异常:{ex}");
                return ResultUtil.Fail("增加设备检点数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        ///  设备检点-修改设备检点数据
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [HttpPost("UpdateCheckPoint")]
        public string UpdateCheckPoint(CheckPoint point)
        {
            try
            {
                point.UpdateTime = DateTime.Now;
                Boolean flag = checkPointDAL.Update(point);
                if (flag)
                {
                    return ResultUtil.Success("修改设备检点数据成功");
                }
                else
                {
                    return ResultUtil.Fail("修改设备检点数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"修改设备检点数据异常:{ex}");
                return ResultUtil.Fail("修改设备检点数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 设备检点-删除设备检点数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteCheckPoint")]
        public string DeleteCheckPoint(string id)
        {
            try
            {
                Boolean flag = checkPointDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除设备检点数据成功");
                }
                else
                {
                    return ResultUtil.Fail("删除设备检点数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"删除设备检点数据异常:{ex}");
                return ResultUtil.Fail("修改设备检点数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 设备检点-查询设备检点详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetCheckPoint")]
        public string GetCheckPoint(string id)
        {
            try
            {
                CheckPoint checkPoint = checkPointDAL.GetById(id);
                return ResultUtil.Success(checkPoint, "查询设备检点详情数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备检点详情数据异常:{ex}");
                return ResultUtil.Fail("查询设备检点详情数据异常：" + ex.ToString());
            }
        }


        /// <summary>
        /// 查询设备维修分页列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetAllDeviceRepair")]
        public string GetAllDeviceRepair(int pageNo, int pageSize)
        {
            try
            {
                List<DeviceRepair> deviceRepairList = deviceRepairDAL.GetList("1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize); ;
                int count = deviceRepairDAL.GetRecordCount("");
                Dictionary<string, object> map = new Dictionary<string, object>();
                map.Add("total", count);
                map.Add("list", deviceRepairList);
                return ResultUtil.Success(map, "查询设备维修分页列表成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备维修分页列表数据异常:{ex}");
                return ResultUtil.Fail("查询设备维修分页列表数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 增加维修数据
        /// </summary>
        /// <param name="deviceRepair"></param>
        /// <returns></returns>
        [HttpPost("AddDeviceRepair")]
        public string AddDeviceRepair(DeviceRepair deviceRepair)
        {
            try
            {
                bool flag = deviceRepairDAL.Add(deviceRepairDAL.SetDeviceRepair(deviceRepair));
                if (flag)
                {
                    return ResultUtil.Success("增加设备维修数据成功");
                }
                return ResultUtil.Fail("增加设备维修数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"增加设备维修数据异常:{ex}");
                return ResultUtil.Fail("增加设备维修数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 修改维修计划数据
        /// </summary>
        /// <param name="deviceRepair"></param>
        /// <returns></returns>
        [HttpPost("UpdateDeviceRepair")]
        public string UpdateDeviceRepair(DeviceRepair deviceRepair)
        {
            try
            {
                deviceRepair.UpdateTime = DateTime.Now;
                bool flag = deviceRepairDAL.Update(deviceRepair);
                if (flag)
                {
                    return ResultUtil.Success("修改设备维修数据成功");
                }
                return ResultUtil.Fail("修改设备维修数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"修改设备维修数据异常:{ex}");
                return ResultUtil.Fail("修改设备维修数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 删除维修记录数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteDeviceRepairById")]
        public string DeleteDeviceRepairById(string id)
        {
            try
            {
                bool flag = deviceRepairDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除设备维修数据成功");
                }
                return ResultUtil.Fail("删除设备维修数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"删除设备维修数据异常:{ex}");
                return ResultUtil.Fail("删除设备维修数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 查询设备维修详情数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetDeviceRepairById")]
        public string GetDeviceRepairById(string id)
        {
            try
            {
                DeviceRepair deviceRepair = deviceRepairDAL.GetById(id);
                return ResultUtil.Success(deviceRepair, "查询设备维修详情数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备维修详情数据异常:{ex}");
                return ResultUtil.Fail("查询设备维修详情数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 设备维修完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CompleteDeviceRepair")]
        public string CompleteDeviceRepair(string id)
        {
            try
            {
                DeviceRepair repair = deviceRepairDAL.GetById(id);
                double hour = GetHourDifference(repair.StartTime, DateTime.Now);
                repair.RepairDuration = hour;
                repair.EndTime = DateTime.Now;
                repair.RepairState = 1;
                repair.UpdateTime = DateTime.Now;
                bool flag = deviceRepairDAL.Update(repair);
                if (flag)
                {
                    return ResultUtil.Fail("设备维修完成成功");
                }
                return ResultUtil.Fail("设备维修失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"设备维修完成异常:{ex}");
                return ResultUtil.Fail("设备维修完成异常：" + ex.ToString());
            }
        }




        /// <summary>
        /// 
        /// 生产统计-实时产量统计
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpGet("RealTimeOutPut")]
        public string RealTimeOutPut(string deviceId, string time)
        {
            try
            {
                List<Dictionary<string, object>> mapList = partsTotalDAL.RealTimeOutPut(deviceId, time);
                return ResultUtil.Success(mapList, "实时产量统计成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"实时产量统计异常：{ex}");
                return ResultUtil.Fail("实时产量统计异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 5.生产统计
        ///  生产统计-累计产量统计 
        /// </summary>
        /// <returns></returns>
        [HttpGet("CumulativeProduction")]
        public string CumulativeProduction(string deviceId, long startTime, long endTime)
        {
            try
            {
                List<Dictionary<string, object>> mapList = partsTotalDAL.CumulativeProduction(deviceId, ToTime(startTime), ToTime(endTime));
                return ResultUtil.Success(mapList, "累计产量统计成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"累计产量统计异常：{ex}");
                return ResultUtil.Fail("累计产量统计异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 合格率统计分析-添加合格率统计分数据  
        /// </summary>
        /// <param name="passRate"></param>
        [HttpPost("AddPassRate")]
        public string AddPassRate(PassRate passRate)
        {
            try
            {
                Boolean flag = passRateDAL.Add(passRateDAL.SetPassRate(passRate));
                if (flag)
                {
                    return ResultUtil.Success("添加合格率统计分数据成功");
                }
                else
                {
                    return ResultUtil.Fail("添加合格率统计分数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"添加合格率统计分数据失败:{ex}");
                return ResultUtil.Fail("添加合格率统计分数据失败");
            }
        }


        /// <summary>
        ///  合格率统计分析-查询合格率统计分析
        /// </summary>
        /// <returns></returns>
        [HttpGet("PassRateAnalysis")]
        public string PassRateAnalysis(int pageNo, int pageSize)
        {
            try
            {
                List<PassRate> passRateList = passRateDAL.GetList(" 1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                Dictionary<string, object> map = new Dictionary<string, object>();
                int tatol = passRateDAL.GetRecordCount("");
                map.Add("total", tatol);
                map.Add("list", passRateList);
                return ResultUtil.Success(map, "查询合格率统计分析成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询合格率统计分数据异常:{ex}");
                return ResultUtil.Fail("查询合格率统计分数据异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 合格率统计分析-修改合格率统计分析
        /// </summary>
        /// <param name="passRate"></param>
        /// <returns></returns>
        [HttpPost("UpdatePassRate")]
        public string UpdatePassRate(PassRate passRate)
        {
            try
            {
                passRate.UpdateTime = DateTime.Now;
                Boolean flag = passRateDAL.Update(passRate);
                if (flag)
                {
                    return ResultUtil.Success("修改合格率统计分数据成功");
                }
                else
                {
                    return ResultUtil.Fail("修改合格率统计分数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"修改合格率统计分数据异常:{ex}");
                return ResultUtil.Fail("修改合格率统计分数据异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 合格率统计分析-获取合格率统计分析详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetPassRateById")]
        public string GetPassRateById(string id)
        {
            try
            {
                PassRate passRate = passRateDAL.GetById(id);
                return ResultUtil.Success(passRate, "查询合格率统计分析详情成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询合格率统计分析详情异常:{ex}");
                return ResultUtil.Fail("查询合格率统计分析详情异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 合格率统计分析-删除合格率统计分析详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeletePassRateById")]
        public string DeletePassRateById(string id)
        {
            try
            {
                Boolean flag = passRateDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除合格率统计分数据成功");
                }
                else
                {
                    return ResultUtil.Fail("删除合格率统计分数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"删除合格率统计分数据异常:{ex}");
                return ResultUtil.Fail("删除合格率统计分数据异常：" + ex.Message);
            }
        }





        /// <summary>
        /// 外协加工进度-增加外协加工进度
        /// </summary>
        /// <param name="outsourcing"></param>
        [HttpPost("AddOutsourcing")]
        public string AddOutsourcing(Outsourcing outsourcing)
        {
            try
            {
                Boolean flag = outsourcingDAL.Add(outsourcingDAL.SetOutsourcing(outsourcing));
                if (flag)
                {
                    return ResultUtil.Success("增加外协加工进度数据成功");
                }
                else
                {
                    return ResultUtil.Fail("增加外协加工进度数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"增加外协加工进度数据异常:{ex}");
                return ResultUtil.Fail("增加外协加工进度数据异常：" + ex.Message);
            }
        }


        /// <summary>
        /// 外协加工进度-修改外协加工进度
        /// </summary>
        /// <param name="outsourcing"></param>
        /// <returns></returns>
        [HttpPost("UpdateOutsourcing")]
        public string UpdateOutsourcing(Outsourcing outsourcing)
        {
            try
            {
                outsourcing.UpdateTime = DateTime.Now;
                Boolean flag = outsourcingDAL.Update(outsourcing);

                if (flag)
                {
                    return ResultUtil.Success("修改外协加工进度数据成功");
                }
                else
                {
                    return ResultUtil.Fail("修改外协加工进度数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("修改外协加工进度数据异常：" + ex);
                return ResultUtil.Fail("修改外协加工进度数据异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 外协加工进度-通过ID查询外协加工进度详细数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetOutsourcing")]
        public string GetOutsourcing(string id)
        {
            try
            {
                Outsourcing outsourcing = outsourcingDAL.GetById(id);
                return ResultUtil.Success(outsourcing, "查询外协加工进度详细信息成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询外协加工进度详细信息异常：{ex}");
                return ResultUtil.Fail("查询外协加工进度详细信息异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 外协加工进度-删除外协加工数据通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteOutsourcing")]
        public string DeleteOutsourcing(string id)
        {
            try
            {
                Boolean flag = outsourcingDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除外协加工进度数据成功");
                }
                else
                {
                    return ResultUtil.Fail("删除外协加工进度数据失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("删除外协加工进度数据异常：" + ex);
                return ResultUtil.Fail("删除外协加工进度数据异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 外协加工进度-外协加工进度列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOutsourcingList")]
        public string GetOutsourcingList(int pageNo, int pageSize)
        {
            try
            {
                List<Outsourcing> list = outsourcingDAL.GetList(" 1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                Dictionary<string, object> map = new Dictionary<string, object>();
                int tatol = outsourcingDAL.GetRecordCount("");
                map.Add("total", tatol);
                map.Add("list", list);
                return ResultUtil.Success(map, "查询外协加工进度列表成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询外协加工进度列表异常：{ex}");
                return ResultUtil.Fail("查询外协加工进度列表异常：" + ex.Message);
            }
        }




        /// <summary>
        /// 计划达成率
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("PlannedOutPut")]
        public string PlannedOutPut(string deviceId, long startTime, long endTime)
        {
            try
            {
                if (deviceId.Length < 5)
                {
                    deviceId = string.Empty;
                }
                //查询某台或者是所有机床一段时间内每天的产量
                List<Dictionary<string, object>> listMap = partsTotalDAL.CumulativeProduction(deviceId, ToTime(startTime), ToTime(endTime));
                //TODO ERP系统接口
                List<Dictionary<string, object>> dailySchedileList = new List<Dictionary<string, object>>();
                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
                keyValuePairs.Add("Time", "2022-05-13");
                keyValuePairs.Add("Total", 100000);

                Dictionary<string, object> keyValuePairs1 = new Dictionary<string, object>();
                keyValuePairs1.Add("Time", "2022-05-14");
                keyValuePairs1.Add("Total", 500);

                Dictionary<string, object> keyValuePairs2 = new Dictionary<string, object>();
                keyValuePairs2.Add("Time", "2022-05-15");
                keyValuePairs2.Add("Total", 683);


                dailySchedileList.Add(keyValuePairs);
                dailySchedileList.Add(keyValuePairs1);
                dailySchedileList.Add(keyValuePairs2);


                //TODO 计算是这一段时间的达成率还是排产每天的达成率
                //计算每天的达成率
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                for (int i = 0; i < listMap.Count; i++)
                {
                    Dictionary<string, object> map = listMap[i];
                    test1.Add(map["Time"].ToString(), map["Total"]);
                }

                Dictionary<string, object> test2 = new Dictionary<string, object>();
                for (int i = 0; i < dailySchedileList.Count; i++)
                {
                    Dictionary<string, object> map = dailySchedileList[i];
                    test2.Add(map["Time"].ToString(), map["Total"]);
                }
                List<Dictionary<string, object>> ResuleList = new List<Dictionary<string, object>>();
                foreach (var daily in test2)
                {
                    Dictionary<string, object> resule = new Dictionary<string, object>();
                    if (test1.Keys.Contains(daily.Key))
                    {
                        object test;
                        var var = test1.TryGetValue(daily.Key, out test);
                        decimal var1 = decimal.Parse(test.ToString());
                        var a2 = (decimal)int.Parse(daily.Value.ToString());
                        decimal var2 = a2;
                        Console.WriteLine("var1===" + var1);
                        Console.WriteLine("var2===" + var2);
                        var Planned = Math.Round(decimal.Parse((var1 / var2).ToString("0.000")), 2) * 100;
                        resule.Add(daily.Key, Planned);
                    }
                    else
                    {
                        resule.Add(daily.Key, 0);

                    }
                    ResuleList.Add(resule);
                }
                return ResultUtil.Success(ResuleList, "计划达成率查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"计划达成率查询异常：{ex}");
                return ResultUtil.Fail("计划达成率查询异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 计划产量实际产量列表
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("PlanList")]
        public string PlanList(string deviceId)
        {
            try
            {
                //TODO 获取ERP接口计划产量数据
                string startTime = "2022-05-12 00:00:00";
                string endTime = "2022-05-15 23:59:59";
                List<Dictionary<string, object>> DailySchedileList = new List<Dictionary<string, object>>();
                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
                keyValuePairs.Add("Time", "2022-05-13");
                keyValuePairs.Add("Total", 100000);

                Dictionary<string, object> keyValuePairs1 = new Dictionary<string, object>();
                keyValuePairs1.Add("Time", "2022-05-14");
                keyValuePairs1.Add("Total", 500);

                Dictionary<string, object> keyValuePairs2 = new Dictionary<string, object>();
                keyValuePairs2.Add("Time", "2022-05-15");
                keyValuePairs2.Add("Total", 683);


                DailySchedileList.Add(keyValuePairs);
                DailySchedileList.Add(keyValuePairs1);
                DailySchedileList.Add(keyValuePairs2);




                List<Dictionary<string, object>> ActualOutput = partsTotalDAL.CumulativeProduction(deviceId, startTime, endTime);

                Dictionary<string, object> test1 = new Dictionary<string, object>();
                for (int i = 0; i < DailySchedileList.Count; i++)
                {
                    Dictionary<string, object> map = DailySchedileList[i];
                    test1.Add(map["Time"].ToString(), map["Total"]);
                }

                Dictionary<string, object> test2 = new Dictionary<string, object>();
                for (int i = 0; i < ActualOutput.Count; i++)
                {
                    Dictionary<string, object> map = ActualOutput[i];
                    test2.Add(map["Time"].ToString(), map["Total"]);
                }
                List<Dictionary<string, object>> ResuleList = new List<Dictionary<string, object>>();
                foreach (var daily in test1)
                {

                    Dictionary<string, object> resule = new Dictionary<string, object>();
                    if (test2.Keys.Contains(daily.Key))
                    {
                        resule.Add("Time", daily.Key);
                        resule.Add("daily", daily.Value);
                        object test;
                        var var = test2.TryGetValue(daily.Key, out test);
                        resule.Add("Actual", test);
                    }
                    else
                    {
                        resule.Add("Time", daily.Key);
                        resule.Add("daily", daily.Value);
                        resule.Add("Actual", 0);
                    }
                    ResuleList.Add(resule);
                }
                return ResultUtil.Success(ResuleList, "计划产量实际产量列表查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"计划产量实际产量列表查询异常：{ex}");
                return ResultUtil.Fail("计划产量实际产量列表查询异常：" + ex.Message);
            }
        }


        /// <summary>
        /// 报警信息列表统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAlarmList")]
        public string GetAlarmList(int pageNo, int pageSize)
        {
            try
            {
                List<AlarmList> areaLists = alarmListDAL.GetList(" 1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                int count = alarmListDAL.GetRecordCount("");
                Dictionary<string, object> map = new Dictionary<string, object>();
                map.Add("total", count);
                map.Add("list", areaLists);
                return ResultUtil.Success(map, "报警统计列表查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"报警统计列表查询异常：{ex}");
                return ResultUtil.Fail("报警统计列表查询异常：" + ex.Message);
            }
        }





        /// <summary>
        /// 添加虚拟数据(虚拟设备)
        /// </summary>
        /// <param name="deviceDetail"></param>
        /// <returns></returns>
        [HttpPost("AddDeviceDetail")]
        public string AddDeviceDetail(DeviceDetail deviceDetail)
        {
            try
            {
                bool flag = deviceDetailDAL.Add(deviceDetailDAL.SetDeviceDetail(deviceDetail));
                if (flag)
                {
                    return ResultUtil.Success("添加虚拟数据成功");
                }
                return ResultUtil.Fail("报添加虚拟数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"添加虚拟数据成功：{ex}");
                return ResultUtil.Fail("添加虚拟数据成功：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除虚拟设备（虚拟设备数据）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteDeviceDetailById")]
        public string DeleteDeviceDetailById(string id)
        {
            try
            {
                bool flag = deviceDetailDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除虚拟数据成功");
                }
                return ResultUtil.Fail("删除虚拟数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"删除虚拟数据异常：{ex}");
                return ResultUtil.Fail("删除虚拟数据异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 修改虚拟数据（虚拟设备）
        /// </summary>
        /// <param name="deviceDetail"></param>
        /// <returns></returns>
        [HttpPost("UpdateDeviceDetail")]
        public string UpdateDeviceDetail(DeviceDetail deviceDetail)
        {
            try
            {
                deviceDetail.UpdateTime = DateTime.Now;
                bool flag = deviceDetailDAL.Update(deviceDetail);
                if (flag)
                {
                    return ResultUtil.Success("修改虚拟数据成功");
                }
                return ResultUtil.Fail("修改虚拟数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"修改虚拟数据异常：{ex}");
                return ResultUtil.Fail("修改虚拟数据异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 分页查询虚拟设备数据
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetDeviceDetailList")]
        public string GetDeviceDetailList(int pageNo, int pageSize)
        {
            try
            {
                List<DeviceDetail> list = deviceDetailDAL.GetList(" 1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                int count = deviceDetailDAL.GetRecordCount("");
                Dictionary<string, object> map = new Dictionary<string, object>();
                map.Add("tatol", count);
                map.Add("list", list);
                return ResultUtil.Success(map, "虚拟设备列表查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"虚拟设备列表查询异常：{ex}");
                return ResultUtil.Fail("虚拟设备列表查询异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 虚拟设备详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetDeviceDetailById")]
        public string GetDeviceDetailById(string id)
        {
            try
            {
                DeviceDetail deviceDetails = deviceDetailDAL.GetDeviceDetailById(id);
                return ResultUtil.Success(deviceDetails, "查询设备详情成功");
            }
            catch (Exception ex)
            {
                logger.LogError("查询单台设备详情异常:" + ex);
                return ResultUtil.Fail("查询单台设备详情异常：" + ex.Message);
            }
        }






        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="fileDtos"></param>
        [HttpPost("UploadImg")]
        public string UploadImg(FileDtos fileDtos)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileDtos.Type) && fileDtos.Type.ToLower() == "image")
                {
                    //文件类型
                    string FileEextension = Path.GetExtension(fileDtos.Filename).ToLower();//获取文件的后缀//判断文件类型是否是允许的类型
                    List<string> fileType = new List<string>() { ".gif", ".jpg", ".jpeg", ".png", ".bmp", ".GIF", ".JPG", ".JPEG", ".PNG", ".BMP" };
                    if (fileType.Contains(FileEextension))
                    {
                        //图片类型是允许的类型
                        Images_Mes fmster = new Images_Mes();//图片存储信息类，跟mysql里面表名一致
                        string fguid = Guid.NewGuid().ToString().Replace("-", ""); //文件名称
                        fmster.AddTime = DateTime.Now;//添加时间为当前时间

                        if (Base64Helper.IsBase64String(fileDtos.Base64String, out byte[] fmsterByte))
                        {
                            //判断是否是base64字符串，如果是则转换为字节数组，用来保存
                            fmster.FileCon = fmsterByte;
                            Console.WriteLine(fmster.FileCon.ToString());
                        }
                        fmster.FileName = Path.GetFileName(fileDtos.Filename);//文件名称
                        fmster.FileSize = fmster.FileCon.Length;//文件大小
                        fmster.FileType = FileEextension;//文件扩展名
                        fmster.Id = fguid;//唯一主键，通过此来获取图片数据
                        bool flag = imagesMesDAL.Add(fmster);
                        if (flag)
                        {
                            return ResultUtil.Success(fmster.Id, "图片上传成功");
                        }
                    }
                }
                return ResultUtil.Fail("图片上传失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"图片上传异常：{ex}");
                return ResultUtil.Fail("图片上传异常：" + ex.Message);
            }
        }


        /// <summary>
        /// 图片展示
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet("ShowImg")]
        public string ShowImg(string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(guid))
                {
                    return null;
                }
                Images_Mes images = imagesMesDAL.GetById(guid);

                if (images != null)
                {
                    //FileContentResult file = File(images.FileCon, "image/jpeg", images.FileName);
                    byte[] fileCon = images.FileCon;
                    string baseString = Convert.ToBase64String(fileCon);

                    /*MD5 md5Hasher = new MD5CryptoServiceProvider();
                    byte[] arrbytHashValue = md5Hasher.ComputeHash(stream);
                    string fullFileName = System.IO.Directory.GetCurrentDirectory();
                    Console.WriteLine("fullFileName=====" + fullFileName);

                    string strHashData = System.BitConverter.ToString(arrbytHashValue).Replace("-", "");
                    string FileEextension = Path.GetExtension(images.FileName);
                    string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                    string virtualPath = string.Format("/images/{1}{2}", uploadDate, strHashData, FileEextension);

                    //Console.WriteLine(System.Windows.Forms.Application.StartupPath + "\\images\\");
                    string pathstr = fullFileName + "\\" + virtualPath;
                    //Console.WriteLine(pathstr);
                    string path = Path.GetDirectoryName(pathstr);
                    //Console.WriteLine(path);
                    Directory.CreateDirectory(path);

                    MemoryStream stmBLOBData = new MemoryStream(fileCon);
                    Image img = Image.FromStream(stmBLOBData);
                    img.Save(pathstr);*/

                    return ResultUtil.Success("data:image/png;base64," + baseString, "图片数据查询成功");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"图片数据查询异常：{ex}");
                return ResultUtil.Fail("图片数据查询异常：" + ex.Message);
            }



        }



        /// <summary>
        /// 获取员工列
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllEmplyee")]
        public string GetAllEmplyee()
        {
            try
            {
                List<Employee> employeeList = employeeDAL.GetList("");
                return ResultUtil.Success(employeeList, "查询员工类表成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询员工类表异常：{ex}");
                return ResultUtil.Fail("查询员工类表异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取设备档案信息
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet("GetDeviceArchivesList")]
        public string GetDeviceArchivesList()
        {
            try
            {
                List<DeviceArchives> deviceArchives = archivesDAL.GetList("");
                return ResultUtil.Success(deviceArchives, "查询设备档案信息成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备档案信息异常：{ex}");
                return ResultUtil.Fail("查询设备档案信息异常：" + ex.Message);
            }
        }





        /// <summary>
        /// 添加保养记录
        /// </summary>
        /// <param name="maintainRecord"></param>
        /// <returns></returns>
        [HttpPost("AddMaintainRecord")]
        public string AddMaintainRecord(MaintainRecord maintainRecord)
        {
            try
            {
                bool flag = maintainRecordDAL.Add(maintainRecordDAL.SetMaintainRecord(maintainRecord));
                if (flag)
                {
                    return ResultUtil.Success("添加保养记录数据成功");
                }
                return ResultUtil.Fail("添加保养记录失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"添加保养记录异常：{ex}");
                return ResultUtil.Fail("添加保养记录异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 查询保养记录列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetMaintainRecord")]
        public string GetMaintainRecord(int pageNo, int pageSize)
        {
            try
            {
                List<MaintainRecord> maintainRecordList = maintainRecordDAL.GetList(" 1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                return ResultUtil.Success(maintainRecordList,"保养记录列表查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询保养记录异常：{ex}");
                return ResultUtil.Fail("查询保养记录异常：" + ex.Message);
            }
        }



        /// <summary>
        /// 查询一级分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllDictCodeGroup")]
        public string GetAllDictCodeGroup()
        {
            try
            {
                List<DictCodeGroup> dictList = dictCodeGroupDAL.GetList("");
                return ResultUtil.Success(dictList, "查询质量分类一级成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询质量分类一级异常：{ex}");
                return ResultUtil.Fail("查询质量分类一级异常：" + ex.Message);
            }
        }


        //质量分类查询二级
        [HttpGet("GetAllDictCode")]
        public string GetAllDictCode(int code)
        {
            try
            {
                List<DictCode> dictList = dictCodeDAL.GetList(" code_group = " + code);
                return ResultUtil.Success(dictList, "查询质量分类二级成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询质量分类二级异常：{ex}");
                return ResultUtil.Fail("查询质量分类二级异常：" + ex.Message);
            }
        }









        /// <summary>
        /// excel导入数据
        /// </summary>
        /// <param name="fileDtos"></param>
        [HttpPost("ImportExcel")]

        public string ImportExcel(FileDtos fileDtos)
        {
            Base64Helper.IsBase64String(fileDtos.Base64String, out byte[] fmsterByte);
            string fullFilePath = System.IO.Directory.GetCurrentDirectory() + string.Format("\\file\\execl");
            if (!System.IO.Directory.Exists(fullFilePath))
            {
                Directory.CreateDirectory(fullFilePath);
            }
            if (System.IO.File.Exists(fullFilePath + "\\" + fileDtos.Filename))
            {
                System.IO.File.Delete(fullFilePath + "\\" + fileDtos.Filename);
            }
            byte[] byteStr = fmsterByte;
            FileStream fileStream = new FileStream(fullFilePath + "\\" + fileDtos.Filename, FileMode.CreateNew);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(byteStr, 0, byteStr.Length);
            binaryWriter.Close();
            fileStream.Close();
            Console.WriteLine("fullFilePath======" + fullFilePath);

            string excelFilePath = fullFilePath + "\\" + fileDtos.Filename;
            using (FileStream fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                ArrayList rowList = new ArrayList();
                string sheetName = "sheet1";
                bool isColumnName = true;//标识第一行是否为列名(表头) 
                IWorkbook workBook;

                string fileExt = Path.GetExtension(excelFilePath).ToLower();//获取扩展名
                if (fileExt == ".xlsx")
                {
                    workBook = new XSSFWorkbook(fs);
                }
                else if (fileExt == ".xls")
                {
                    workBook = new HSSFWorkbook(fs);
                }
                else
                {
                    workBook = null;
                }
                ISheet sheet = null;
                if (sheetName != null && sheetName != "")
                {
                    sheet = workBook.GetSheet(sheetName);//获取指定sheet名称的工作表
                    if (sheet == null)
                    {
                        sheet = workBook.GetSheetAt(0);//获取第一个工作表
                    }
                }
                else
                {
                    sheet = workBook.GetSheetAt(0);//获取第一个工作表
                }

                IRow header = sheet.GetRow(sheet.FirstRowNum);//获取第一行
                int startRow = 0;
                if (isColumnName)
                {
                    startRow = sheet.FirstRowNum + 1;
                    for (int i = header.FirstCellNum; i < header.LastCellNum; i++)
                    {
                        object obj = GetCellValue(header.GetCell(i));
                        if (obj == null || obj.ToString() == string.Empty)
                        {

                        }
                        else
                        {
                            DataColumn col = new DataColumn(obj.ToString());
                        }
                    }
                }
                int rowIndex = 1;
                //数据读取
                while (sheet.GetRow(rowIndex) != null)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row == null)
                    {
                        continue;
                    }
                    ArrayList cellList = new ArrayList();
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {
                        if (null == row.GetCell(j).ToString() || "" == row.GetCell(j).ToString())
                        {

                        }
                        else
                        {
                            cellList.Add(GetCellValue(row.GetCell(j)).ToString());
                        }
                    }
                    rowList.Add(cellList);
                    rowIndex++;
                }
                //判断业务名称进行数据组装保存数据
                ArrayList list = new ArrayList();
                for (int a = 0; a < rowList.Count; a++)
                {
                    ArrayList list1 = (ArrayList)rowList[a];
                    Console.WriteLine(list1.Count);
                    if (list1.Count > 0)
                    {
                        list.Add(list1);
                    }
                }
                bool falg = SaveExcelData(fileDtos.BusinessName, list);
                if (!falg)
                {
                    return ResultUtil.Fail("导入EXCEL表格数据失败");
                }
                return ResultUtil.Success("导入EXCEL表格数据成功");
            }
        }


        //获取cell的数据，并设置为对应的数据类型
        public object GetCellValue(ICell cell)
        {
            object value = null;
            try
            {
                if (cell.CellType != CellType.Blank)
                {
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            // Date comes here
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                value = cell.DateCellValue;
                            }
                            else
                            {
                                // Numeric type
                                value = cell.NumericCellValue;
                            }
                            break;
                        case CellType.Boolean:
                            // Boolean type
                            value = cell.BooleanCellValue;
                            break;
                        case CellType.Formula:
                            value = cell.CellFormula;
                            break;
                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                value = "";
            }
            return value;
        }



        /// <summary>
        /// 处理保存个业务导入数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rowList"></param>
        /// <returns></returns>

        public bool SaveExcelData(string name, ArrayList rowList)
        {
            bool save = false;
            List<string> listId = new List<string>();
            //Console.WriteLine("+=======================" + rowList.Count);
            if (name == "维保计划")
            {
                List<DeviceMaintain> maintainList = new List<DeviceMaintain>();
                for (int i = 0; i < rowList.Count; i++)
                {
                    ArrayList cellList = (ArrayList)rowList[i];
                    if (cellList != null)
                    {
                        DeviceMaintain maintain = new DeviceMaintain();
                        maintain.DeviceNumber = cellList[0].ToString();
                        maintain.DeviceName = cellList[1].ToString();
                        maintain.DeviceModel = cellList[2].ToString();
                        maintain.DeviceType = cellList[3].ToString();
                        maintain.PurchaseDate = StrToDateTime(cellList[4].ToString());
                        maintain.DurableYears = StrToInt(cellList[5].ToString());
                        maintain.Content = cellList[6].ToString();
                        maintain.Cycle = StrToInt(cellList[7].ToString());
                        maintain.LastTime = StrToDateTime(cellList[8].ToString());
                        maintain.PlannedTime = StrToDateTime(cellList[9].ToString());
                        maintain.ActualTime = StrToDateTime(cellList[10].ToString());
                        maintain.PersonLiable = cellList[11].ToString();
                        maintain.EarlyWarningTime = StrToInt(cellList[12].ToString());
                        string val = cellList[13].ToString();
                        int status = 0;
                        //0 未开始 1 进行中 2 完成
                        if ("未开始" == val)
                        {
                            status = 0;
                        }
                        if ("进行中" == val)
                        {
                            status = 1;
                        }
                        if ("完成" == val)
                        {
                            status = 2;
                        }
                        maintain.MaintainState = status;
                        maintainList.Add(maintain);
                    }
                }

                //将数插入数据库
                foreach (DeviceMaintain maintain in maintainList)
                {
                    listId.Add(maintainDAL.SetDeviceMaintain(maintain).Id);
                    bool falg = maintainDAL.Add(maintainDAL.SetDeviceMaintain(maintain));
                    if (falg)
                    {
                        save = true;
                    }
                    else
                    {
                        //如果有一条失败这删除已经导入的数据
                        foreach (string id in listId)
                        {
                            maintainDAL.DeleteById(id);
                        }
                        return false;
                        break;
                    }
                }
            }
            if (name == "档案管理")
            {
                List<DeviceArchives> archivesList = new List<DeviceArchives>();
                for (int i = 0; i < rowList.Count; i++)
                {
                    ArrayList arrayList = (ArrayList)rowList[i];
                    DeviceArchives archives = new DeviceArchives();
                    archives.DeviceName = arrayList[0].ToString();
                    archives.DeviceType = arrayList[1].ToString();
                    archives.DerviceNumber = arrayList[2].ToString();
                    archives.DeviceModel = arrayList[3].ToString();
                    archives.PurchaseDate = StrToDateTime(arrayList[4].ToString());
                    archives.DurableYears = StrToInt(arrayList[4].ToString());
                    archivesList.Add(archives);
                }

                foreach (DeviceArchives archive in archivesList)
                {
                    DeviceArchives deviceArchives = archivesDAL.SetDeviceArchives(archive);
                    listId.Add(deviceArchives.Id);
                    bool falg = archivesDAL.Add(deviceArchives);
                    if (falg)
                    {
                        save = true;
                    }
                    else
                    {
                        foreach (string id in listId)
                        {
                            archivesDAL.DeleteById(id);
                        }
                        return false;
                        break;
                    }
                }
                return true;
            }
            if (name == "维修记录")
            {
                List<DeviceRepair> repairList = new List<DeviceRepair>();
                for (int i = 0; i < rowList.Count; i++)
                {
                    ArrayList arrayList = (ArrayList)rowList[i];
                    DeviceRepair repair = new DeviceRepair();
                    repair.DeviceName = arrayList[0].ToString();
                    repair.DeviceType = arrayList[1].ToString();
                    repair.DeviceNumber = arrayList[2].ToString();
                    repair.DeviceModel = arrayList[3].ToString();
                    repair.PurchaseDate = StrToDateTime(arrayList[4].ToString());
                    repair.DurableYars = StrToInt(arrayList[5].ToString());
                    repair.StartTime = StrToDateTime(arrayList[6].ToString());
                    repair.EndTime = StrToDateTime(arrayList[7].ToString());
                    repair.reason = arrayList[8].ToString();
                    repair.RepairPersonnel = arrayList[9].ToString();
                    repair.RepairState = StrToInt(arrayList[10].ToString());
                    repair.RepairDuration = StrToDouble(arrayList[11].ToString());
                    repair.RepairCost = StrToDecimal(arrayList[12].ToString());
                    repair.DeviceName = arrayList[13].ToString();
                    repairList.Add(repair);
                }
                foreach (DeviceRepair repair in repairList)
                {
                    DeviceRepair deviceRepair = deviceRepairDAL.SetDeviceRepair(repair);
                    listId.Add(deviceRepair.Id);
                    bool falg = deviceRepairDAL.Add(deviceRepair);
                    if (falg)
                    {
                        save = true;
                    }
                    else
                    {
                        foreach (string id in listId)
                        {
                            deviceRepairDAL.DeleteById(id);
                        }
                        return false;
                        break;
                    }
                }
            }

            if (name == "设备点检")
            {
                List<CheckPoint> pointList = new List<CheckPoint>();
                for (int i = 0; i < rowList.Count; i++)
                {
                    ArrayList arrayList = (ArrayList)rowList[i];
                    CheckPoint point = new CheckPoint();
                    point.Number = arrayList[0].ToString();
                    point.Operator = arrayList[1].ToString();
                    point.DeviceName = arrayList[2].ToString();
                    point.DeviceNumber = arrayList[3].ToString();
                    point.DeviceModel = arrayList[4].ToString();
                    point.SpareParts = StrToInt(arrayList[5].ToString());
                    point.Liquid = StrToInt(arrayList[6].ToString());
                    point.Pressure = StrToInt(arrayList[7].ToString());
                    point.Handle = StrToInt(arrayList[8].ToString());
                    point.SafetyDevices = StrToInt(arrayList[9].ToString());
                    point.InstrumentPressure = StrToInt(arrayList[10].ToString());
                    point.FanScreen = StrToInt(arrayList[11].ToString());
                    point.DriveMotor = StrToInt(arrayList[12].ToString());
                    point.LeakageOilGasWater = StrToInt(arrayList[13].ToString());
                    point.PrincipalAxis = StrToInt(arrayList[14].ToString());
                    point.Appearance = StrToInt(arrayList[15].ToString());
                    point.ElectricalPart = StrToInt(arrayList[16].ToString());
                    pointList.Add(point);
                }
                foreach (CheckPoint checkPoint in pointList)
                {
                    CheckPoint point = checkPointDAL.SetCheckPoint(checkPoint);
                    listId.Add(point.Id);
                    bool falg = checkPointDAL.Add(point);
                    if (falg)
                    {
                        save = true;
                    }
                    else
                    {
                        foreach (string id in listId)
                        {
                            checkPointDAL.DeleteById(id);
                        }
                        return false;
                        break;
                    }
                }
            }
            //最后如果全部倒入成功则返回true
            if (save)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 日排班导入
        /// </summary>
        /// <returns></returns>
        [HttpPost("RosteringImport")]
        public string RosteringImport()
        {
            try
            {


                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }







        /// <summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static double GetHourDifference(DateTime? startTime, DateTime endTime)
        {
            DateTime t1 = DateTime.Parse(endTime.ToString());
            DateTime t2 = DateTime.Parse(startTime.ToString());
            //两个时间相减 。默认得到的是两个的时间差  格式如：365.10:35:25 
            TimeSpan t3 = t1 - t2;
            //将这个天数转换成小时, 返回值是double类型的  
            double getHours = t3.TotalHours;
            return getHours;
        }


        /// <summary>
        /// long 类型时间戳转为 字符串时间
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static string ToTime(long TimeStamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return startTime.AddTicks(TimeStamp * 10000).ToString("yyyy-MM-dd HH:mm:ss");
        }


        public static DateTime StrToDateTime(string var)
        {
            DateTime dateTime;
            DateTime.TryParse(var, out dateTime);
            return dateTime;
        }

        public static int StrToInt(string var)
        {
            int value;
            int.TryParse(var.Trim(), out value);
            return value;
        }

        public static double StrToDouble(string var)
        {
            double value;
            double.TryParse(var.Trim(), out value);
            return value;
        }

        public static decimal StrToDecimal(string var)
        {
            decimal value;
            decimal.TryParse(var.Trim(), out value);
            return value;
        }
    }
}
