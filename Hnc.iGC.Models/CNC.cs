using System;
using System.Collections.Generic;

namespace Hnc.iGC.Models
{
    public class CNC : Device
    {
        /// <summary>
        /// 运行状态
        /// </summary>
        public string RunState { get; set; }

        /// <summary>
        /// 工作模式
        /// </summary>
        public string WorkMode { get; set; }

        /// <summary>
        /// 急停状态
        /// </summary>
        public bool Emergency { get; set; }

        /// <summary>
        /// 报警状态
        /// </summary>
        public bool Alarm { get; set; }

        /// <summary>
        /// 当前程序名
        /// </summary>
        public string CurrentProgramName { get; set; }

        /// <summary>
        /// 当前程序号
        /// </summary>
        public int CurrentProgramNumber { get; set; }

        /// <summary>
        /// 当前程序行号
        /// </summary>
        public int CurrentProgramLineNumber { get; set; }

        /// <summary>
        /// 当前程序内容
        /// </summary>
        public string CurrentProgramContent { get; set; }

        /// <summary>
        /// 当前刀号
        /// </summary>
        public int CurrentCutterNumber { get; set; }

        /// <summary>
        /// 进给速度 [F] mm/min
        /// </summary>
        public double FeedSpeed { get; set; }

        /// <summary>
        /// 进给速度单位
        /// </summary>
        public string FeedSpeedUnit { get; set; }

        /// <summary>
        /// 进给倍率，例如100代表100%
        /// </summary>
        public int FeedOverride { get; set; }

        /// <summary>
        /// 主轴转速 [S] r/min
        /// </summary>
        public double SpindleSpeed { get; set; }

        /// <summary>
        /// 主轴转速单位
        /// </summary>
        public string SpindleSpeedUnit { get; set; }

        /// <summary>
        /// 主轴倍率，例如100代表100%
        /// </summary>
        public int SpindleOverride { get; set; }

        /// <summary>
        /// 主轴负载
        /// </summary>
        public double SpindleLoad { get; set; }

        /// <summary>
        /// 快移倍率，例如100代表100%
        /// </summary>
        public int RapidOverride { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime? SystemTime { get; set; }

        /// <summary>
        /// 工件数量
        /// </summary>
        public long PartsCount { get; set; }

        /// <summary>
        /// 工件总数
        /// </summary>
        public long PartsTotal { get; set; }

        /// <summary>
        /// 工件需求数量
        /// </summary>
        public long PartsRequired { get; set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        public TimeSpan? TimePowerOn { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        public TimeSpan? TimeOperating { get; set; }

        /// <summary>
        /// 切削时间
        /// </summary>
        public TimeSpan? TimeCutting { get; set; }

        /// <summary>
        /// 循环时间
        /// </summary>
        public TimeSpan? TimeCycle { get; set; }

        /// <summary>
        /// 报警信息
        /// </summary>
        public IList<AlarmMessage> AlarmMessages { get; set; } = new List<AlarmMessage>();

        /// <summary>
        /// 主轴数据
        /// </summary>
        public IList<Spindle> Spindles { get; set; } = new List<Spindle>();

        /// <summary>
        /// 伺服轴数据
        /// </summary>
        public IList<Axis> Axes { get; set; } = new List<Axis>();

        /// <summary>
        /// 程序列表
        /// </summary>
        public IList<File> Files { get; set; } = new List<File>();

        /// <summary>
        /// 刀具信息
        /// </summary>
        public IList<CutterInfo> CutterInfos { get; set; } = new List<CutterInfo>();

        /// <summary>
        /// 寄存器
        /// </summary>
        //public IDictionary<string, IDictionary<int, byte>> Registers { get; set; } = new Dictionary<string, IDictionary<int, byte>>();


        /// <summary>
        /// 报警信息
        /// </summary>
        public class AlarmMessage : Entity
        {
            /// <summary>
            /// 报警号
            /// </summary>
            public string Number { get; set; }

            /// <summary>
            /// 报警内容
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// 报警开始时间
            /// </summary>
            public DateTime? StartAt { get; set; }

            /// <summary>
            /// 报警结束时间
            /// </summary>
            public DateTime? EndAt { get; set; }
        }

        /// <summary>
        /// 轴
        /// </summary>
        public class Axis : Entity
        {
            /// <summary>
            /// 轴名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 绝对坐标
            /// </summary>
            public double Absolute { get; set; }

            /// <summary>
            /// 相对坐标
            /// </summary>
            public double Relative { get; set; }

            /// <summary>
            /// 机械坐标
            /// </summary>
            public double Machine { get; set; }

            /// <summary>
            /// 剩余移动量
            /// </summary>
            public double Distance { get; set; }

            /// <summary>
            /// 伺服负载
            /// </summary>
            public double Load { get; set; }
        }

        /// <summary>
        /// 刀偏信息
        /// </summary>
        public class CutterInfo : Entity
        {
            /// <summary>
            /// 刀偏号
            /// </summary>
            public int Number { get; set; }

            /// <summary>
            /// 长度形状补偿
            /// </summary>
            public double LengthSharpOffset { get; set; }

            /// <summary>
            /// 长度磨损补偿
            /// </summary>
            public double LengthWearOffset { get; set; }

            /// <summary>
            /// 半径形状补偿
            /// </summary>
            public double RadiusSharpOffset { get; set; }

            /// <summary>
            /// 半径磨损补偿
            /// </summary>
            public double RadiusWearOffset { get; set; }
        }

        /// <summary>
        /// 主轴
        /// </summary>
        public class Spindle : Entity
        {
            /// <summary>
            /// 速度
            /// </summary>
            public double Speed { get; set; }

            /// <summary>
            /// 负载
            /// </summary>
            public double Load { get; set; }
        }

        public override string ToString() => $"{IP}|{Name}|{Description}|{DeviceId}";

    }
}
