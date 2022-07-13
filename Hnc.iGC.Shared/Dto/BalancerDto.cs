namespace Hnc.iGC
{
    /// <summary>
    /// 动平衡仪
    /// </summary>
    public class BalancerDto : BaseDto
    {
        /// <summary>
        /// 动平衡自动运行准备好信号
        /// </summary>
        public bool BalanceReadyOK { get; set; }

        /// <summary>
        /// 动平衡报警
        /// </summary>
        public bool BalanceERROR { get; set; }

        /// <summary>
        /// 动平衡安全门开启磁环开关
        /// </summary>
        public bool BalanceISafetyGateOpen { get; set; }

        /// <summary>
        /// 动平衡工作中信号
        /// </summary>
        public bool BalanceWorking { get; set; }

        /// <summary>
        /// 动平衡安全门关闭磁环开关
        /// </summary>
        public bool BalanceISafetyGateClose { get; set; }

        /// <summary>
        /// 动平衡急停信号
        /// </summary>
        public bool BalanceiEmergencyStop { get; set; }

        /// <summary>
        /// 动平衡装料许可信号（允许上料）
        /// </summary>
        public bool BalanceAllowLoadPiece { get; set; }

        /// <summary>
        /// 动平衡卸料许可信号（允许取料）
        /// </summary>
        public bool BalanceAllowDownPiece { get; set; }

        /// <summary>
        /// 机器人卸料完成信号
        /// </summary>
        public bool RobotUnloadingDone { get; set; }

        /// <summary>
        /// 机器人装料完成信号
        /// </summary>
        public bool RobotLoadingDone { get; set; }

        /// <summary>
        /// 机器人急停信号
        /// </summary>
        public bool RobotiEmergencyStop { get; set; }

        /// <summary>
        /// 测量转速
        /// </summary>
        public double Speed_D20D0 { get; set; }

        /// <summary>
        /// 静初测量值（历史数据记录）
        /// </summary>
        public double QuietAmountDataPS { get; set; }

        /// <summary>
        /// 静初测角度（历史数据记录）
        /// </summary>
        public double QuietAngleDataPS { get; set; }

        /// <summary>
        /// 静复测量值（历史数据记录）
        /// </summary>
        public double QuietPreAmountDataPS { get; set; }

        /// <summary>
        /// 静复测角度（历史数据记录）
        /// </summary>
        public double QuietPreAngleDataPS { get; set; }

        /// <summary>
        /// 静允许不平衡量（g）
        /// </summary>
        public double FMTol { get; set; }

        /// <summary>
        /// 总合格
        /// </summary>
        public bool OK_D20X4416 { get; set; }

        /// <summary>
        /// 总不合格
        /// </summary>
        public bool NG_D20X4417 { get; set; }

        /// <summary>
        /// 工作方式
        /// </summary>
        public int OperationMode_D20X4418 { get; set; }

        /// <summary>
        /// 静平衡
        /// </summary>
        public bool QuietInterface_D20X4419 { get; set; }

        /// <summary>
        /// 单机/联机
        /// </summary>
        public int OnLine_D20W342 { get; set; }

        /// <summary>
        /// SN到期
        /// </summary>
        public bool SNExpire_D20X4607 { get; set; }

        /// <summary>
        /// 刀具寿命
        /// </summary>
        public int ToolLife_D20W332 { get; set; }

        /// <summary>
        /// 刀具剩余寿命
        /// </summary>
        public int ToolResidualLife_D20W322 { get; set; }
    }
}
