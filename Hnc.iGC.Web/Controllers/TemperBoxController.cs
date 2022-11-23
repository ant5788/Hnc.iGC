using AutoMapper;
using Hnc.iGC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Hnc.iGC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperBoxController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TemperBoxController> logger;

        public LaboratoryDeviceDAL laboratoryDeviceDAL = new LaboratoryDeviceDAL();
        public TemperStateTotalDAL temperStateTotalDAL = new TemperStateTotalDAL();
        public TemperBoxDtoHisDAL temperBoxDtoHisDAL = new TemperBoxDtoHisDAL();
        public TemperHistoricalRangeDAL historicalRangeDAL = new TemperHistoricalRangeDAL();
        public DeviceListDAL deviceListDAL = new DeviceListDAL();
        public CalibrationPlanDAL calibrationPlanDAL = new CalibrationPlanDAL();

        public TemperBoxController(ApplicationDbContext context, IMapper mapper, ILogger<TemperBoxController> logger)
        {
            _context = context;
            _mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// 读取温箱数据
        /// </summary>
        /// <returns>设备Dto</returns>
        [HttpGet]
        public async Task<List<TemperBoxDto>> GetTemperBoxs()
        {
            try
            {
                var dData = await _context.TemperBoxs.Where(p => p.CreationTime < DateTime.Now.AddHours(-1)).ToListAsync();
                _context.TemperBoxs.RemoveRange(dData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "删除一小时前的温箱数据出错");
            }

            var ids = await _context.TemperBoxs.Select(s => s.DeviceId).Distinct().ToListAsync();
            var entities = ids.Select(deviceId =>
                    _context.TemperBoxs
                       .Where(p => p.DeviceId == deviceId)
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefault())
                    .OrderBy(o => o?.Name).ToList();
            List<TemperBoxDto> listTemperBox = _mapper.Map<List<TemperBoxDto>>(entities);
            return listTemperBox;
        }


        /// <summary>
        /// 读取温箱数据
        /// </summary>
        /// <param name="deviceId">设备Id</param>
        /// <returns>设备Dto</returns>
        [HttpGet("{deviceId}")]
        public async Task<ActionResult<TemperBoxDto>> GetTemperBox(string deviceId)
        {
            var entity = await _context.TemperBoxs
                       .Where(p => p.DeviceId == deviceId)
                       .OrderByDescending(p => p.CreationTime)
                       .FirstOrDefaultAsync();
            return _mapper.Map<TemperBoxDto>(entity);
        }

        /// <summary>
        /// 创建或更新温箱数据（覆盖）
        /// </summary>
        /// <param name="dto">设备Dto</param>
        /// <returns>设备Dto</returns>
        [HttpPost]
        public async Task<ActionResult<TemperBoxDto>> PostTemperBox(TemperBoxDto dto)
        {
            var entity = await _context.TemperBoxs
                  .OrderByDescending(t => t.CreationTime)
                  .FirstOrDefaultAsync(t => t.DeviceId == dto.DeviceId);
            entity = _mapper.Map<TemperBox>(dto);
            _context.TemperBoxs.Add(entity);

            await _context.SaveChangesAsync();
            //统计状态
            SetStateProcess(dto);
            if (dto.State != 0)
            {
                //处理保存历史数据
                //SetHistoryData(dto);
                //保存
            }
            //历史查询条件
            //SetHistoryRange(dto);
            if (deviceListDAL.getOneById(dto.DeviceId))
            {
                deviceListDAL.Add(deviceListDAL.SetDeviceList(dto));
                laboratoryDeviceDAL.Add(laboratoryDeviceDAL.SetLaboratoryDevice(dto));
            }
            return CreatedAtAction(nameof(GetTemperBox), new { deviceId = dto.DeviceId }, dto);

        }

        /// <summary>
        /// 删除温箱
        /// </summary>
        /// <param name="deviceId">设备Id</param>
        /// <returns></returns>
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteTemperBox(string deviceId)
        {
            var entities = await _context.TemperBoxs
                       .Where(p => p.DeviceId == deviceId)
                       .ToListAsync();

            if (entities == null)
            {
                return NotFound();
            }

            _context.TemperBoxs.RemoveRange(entities);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// 设置保存历史数据查询标识及范围
        /// </summary>
        public void SetHistoryRange(TemperBoxDto dto)
        {
            //查詢结束时间是否为空的数据。
            List<TemperHistoricalRange> rangeList = historicalRangeDAL.GetListByDate(dto.DeviceId, DateTime.Now);
            if (rangeList == null && dto.State != 0)
            {
                TemperHistoricalRange range = historicalRangeDAL.SetTemperHistoricalRange(dto);
                string makeStr = DateTime.Now.ToString("yyyyMMdd") + "000";
                range.Make = SetMakt(makeStr);
                range.State = dto.State;
                historicalRangeDAL.Add(range);
            }
            if (dto.State == 0 && rangeList != null)
            {
                foreach (TemperHistoricalRange range in rangeList)
                {
                    if (range.EndTime == null && range.State != dto.State)
                    {
                        Console.WriteLine("=====================");
                        range.EndTime = DateTime.Now;
                        range.State = 0;
                        historicalRangeDAL.Update(range);
                    }
                }
            }
            if (dto.State != 0 && rangeList.First().State == 0)
            {
                Console.WriteLine("======" + rangeList.First().Make);
                TemperHistoricalRange range = rangeList.First();
                string makeStr = range.Make;
                TemperHistoricalRange range1 = historicalRangeDAL.SetTemperHistoricalRange(dto);
                range1.Make = SetMakt(makeStr);
                range1.State = dto.State;
                historicalRangeDAL.Add(range1);
            }
        }

        /// <summary>
        /// 按照时间生成规定自增的编码
        /// </summary>
        /// <param name="strId"></param>
        /// <returns></returns>
        public string SetMakt(string strId)
        {
            string strID = strId; //传入最新的编号
            string strDate = strID.Substring(0, 8);
            Console.WriteLine(strDate);
            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            string strNum = "";

            if (strDate.Equals(nowDate))
            {
                int num = Int32.Parse(strID.Substring(strID.Length - 3, 3));
                num += 1;
                strNum = num.ToString("000");
            }
            else
            {
                strNum = "001";
            }
            string newStrID = nowDate + strNum;
            return new string(newStrID);
        }


        /// <summary>
        /// 处理保存历史数据
        /// </summary>
        /// <param name="dto"></param>
        public void SetHistoryData(TemperBoxDto dto)
        {
            try
            {
                //首先删除24个月之前的数据
                bool flag = temperBoxDtoHisDAL.DeleteBy24MonthsAgo();
                /* if (!flag)
                 {
                     logger.LogError("删除24个月之前数据失败");
                 }*/
                TemperBoxDtoHis temperBoxDtoHis = temperBoxDtoHisDAL.SetTemperBoxHis(dto);
                temperBoxDtoHis.DeviceId = dto.DeviceId;
                bool flag2 = temperBoxDtoHisDAL.Add(temperBoxDtoHis);
                if (!flag2)
                {
                    logger.LogError($"{DateTime.Now}:保存温箱历史数据失败");
                }
            }
            catch
            (Exception ex)
            {
                logger.LogError($"{DateTime.Now}:保存温箱历史数据失败异常：{ex}");
            }
        }






        private static int temperState = 9999;
        /// <summary>
        /// 温箱状态统计
        /// </summary>
        /// <param name="dto"></param>
        public void SetStateProcess(TemperBoxDto dto)
        {
            try
            {
                //如何保存设置状态 及状态改变改变时长
                TemperStateTotal temperStateTotal = temperStateTotalDAL.GetModelByParameters(dto.DeviceId, temperState);
                if (temperStateTotal == null && dto.State != temperState)
                {
                    Console.WriteLine("temperState====" + temperState);
                    Console.WriteLine("dto.State======" + dto.State);
                    temperStateTotalDAL.Add(temperStateTotalDAL.SetTemperStateTotal(dto));
                    temperState = dto.State;
                }
                else
                {
                    if (dto.State != temperState)
                    {
                        temperStateTotal.EndTime = DateTime.Now;
                        temperStateTotal.Duration = GetHourDifference(temperStateTotal.StartTime, temperStateTotal.EndTime);
                        temperStateTotal.UpdateTime = DateTime.Now;
                        temperStateTotalDAL.Update(temperStateTotal);
                        //新增一条最新的状态数据
                        temperStateTotalDAL.Add(temperStateTotalDAL.SetTemperStateTotal(dto));
                        temperState = dto.State;
                    }
                    //如果报警状态为true
                    TemperStateTotal temper = temperStateTotalDAL.GetModelByParameters(dto.DeviceId, 99);
                    if (dto.Alarmstate == true && temper == null)
                    {
                        TemperStateTotal temperState = temperStateTotalDAL.SetTemperStateTotal(dto);
                        temperState.State = 99;
                        temperState.RunState = "报警";
                        temperStateTotalDAL.Add(temperState);
                    }
                    if (dto.Alarmstate == false && temper != null)
                    {
                        temper.EndTime = DateTime.Now;
                        temper.Duration = GetHourDifference(temper.StartTime, temper.EndTime);
                        temper.UpdateTime = DateTime.Now;
                        temperStateTotalDAL.Update(temper);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"保存设置温箱状态数据异常：{ex}");
            }




        }


        /// <summary>
        /// 获取所有温箱实时状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTemperBoxRealTimeState")]
        public string GetTemperBoxRealTimeState()
        {
            try
            {
                Task<List<TemperBoxDto>> task = GetTemperBoxs();
                List<TemperBoxDto> listTemperBox = task.Result;
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                foreach (TemperBoxDto dto in listTemperBox)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    map.Add("name", dto.Name + "-" + dto.Description);
                    map.Add("id", dto.DeviceId);
                    if (dto.Alarmstate == true)
                    {
                        map.Add("state", "异常");
                    }
                    else
                    {
                        map.Add("state", dto.RunState);
                    }
                    listMap.Add(map);
                }
                return ResultUtil.Success(listMap, "查询温箱实时状态成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询温箱实时状态异常：{ex}");
                return ResultUtil.Fail($"查询温箱实时状态异常:{ex}");
            }
        }

        /// <summary>
        /// 获取实时报警信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRealTimeAlarm")]
        public string GetRealTimeAlarm()
        {
            try
            {
                Task<List<TemperBoxDto>> task = GetTemperBoxs();
                List<TemperBoxDto> listTemperBox = task.Result;
                List<Dictionary<string, object>> listDictionary = new List<Dictionary<string, object>>();

                foreach (TemperBoxDto dto in listTemperBox)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    if (dto.Alarmstate == true)
                    {
                        map.Add("name", dto.Name + dto.Description);
                        map.Add("message", dto.AlarmData);
                        listDictionary.Add(map);
                    }
                }
                return ResultUtil.Success(listDictionary, "查询实时报警信息成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询实时报警信息异常：{ex}");
                return ResultUtil.Fail($"查询实时报警信息异常:{ex}");
            }
        }

        /// <summary>
        /// 六大区域个区域设备数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSixDeviceNumber")]
        public string GetSixAreaDeviceNumber()
        {
            try
            {
                List<Dictionary<string, object>> list = laboratoryDeviceDAL.GetSixAreaDeviceNumber();
                return ResultUtil.Success(list, "查询六大区域设备数量成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询六大区域设备数量异常：{ex}");
                return ResultUtil.Fail($"查询六大区域设备数量异常:{ex}");
            }
        }
        /// <summary>
        /// 查询六大区域
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSixArea")]
        public string GetSixArea()
        {
            try
            {
                List<AreaList> areaList = laboratoryDeviceDAL.GetSixArea();
                return ResultUtil.Success(areaList, "查询六大区域成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询六大区域异常：{ex}");
                return ResultUtil.Fail($"查询六大区域异常:{ex}");
            }
        }



        /// <summary>
        /// 添加虚拟设备
        /// </summary>
        /// <param name="laboratory"></param>
        /// <returns></returns>
        [HttpPost("AddLaboratoryDevice")]
        public string AddLaboratoryDevice(LaboratoryDevice laboratory)
        {
            try
            {
                bool flag = laboratoryDeviceDAL.Add(laboratoryDeviceDAL.SetLaboratoryDevice(laboratory));
                if (flag)
                {
                    return ResultUtil.Success("增加实验室设备数据成功");
                }
                return ResultUtil.Fail("增加实验室设备数据成功失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"增加实验室设备数据成功异常：{ex}");
                return ResultUtil.Fail($"增加实验室设备数据成功异常:{ex}");
            }
        }

        /// <summary>
        /// 删除实验室设备数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteById")]
        public string DeleteById(string id)
        {
            try
            {
                bool flag = laboratoryDeviceDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除实验室设备数据成功");
                }

                return ResultUtil.Fail("删除实验室设备数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"删除实验室设备数据异常：{ex}");
                return ResultUtil.Fail($"删除实验室设备数据异常:{ex}");
            }
        }


        /// <summary>
        /// 修改实验室设备数据
        /// </summary>
        /// <param name="laboratoryDevice"></param>
        /// <returns></returns>
        [HttpPost("UpdateLaboratory")]
        public string UpdateLaboratory(LaboratoryDevice laboratoryDevice)
        {
            try
            {
                laboratoryDevice.UpdateTime = DateTime.Now;
                bool flag = laboratoryDeviceDAL.Update(laboratoryDevice);
                if (flag)
                {
                    return ResultUtil.Success("修改实验室设备数据成功");
                }
                return ResultUtil.Fail("修改实验室设备数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"修改实验室设备数据异常：{ex}");
                return ResultUtil.Fail($"修改实验室设备数据异常:{ex}");
            }
        }

        /// <summary>
        /// 分页查询数据列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetAllDeviceList")]
        public string GetAllDeviceList(int pageNo, int pageSize)
        {
            try
            {
                List<LaboratoryDevice> LaboratoryList = laboratoryDeviceDAL.GetAllDeviceList(pageNo, pageSize);
                //TODO 查询总数
                int count = laboratoryDeviceDAL.GetRecordCount("");
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("count", count);
                result.Add("list", LaboratoryList);
                return ResultUtil.Success(result, "查询实验室设备列表数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询实验室设备列表数据异常：{ex}");
                return ResultUtil.Fail($"查询实验室设备列表数据异常:{ex}");
            }
        }

        /// <summary>
        /// 设备详情查看
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetLaboratoryDevice")]
        public string GetLaboratoryDevice(string id)
        {
            try
            {
                LaboratoryDevice Laboratory = laboratoryDeviceDAL.GetOneById(id);
                return ResultUtil.Success(Laboratory, "查询实验室设备详情数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询实验室设备详情数据异常：{ex}");
                return ResultUtil.Fail($"查询实验室设备详情数据异常:{ex}");
            }
        }


        /// <summary>
        /// 六大区域设备列表 参数 区域ID 
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        [HttpGet("GetAreaDeviceListByAraeCode")]
        public string GetAreaDeviceListByAraeCode(int areaCode)
        {
            try
            {
                List<LaboratoryDevice> laboratoriesList = laboratoryDeviceDAL.GetAreaDeviceListByAraeCode(areaCode);
                return ResultUtil.Success(laboratoriesList, "查询实验室区域设备列表数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询实验室区域设备列表数据异常：{ex}");
                return ResultUtil.Success($"查询实验室区域设备列表数据异常：{ex}");
            }
        }

        /// <summary>
        /// 查询温箱个状态时长
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet("GetStatusDuration")]
        public string GetStatusDuration(string deviceId)
        {
            try
            {
                List<TemperStatusDuration> listMap = laboratoryDeviceDAL.GetStatusDuration(deviceId);
                return ResultUtil.Success(listMap, "查询温箱设备个状态时长成功");
            }
            catch (Exception ex)
            {
                return ResultUtil.Success($"查询实验室区域设备列表数据异常：{ex}");
            }
        }

        /// <summary>
        /// 温箱效率分析
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("GetStatusEfficiency")]
        public string GetStatusEfficiency(string deviceId)
        {
            try
            {
                List<TemperStatusDuration> listMap = laboratoryDeviceDAL.GetStatusDuration(deviceId);
                double durationTotal = 0.00;
                foreach (TemperStatusDuration duration in listMap)
                {
                    durationTotal += duration.duration;
                }
                List<TemperStatusDuration> durationList = new List<TemperStatusDuration>();
                foreach (TemperStatusDuration duration in listMap)
                {
                    TemperStatusDuration statusDuration = new TemperStatusDuration();
                    decimal Ratio = Math.Round(decimal.Parse((duration.duration / durationTotal).ToString("0.000")), 2) * 100;
                    statusDuration.RunState = duration.RunState;
                    statusDuration.state = duration.state;
                    statusDuration.Efficiency = Ratio;
                    durationList.Add(statusDuration);
                }
                return ResultUtil.Success(durationList, "查询温箱设备状态效率成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询温箱设备状态效率异常：{ex}");
                return ResultUtil.Fail($"查询温箱设备状态效率异常：{ex}");
            }
        }

        /// <summary>
        /// 实时曲线数据
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("RealTimeCurve")]
        public string RealTimeCurve(string deviceId)
        {
            try
            {
                var dData = _context.TemperBoxs.Where(p => p.CreationTime > DateTime.Now.AddSeconds(-60)).Where(p => p.DeviceId == deviceId).ToListAsync();
                List<TemperBox> temperBoxes = dData.Result;
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                foreach (TemperBox temperBox in temperBoxes)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    map.Add("time", temperBox.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    map.Add("PV_TMP", temperBox.PV_TMP);
                    listMap.Add(map);
                }
                return ResultUtil.Success(listMap, "查询实时温度曲线成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询实时温度曲线异常：{ex}");
                return ResultUtil.Fail($"查询实时温度曲线异常：{ex}");
            }
        }





        /// <summary>
        /// 查询温度历史曲线  查询什么时间段内的数据
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="make"></param>
        /// <returns></returns>
        [HttpGet("HistoricalCurve")]
        public string HistoricalCurve(string deviceId, string make)
        {
            try
            {
                //通过设备ID和标识查询 需要查询的时间范围的详细数据
                TemperHistoricalRange range = historicalRangeDAL.GetQueryTime(deviceId, make);
                if(range == null)
                {
                    return ResultUtil.Fail($"查询历史温度曲线时间范围为空");
                }
                string startTime = DateToString(range.StartTime);
                string endTime = DateToString((DateTime)range.EndTime);

                List<TemperBoxDtoHis> boxDtoHisList = temperBoxDtoHisDAL.HistoricalCurve(deviceId, startTime, endTime);
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                foreach (TemperBoxDtoHis temperBoxDtoHis in boxDtoHisList)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    map.Add("time", DateToString((DateTime)temperBoxDtoHis.CreateTime));
                    map.Add("TMP", temperBoxDtoHis.PV_TMP);
                    listMap.Add(map);
                }
                return ResultUtil.Success(listMap, "查询温度历史曲线成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询历史温度曲线异常：{ex}");
                return ResultUtil.Fail($"查询历史温度曲线异常：{ex}");
            }
        }

        /// <summary>
        /// 历史曲线显示条件显示 显示标识码
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("HistoryQueryCriteria")]
        public string HistoryQueryCriteria(string deviceId)
        {
            try
            {
                //查询此设备先所有产生的编码标识
                List<TemperHistoricalRange> rangeList = historicalRangeDAL.GetMakeListByDeviceId(deviceId);
                List<string> listMake = new List<string>();
                foreach (TemperHistoricalRange range in rangeList)
                {
                    listMake.Add(range.Make);
                }
                return ResultUtil.Success(listMake, "查询曲线历史查询条件成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询曲线历史查询条件异常：{ex}");
                return ResultUtil.Fail($"查询曲线历史查询条件异常：{ex}");
            }
        }



        /// <summary>
        /// 电磁振动台 电磁兼容 噪声等设备各个时间统计  虚拟数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("VibrationTable")]
        public string VibrationTable()
        {
            try
            {
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                Dictionary<string, object> map1 = new Dictionary<string, object>();
                map1.Add("time", "开机时间");
                map1.Add("value", "3824");

                Dictionary<string, object> map2 = new Dictionary<string, object>();
                map2.Add("time", "停机时间");
                map2.Add("value", "00");
                Dictionary<string, object> map3 = new Dictionary<string, object>();
                map3.Add("time", "待机时间");
                map3.Add("value", "324");
                Dictionary<string, object> map4 = new Dictionary<string, object>();
                map4.Add("time", "运行时间");
                map4.Add("value", "3500");
                Dictionary<string, object> map5 = new Dictionary<string, object>();
                map5.Add("time", "操作时间");
                map5.Add("value", "68");
                Dictionary<string, object> map6 = new Dictionary<string, object>();
                map6.Add("time", "报警时间");
                map6.Add("value", "00");
                listMap.Add(map1);
                listMap.Add(map2);
                listMap.Add(map3);
                listMap.Add(map4);
                listMap.Add(map5);
                listMap.Add(map6);
                return ResultUtil.Success(listMap, "查询电磁振动台各个时间统计成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询电磁振动台各个时间统计异常：{ex}");
                return ResultUtil.Fail($"查询电磁振动台各个时间统计异常：{ex}");
            }
        }


        /// <summary>
        /// 电磁振动台 电磁兼容 噪声等设备状态比率  虚拟数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("Ratio")]
        public string Ratio()
        {
            try
            {
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                Dictionary<string, object> map1 = new Dictionary<string, object>();
                Dictionary<string, object> map2 = new Dictionary<string, object>();
                Dictionary<string, object> map3 = new Dictionary<string, object>();
                Dictionary<string, object> map4 = new Dictionary<string, object>();
                map1.Add("name", "开机率");
                map1.Add("value", 99.98);
                map2.Add("name", "设备开动率");
                map2.Add("value", 56);
                map3.Add("name", "生产开动率");
                map3.Add("value", 56.5);
                map4.Add("name", "报警率");
                map4.Add("value", 00);
                listMap.Add(map1);
                listMap.Add(map2);
                listMap.Add(map3);
                listMap.Add(map4);
                return ResultUtil.Success(listMap, "设备状态比率查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询设备状态比率异常：{ex}");
                return ResultUtil.Fail($"查询设备状态比率异常：{ex}");
            }
        }

        /// <summary>
        /// 设备曲线图  虚拟数据使用温箱的实时曲线的方法
        /// </summary>
        /// <returns></returns>
        [HttpGet("DeviceRealTimeCurve")]
        public string DeviceRealTimeCurve(string deviceId)
        {
            try
            {
                var dData = _context.TemperBoxs.Where(p => p.CreationTime < DateTime.Now.AddSeconds(-10)).Where(p => p.DeviceId == deviceId).ToListAsync();
                List<TemperBox> temperBoxes = dData.Result;
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                for (int i = 0; i < 10; i++)
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();
                    map.Add("time", "2022-08-12");
                    map.Add("value", 10 + i);
                    listMap.Add(map);
                }
                return ResultUtil.Success(listMap, "设备实施曲线查询成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"设备实时曲线查询异常：{ex}");
                return ResultUtil.Fail("设备实时曲线查询异常");
            }
        }


        /// <summary>
        /// 设备历史曲线图 电磁振动台 电磁兼容  噪声
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet("DeviceHistoricalCurve")]
        public string DeviceHistoricalCurve()
        {
            try
            {
                List<Dictionary<string, object>> listMap = new List<Dictionary<string, object>>();
                Dictionary<string, object> map1 = new Dictionary<string, object>();
                Dictionary<string, object> map2 = new Dictionary<string, object>();
                Dictionary<string, object> map3 = new Dictionary<string, object>();
                Dictionary<string, object> map4 = new Dictionary<string, object>();
                Dictionary<string, object> map5 = new Dictionary<string, object>();
                Dictionary<string, object> map6 = new Dictionary<string, object>();
                Dictionary<string, object> map7 = new Dictionary<string, object>();
                Dictionary<string, object> map8 = new Dictionary<string, object>();

                map1.Add("time", "2022-07-09 12:36:05");
                map1.Add("value", 50);
                map2.Add("time", "2022-07-10 15:28:05");
                map2.Add("value", 60);
                map3.Add("time", "2022-07-11 05:14:05");
                map3.Add("value", 20);
                map4.Add("time", "2022-07-13 10:03:05");
                map4.Add("value", 15);
                map5.Add("time", "2022-07-15 15:06:05");
                map5.Add("value", 45);
                map6.Add("time", "2022-07-20 14:45:05");
                map6.Add("value", 79);
                map7.Add("time", "2022-07-24 15:36:05");
                map7.Add("value", 90);
                map8.Add("time", "2022-07-27 17:25:05");
                map8.Add("value", 67);

                listMap.Add(map1);
                listMap.Add(map2);
                listMap.Add(map3);
                listMap.Add(map4);
                listMap.Add(map5);
                listMap.Add(map6);
                listMap.Add(map7);
                listMap.Add(map8);
                return ResultUtil.Success(listMap, "查询设备历史曲线成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"设备历史曲线查询异常：{ex}");
                return ResultUtil.Fail("设备历史曲线查询异常");
            }
        }



        /// <summary>
        /// 增加校准计划
        /// </summary>
        /// <param name="calibrationPlan"></param>
        /// <returns></returns>
        [HttpPost("AddCalibrationPlan")]
        public string AddCalibrationPlan(CalibrationPlan calibrationPlan)
        {
            try
            {
                bool flag = calibrationPlanDAL.Add(calibrationPlanDAL.SetCalibrationPlan(calibrationPlan));
                if (flag)
                {
                    return ResultUtil.Success("增加校准计划数据成功");
                }
                return ResultUtil.Fail("增加校准计划数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"增加校准计划数据异常：{ex}");
                return ResultUtil.Fail("增加校准计划数据异常");
            }
        }


        /// <summary>
        /// 删除校准计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteCalibrationPlan")]
        public string DeleteCalibrationPlan(string id)
        {
            try
            {
                bool flag = calibrationPlanDAL.DeleteById(id);
                if (flag)
                {
                    return ResultUtil.Success("删除校准计划数据成功");
                }
                return ResultUtil.Fail("删除校准计划数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"删除校准计划数据异常：{ex}");
                return ResultUtil.Fail("删除校准计划数据异常");
            }
        }

        /// <summary>
        /// 修改校准计划
        /// </summary>
        /// <param name="calibrationPlan"></param>
        /// <returns></returns>
        [HttpPost("UpdateCalibrationPlan")]
        public string UpdateCalibrationPlan(CalibrationPlan calibrationPlan)
        {
            try
            {
                calibrationPlan.UpdateTime = DateTime.Now;
                bool flag = calibrationPlanDAL.Update(calibrationPlan);
                if (flag)
                {
                    return ResultUtil.Success("修改校准计划数据成功");
                }
                return ResultUtil.Fail("修改校准计划数据失败");
            }
            catch (Exception ex)
            {
                logger.LogError($"修改校准计划数据异常：{ex}");
                return ResultUtil.Fail("修改校准计划数据异常："+ex);
            }
        }

        /// <summary>
        /// 分页查询列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCalibrationPlanList")]
        public string GetCalibrationPlanList(int pageNo, int pageSize)
        {
            try
            {
                List<CalibrationPlan> planList = calibrationPlanDAL.GetList("1=1 order by create_time desc limit " + (pageNo - 1) * pageSize + ", " + pageSize);
                int count = calibrationPlanDAL.GetRecordCount("");
                Dictionary<string, object> map = new Dictionary<string, object>();
                map.Add("count", count);
                map.Add("list", planList);
                return ResultUtil.Success(map, "查询校准计划列表数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询校准计划列表数据异常：{ex}");
                return ResultUtil.Fail("查询校准计划列表数据");
            }
        }

        /// <summary>
        /// 获取校准计划详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetCalibrationPlanById")]
        public string GetCalibrationPlanById(string id)
        {
            try
            {
                CalibrationPlan calibrationPlan = calibrationPlanDAL.GetById(id);

                return ResultUtil.Success(calibrationPlan, "查询校准计划详细数据成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"查询校准计划详细数据异常：{ex}");
                return ResultUtil.Fail("查询校准计划详细数据异常");
            }
        }

        /// <summary>
        /// 盐雾试验箱当前任务列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SaltSprayCurrentTask(string id)
        {
            try
            {
                return ResultUtil.Success(null, "盐雾试验箱当前任务列表成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"盐雾试验箱当前任务列表异常：{ex}");
                return ResultUtil.Fail("盐雾试验箱当前任务列表异常");
            }
        }

        /// <summary>
        /// 历史任务列表
        /// </summary>
        /// <returns></returns>
        public string SaltSprayHIistoryTask() 
        {
            return ResultUtil.Success(null, "盐雾试验箱历史任务列表成功");
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

        /// <summary>
        /// DateTime 转 string 字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string DateToString(DateTime dateTime)
        {
            try
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }


        }
    }
}
