namespace Hnc.iGC.Web
{
    /// <summary>
    /// 报警信息
    /// </summary>
    public class AlarmMessageDto
    {
        public string DeviceName { get; set; }
        /// <summary>
        /// 报警号
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// 报警内容
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 报警开始时间
        /// </summary>
        public DateTime? StartAt { get; set; }

        /// <summary>
        /// 报警结束时间
        /// </summary>
        public DateTime? EndAt { get; set; }
    }

}