using AutoMapper;
using Hnc.iGC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using System.Text.Json;

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


        //全局机床状态
        private static short RunState = 9999;
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

        public CNCController(ApplicationDbContext context, IMapper mapper, ILogger<CNCController> logger)
        {
            _context = context;
            _mapper = mapper;
            this.logger = logger;
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
            }
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
        /// 设置状态数据
        /// </summary>
        /// <param name="dto"></param>
        private void SetStateProcess(CNCDto dto)
        {
            //判断当前运行状态是否为9999 这默认保存一条当前状态的数据
            if (RunState == 9999)
            {
                //新增保存数据
                statusTotalDAL.Add(statusTotalDAL.SetStatusTotal(dto));
                RunState = dto.State;
            }
            else
            {
                //如果状态不为9999 查询通过查询runstate和当前结束时间和市场为空的数据。
                StatusTotal status = statusTotalDAL.GetModelByParameters(dto.DeviceId, RunState);
                status.EndTime = DateTime.Now;
                status.Duration = GetHourDifference(status.StartTime, status.EndTime);
                statusTotalDAL.Update(status);
                //最新的状态和全局状态不相同 新增一条数据
                if (RunState != dto.State)
                {
                    statusTotalDAL.Add(statusTotalDAL.SetStatusTotal(dto));
                    RunState = dto.State;
                }
            }
            //保存报警状态及时长
            if (dto.Alarm && dto.Alarm != AlarmStatus)
            {
                StatusTotal status = statusTotalDAL.GetModelByParameters(dto.DeviceId, 99);
                if (null == status)
                {
                    StatusTotal statusTotal = statusTotalDAL.SetStatusTotal(dto);
                    statusTotal.DeviceStatus = 99;
                    statusTotalDAL.Add(statusTotal);
                }
                else
                {
                    status.EndTime = DateTime.Now;
                    status.Duration = GetHourDifference(status.StartTime, status.EndTime);
                    statusTotalDAL.Update(status);
                }
            }
            //保存急停状态机时长
            if (dto.Emergency && dto.Emergency != emergencyStatus)
            {
                StatusTotal status = statusTotalDAL.GetModelByParameters(dto.DeviceId, 98);
                if (null == status)
                {
                    StatusTotal statusTotal = statusTotalDAL.SetStatusTotal(dto);
                    statusTotal.DeviceStatus = 98;
                    statusTotalDAL.Add(statusTotal);
                }
                else
                {
                    status.EndTime = DateTime.Now;
                    status.Duration = GetHourDifference(status.StartTime, status.EndTime);
                    statusTotalDAL.Update(status);
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
            CutterTotal cutterTotal = cutterTotalDAL.GetModelByParameters(dto.DeviceId, CutterNumber);
            if (null == cutterTotal)
            {
                //新增保存数据
                cutterTotalDAL.Add(cutterTotalDAL.SetCutterTotal(dto));
                CutterNumber = dto.CurrentCutterNumber;
            }
            else
            {
                //如果刀号不是上次的刀号并且机床状态是循环启动状态
                if (dto.CurrentCutterNumber != CutterNumber && dto.State == 0)
                {
                    //修改上一个刀号的使用结束时间及使用市场
                    cutterTotal.EndTime = DateTime.Now;
                    cutterTotal.UseDuration = cutterTotal.UseDuration + GetHourDifference(cutterTotal.StartTime, cutterTotal.EndTime);
                    cutterTotalDAL.Update(cutterTotal);
                    //查询当前刀号 并且更改当前刀号的开始使用时间
                    CutterTotal cutter = cutterTotalDAL.GetModelByParameters(dto.DeviceId, dto.CurrentCutterNumber);
                    cutter.StartTime = DateTime.Now;
                    cutterTotalDAL.Update(cutter);
                    CutterNumber = dto.CurrentCutterNumber;
                }
                //判断刀号没有改变 但是机床当前状态是循环启动状态 并且 全局机床状态是其他状态，
                //即机床状态是由其他状态变为循环启动状态说明是在加工（手动切刀的操作场景下）
                if (CutterNumber == dto.CurrentCutterNumber && dto.State != 0)
                {
                    CutterTotal cutter = cutterTotalDAL.GetModelByParameters(dto.DeviceId, dto.CurrentCutterNumber);
                    cutter.EndTime = DateTime.Now;
                    cutter.UseDuration = cutterTotal.UseDuration + GetHourDifference(cutterTotal.StartTime, cutterTotal.EndTime);
                    cutterTotalDAL.Update(cutterTotal);
                    CutterNumber = dto.CurrentCutterNumber;
                }
                if (CutterNumber == dto.CurrentCutterNumber && dto.State == 0 && RunState != dto.State)
                {
                    CutterTotal cutter = cutterTotalDAL.GetModelByParameters(dto.DeviceId, dto.CurrentCutterNumber);
                    cutter.StartTime = DateTime.Now;
                    CutterNumber = dto.CurrentCutterNumber;
                }
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
        /// 4.0所有设备设备效率分析 多台设备运行日志 已测试（访问返回数据）
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
                    //TODO 运行状态 状态表示未明确
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

                var count = ReSetDuration + StopDuration + HoldDuration + ExceptionDuration + CycleStartDurtion;

                var ExerciseRate = Math.Round(decimal.Parse((count / totalDuration).ToString("0.000")), 2) * 100;
                var MarriageRate = Math.Round(decimal.Parse((CycleStartDurtion / totalDuration).ToString("0.000")), 2) * 100;
                //var Shutdown = Math.Round(decimal.Parse((ShutdownDuration / totalDuration).ToString("0.000")), 2) * 100;
                Dictionary<string, object> map = new Dictionary<string, object>
                {
                    { "totalDuration", totalDuration },
                    { "ExerciseRate", ExerciseRate },
                    { "MarriageRate", MarriageRate }
                };
                return ResultUtil.Success(map, "运动率/嫁动率查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError("设备ID:" + deviceId + "运动率/嫁动率查询查询异常:" + ex);
                return ResultUtil.Fail("运动率/嫁动率查询查询异常" + ex.ToString());
            }
        }



        






        /// <summary>
        /// 获取所有可联网机床的实时状态 	实时设备状态监控 首页1
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
                    else
                    {
                        map.Add("State", cnc.State);
                    }

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
        /// 实时报警信息 首页2
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
        /// 报警时长TOP5 首页3
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
        /// 状态分布 	设备状态分布 首页4
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
                    if (cnc.State == 0)
                    {
                        ReSet++;
                    }
                    if (cnc.State == 1)
                    {
                        Stop++;
                    }
                    if (cnc.State == 2)
                    {
                        Hold++;
                    }
                    if (cnc.State == 3)
                    {
                        Start++;
                    }
                    if (cnc.State == 4)
                    {
                        Mstr++;
                    }
                    if (cnc.State == 98)
                    {
                        Emergency++;
                    }
                    if (cnc.State == 99)
                    {
                        Alarm++;
                    }
                }
                var ReSetRate = Math.Round(decimal.Parse((ReSet / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> ReSetRateMap = new Dictionary<string, object>();
                ReSetRateMap.Add("name", "reset");
                ReSetRateMap.Add("value", ReSetRate);

                var StopRate = Math.Round(decimal.Parse((Stop / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> StoptRateMap = new Dictionary<string, object>();
                StoptRateMap.Add("name", "Stop");
                StoptRateMap.Add("value", StoptRateMap);

                var HoldRate = Math.Round(decimal.Parse((Hold / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> HoldRateMap = new Dictionary<string, object>();
                HoldRateMap.Add("name", "Hold");
                HoldRateMap.Add("value", HoldRateMap);

                var StartRate = Math.Round(decimal.Parse((Start / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> StartRateMap = new Dictionary<string, object>();
                StartRateMap.Add("name", "Start");
                StartRateMap.Add("value", StartRateMap);

                var MstrRate = Math.Round(decimal.Parse((Mstr / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> MstrRateMap = new Dictionary<string, object>();
                MstrRateMap.Add("name", "Mstr");
                MstrRateMap.Add("value", MstrRateMap);

                var EmergencyRate = Math.Round(decimal.Parse((Emergency / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> EmergencyRateMap = new Dictionary<string, object>();
                EmergencyRateMap.Add("name", "Emergency");
                EmergencyRateMap.Add("value", EmergencyRateMap);

                var AlarmRate = Math.Round(decimal.Parse((Alarm / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> AlarmRateMap = new Dictionary<string, object>();
                AlarmRateMap.Add("name", "Alarm");
                AlarmRateMap.Add("value", AlarmRateMap);
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
                return ResultUtil.Fail("查询状态分布异常：" + ex.Message);
            }
        }


        /// <summary>
        /// 查询所有设备列表 首页5
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
        /// 单台设备管理 设备详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetDeviceDetailById")]
        public string GetDeviceDetailById(string id)
        {
            try
            {
                DeviceDetail deviceDetails = deviceDetailDAL.GetDeviceDetailById(id);
                return ResultUtil.Success(deviceDetails, "查询设备列表成功");
            }
            catch (Exception ex)
            {
                logger.LogError("查询单台设备详情异常:" + ex);
                return ResultUtil.Fail("查询单台设备详情异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 单台设备看板 加工零件数目
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="mark"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public string GetMakeNumber(string deviceId, int mark, long startTime, long endTime)
        {
            try
            {
                List<Dictionary<string, object>> mapList = new List<Dictionary<string, object>>();
                //如果是按班或者是天这返回本班次或本天每小时的加工个数。
                //如果是按月或者是俺时间段这返回这个月或者是这个时间段每天的生产数量
                if (0 == mark)
                {
                    mapList = partsTotalDAL.TimeinTervalProduction(deviceId, ToTime(startTime), ToTime(endTime));
                }
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
        /// 单台设备看板 加工零件编码
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
                return ResultUtil.Success(partCode, "查询当前在正在加工的零件编码成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前在正在加工的零件编码异常:{ex}");
                return ResultUtil.Fail("查询当前在正在加工的零件编码异常" + ex.ToString());
            }
        }



        /// <summary>
        /// 单台设备看板 刀具信息展示
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("GetCutterInfo")]
        public string GetCutterInfo(string deviceId)
        {
            try
            {
                Task<List<CNCDto>> tesk = GetCNCs1();
                List<CNCDto> listCNC = tesk.Result;
                Dictionary<string, object> map = null; ;
                foreach (var cnc in listCNC)
                {
                    if (deviceId == cnc.DeviceId)
                    {
                        var CurrentCutterNumber = cnc.CurrentCutterNumber;
                        var FeedSpeed = cnc.FeedSpeed;
                        var FeedSpeedUnit = cnc.FeedSpeedUnit;
                        var FeedOverride = cnc.FeedOverride;
                        map = new Dictionary<string, object>()
                        {
                            {"CurrentCutterNumber",CurrentCutterNumber},
                            {"FeedSpeed",FeedSpeed },
                            {"FeedSpeedUnit",FeedSpeedUnit },
                            {"FeedOverride", FeedOverride}
                        };
                    }
                }
                return ResultUtil.Success("", "查询当前设备刀具信息成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前设备刀具信息异常:{ex}");
                return ResultUtil.Fail("查询当前设备刀具信息异常" + ex.ToString());
            }
        }

        /// <summary>
        /// 单台设备看板 查询主轴负载
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSpindleLoad")]
        public string GetSpindleLoad(string deviceId)
        {
            try
            {
                Task<CNCDto> task = GetCNC1(deviceId);
                CNCDto cnc = task.Result;
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
        /// 单台设备看板 加工零件百分比
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("GetPartPercentage")]
        public string GetPartPercentage(string deviceId)
        {
            try
            {

                return ResultUtil.Success("", "查询当前零件加工百分比成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前零件加工百分比异常:{ex}");
                return ResultUtil.Fail("查询当前零件加工百分比：" + ex.ToString());
            }
        }

        /// <summary>
        /// 单台设备看板 状态分布百分比
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
                ReSetRateMap.Add("name", "reset");
                ReSetRateMap.Add("value", ReSetRate);

                var StopRate = Math.Round(decimal.Parse((Stop / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> StoptRateMap = new Dictionary<string, object>();
                StoptRateMap.Add("name", "Stop");
                StoptRateMap.Add("value", StoptRateMap);

                var HoldRate = Math.Round(decimal.Parse((Hold / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> HoldRateMap = new Dictionary<string, object>();
                HoldRateMap.Add("name", "Hold");
                HoldRateMap.Add("value", HoldRateMap);

                var StartRate = Math.Round(decimal.Parse((Start / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> StartRateMap = new Dictionary<string, object>();
                StartRateMap.Add("name", "Start");
                StartRateMap.Add("value", StartRateMap);

                var MstrRate = Math.Round(decimal.Parse((Mstr / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> MstrRateMap = new Dictionary<string, object>();
                MstrRateMap.Add("name", "Mstr");
                MstrRateMap.Add("value", MstrRateMap);

                var EmergencyRate = Math.Round(decimal.Parse((Emergency / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> EmergencyRateMap = new Dictionary<string, object>();
                EmergencyRateMap.Add("name", "Emergency");
                EmergencyRateMap.Add("value", EmergencyRateMap);

                var AlarmRate = Math.Round(decimal.Parse((Alarm / count).ToString("0.000")), 2) * 100;
                Dictionary<string, object> AlarmRateMap = new Dictionary<string, object>();
                AlarmRateMap.Add("name", "Alarm");
                AlarmRateMap.Add("value", AlarmRateMap);
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                listMap.Add(ReSetRateMap);
                listMap.Add(StoptRateMap);
                listMap.Add(HoldRateMap);
                listMap.Add(StartRateMap);
                listMap.Add(MstrRateMap);
                listMap.Add(EmergencyRateMap);
                listMap.Add(AlarmRateMap);
                return ResultUtil.Success(listMap, "查询当前零件加工百分比成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询当前零件加工百分比异常:{ex}");
                return ResultUtil.Fail("查询当前零件加工百分比：" + ex.ToString());
            }
        }

        /// <summary>
        /// 获取设备档案管理列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetArchivesList")]
        public string GetArchivesList()
        {
            try
            {
                List<DeviceArchives> archiversList = archivesDAL.GetList("1=1 oeder by create_time");
                return ResultUtil.Success(archiversList, "查询设备管理列表成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备管理列表异常:{ex}");
                return ResultUtil.Fail("查询设备管理列表异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 通过ID删除一条设备档案管理的数据
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
        /// 通过ID修改数据
        /// </summary>
        /// <param name="archives"></param>
        /// <returns></returns>
        [HttpPost("UpdateArchives")]
        public string UpdateArchives(DeviceArchives archives)
        {
            try
            {
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
        /// 增加设备管理的数据
        /// </summary>
        /// <param name="archives"></param>
        /// <returns></returns>
        [HttpPost("AddArchives")]
        public string AddArchives(DeviceArchives archives)
        {
            try
            {
                Boolean flag = archivesDAL.Add(archives);
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
        /// 查询档案详情
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
        /// 获取维保计划列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMaintainList")]
        public string GetMaintainList()
        {
            try
            {
                List<DeviceMaintain> maintainsList = maintainDAL.GetList("1=1 oeder by create_time");
                return ResultUtil.Success(maintainsList, "查询设备维保列表数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备维保列表数据异常:{ex}");
                return ResultUtil.Fail("查询设备维保列表数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 增加维保
        /// </summary>
        /// <param name="maintain"></param>
        /// <returns></returns>
        [HttpPost("AddMaintain")]
        public string AddMaintain(DeviceMaintain maintain)
        {
            try
            {
                var falg = maintainDAL.Add(maintain);
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
        /// 删除数据设备维保数
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
        /// 通过ID修改数据设备维保
        /// </summary>
        /// <param name="maintain"></param>
        /// <returns></returns>
        [HttpPost("UpdateArchives")]
        public string UpdateMaintain(DeviceMaintain maintain)
        {
            try
            {
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
        /// 查询详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetArchiversById")]
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
        /// 设备检点列表查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCheckPointList")]
        public string GetCheckPointList()
        {
            try
            {
                List<CheckPoint> checkPointList = checkPointDAL.GetList("1=1 order by create_time");
                return ResultUtil.Success(checkPointList, "查询设备检点数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备检点数据异常:{ex}");
                return ResultUtil.Fail("查询设备检点数据异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 增加设备检点数据
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
        ///  修改设备检点数据
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [HttpPost("UpdateCheckPoint")]
        public string UpdateCheckPoint(CheckPoint point)
        {
            try
            {
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
        /// 删除设备检点数据
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
        /// 查询设备检点详情
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
        /// 
        /// 实时产量统计 未测试
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
        /// 5.1.2 累计产量统计 
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
        /// 添加合格率统计分数据  合格率统计分析
        /// </summary>
        /// <param name="passRate"></param>
        [HttpPost("AddPassRate")]
        public string AddPassRate(PassRate passRate)
        {
            try
            {
                Boolean flag = passRateDAL.Add(passRate);
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
        /// 查询合格率统计分析 合格率统计分析
        /// </summary>
        /// <returns></returns>
        [HttpGet("PassRateAnalysis")]
        public string PassRateAnalysis()
        {
            try
            {
                List<PassRate> passRateList = passRateDAL.GetList(" 1=1 order by create_time");
                return ResultUtil.Success(null, "查询合格率统计分析成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询合格率统计分数据异常:{ex}");
                return ResultUtil.Fail("查询合格率统计分数据异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 修改合格率统计分析 合格率统计分析
        /// </summary>
        /// <param name="passRate"></param>
        /// <returns></returns>
        public string UpdatePassRateAnalysis(PassRate passRate)
        {
            try
            {
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
        /// 获取合格率统计分析详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetPassRateById(string id) 
        {
            try 
            {
                PassRate passRate = passRateDAL.GetById(id);
                return ResultUtil.Success(passRate,"查询合格率统计分析详情成功");
            } 
            catch (Exception ex) 
            {
                logger.LogError($"查询合格率统计分析详情异常:{ex}");
                return ResultUtil.Fail("查询合格率统计分析详情异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除合格率统计分析详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// 增加外协加工进度
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
        ///查询外协加工进度
        /// </summary>
        /// <returns></returns>
        public string Outsourcing()
        {
            try
            {
                List<Outsourcing>  list = outsourcingDAL.GetList("1=1 order by create_time desc");
                return ResultUtil.Success(list, "查询外协加工进度列表成功");
            }
            catch (Exception ex)
            {
                logger.LogError("查询外协加工进度列表异常：" + ex);
                return ResultUtil.Fail("查询外协加工进度列表异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 修改外协加工进度
        /// </summary>
        /// <param name="outsourcing"></param>
        /// <returns></returns>
        public string updateOutsourcing( Outsourcing outsourcing) 
        {
            try
            {
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
        /// 通过ID查询外协加工进度详细数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetOutsourcing(string id) 
        {
            try
            {
                Outsourcing outsourcing = outsourcingDAL.GetById(id);
                return ResultUtil.Success(outsourcing,"查询外协加工进度详细信息成功");
            }
            catch (Exception ex) 
            {
                logger.LogError($"查询外协加工进度详细信息异常：{ex}");
                return ResultUtil.Fail("查询外协加工进度详细信息异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除外协加工数据通过ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                keyValuePairs.Add("Total", 15);

                Dictionary<string, object> keyValuePairs1 = new Dictionary<string, object>();
                keyValuePairs.Add("Time", "2022-05-14");
                keyValuePairs.Add("Total", 11);

                Dictionary<string, object> keyValuePairs2 = new Dictionary<string, object>();
                keyValuePairs.Add("Time", "2022-05-15");
                keyValuePairs.Add("Total", 13);


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
                    foreach (var test in test1)
                    {
                        Dictionary<string, object> resule = new Dictionary<string, object>();
                        if (daily.Key.Equals(test.Key))
                        {
                            decimal var1;
                            int a = int.Parse(test.Value.ToString());
                            var1 = a;
                            decimal var2;
                            int b = int.Parse(daily.Value.ToString());
                            var2 = b;
                            var Planned = Math.Round(decimal.Parse((var1 / var2).ToString("0.000")), 2) * 100;
                            resule.Add(test.Key, Planned);
                        }
                        else
                        {
                            resule.Add(test.Key, 0);
                        }
                        ResuleList.Add(resule);
                    }
                }
                return ResultUtil.Success(ResuleList, "计划达成率查询成功");
            }
            catch (Exception ex)
            {
                return ResultUtil.Fail("计划达成率查询异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 计划产量实际产量列表
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public string PlanList(string deviceId) 
        {
            try
            {
                //TODO 获取ERP接口计划产量数据
                string startTime = "";
                string endTime = "";
                List<Dictionary<string, object>> plannedOutput = new List<Dictionary<string, object>>();

                List<Dictionary<string, object>> ActualOutput = partsTotalDAL.CumulativeProduction(deviceId, startTime, endTime);



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

    }
}
