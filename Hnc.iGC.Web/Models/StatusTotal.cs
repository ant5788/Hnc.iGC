namespace Hnc.iGC.Web
{
    public class StatusTotal
    {
        //主键ID
        public string Id { get; set; }
        //设备ID
        public string? DeviceId { get; set;}

        //设备名称
        public string? DeviceName { get; set; }

        //设备状态
        public int DeviceStatus { get; set; }

        //开始时间
        public DateTime StartTime { get; set; }

        //结束时间
        public DateTime EndTime { get; set; }

        //用时
        public double Duration { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 当前程序号
        /// </summary>
        public int CurrentProgramNumber { get; set; }

        /// <summary>
        /// 当前程序名称
        /// </summary>
        public string? CurrentProgramName { get; set; }
    }
}
