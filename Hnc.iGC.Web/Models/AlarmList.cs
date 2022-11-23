namespace Hnc.iGC.Web
{
    /// <summary>
    /// 报警信息
    /// </summary>
    public class AlarmList
    {
        public string Id { get; set; }

        public string DeviceName { get; set; }

        public string DeviceId { get; set; }
        /// <summary>
        /// 报警号
        /// </summary>
        public string? AlarmNumber { get; set; }

        /// <summary>
        /// 报警内容
        /// </summary>
        public string? AlarmMessage { get; set; }

        /// <summary>
        /// 报警开始时间
        /// </summary>
        public DateTime? StartAt { get; set; }

        /// <summary>
        /// 报警结束时间
        /// </summary>
        public DateTime? EndAt { get; set; }
        public DateTime? CreateTime { get; set; }
    }

}