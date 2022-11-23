namespace Hnc.iGC
{
    /// <summary>
    /// 温箱
    /// </summary>
    public class TemperBoxDto : BaseDto
    {
        /// <summary>
        /// 运行状态，例如：停机、运行、保持
        /// </summary>
        public string? RunState { get; set; }
        // 控制模式
        public string? ControlMode { get; set; }
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
        public bool Alarmstate { get; set; }
        // 报警信息
        public string? AlarmData { get; set; }

        // 设定时间
        public string? RunTime_Set { get; set; }
        // 已运行时间
        public string? RunTime_Cur { get; set; }
        // 步设定时间
        public string? StepTime_Set { get; set; }
        // 步运行时间
        public string? StepTime_Cur { get; set; }
        // 设定循环数
        public string? Cycle_Set { get; set; }
        // 已完成循环数
        public string? Cycle_Cur { get; set; }
        // 设定总段数
        public string? Section_Set { get; set; }
        // 已完成段数
        public string? Section_Cur { get; set; }
        // 设定段循环数
        public string? Cycle_Section_Set { get; set; }
        // 已运行段循环数
        public string? Cycle_Section_Cur { get; set; }
        // 总步数
        public string? StepNum_Set { get; set; }
        // 当前步数
        public string? StepNum_Cur { get; set; }
        // DI状态(16Bit)
        public string? DI { get; set; }
        // DO状态(16Bit)
        public string? DO { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
