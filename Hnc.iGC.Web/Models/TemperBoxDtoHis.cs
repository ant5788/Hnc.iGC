namespace Hnc.iGC
{
    /// <summary>
    /// 温箱
    /// </summary>
    public class TemperBoxDtoHis
    {
        public string Id { get; set; }
        
        //设备ID
        public string DeviceId { get; set; }
        /// <summary>
        /// 运行状态，例如：停机、运行、保持
        /// </summary>
        public string? RunState { get; set; }

        public int State { get; set; }
        // 温度PV
        public float PV_TMP { get; set; }
        // 湿度PV
        public float PV_HUM { get; set; }
        // 温度SV
        public float SV_TMP { get; set; }
        // 湿度SV
        public float SV_HUM { get; set; }
        /// <summary>
        /// 报警状态
        /// </summary>
        public int Alarmstate { get; set; }
        // 报警信息
        public string? AlarmData { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
