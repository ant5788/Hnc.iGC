using System.Text;

namespace Hnc.iGC.Models
{
    public class CNC_Neighbor : Entity
    {
        public enum WorkModes : byte { 手动 = 0, 自动 = 1, 增量, 单段, 回零, MDI }
        public enum RunStates : byte { 循环启动 = 0, 进给保持 = 1, 急停, 待机 }

        /// <summary>
        /// 系统型号, 10 byte
        /// </summary>
        public string SystemModel { get; set; } = new string(' ', 10);

        /// <summary>
        /// 设备名称, 10 byte
        /// </summary>
        public string Name { get; set; } = new string(' ', 10);

        /// <summary>
        /// 工作模式, 1 byte
        /// </summary>
        public WorkModes WorkMode { get; set; }

        /// <summary>
        /// 运行状态, 1 byte
        /// </summary>
        public RunStates RunState { get; set; }

        /// <summary>
        /// 进给速度, 4 byte
        /// </summary>
        public float FeedSpeed { get; set; }

        /// <summary>
        /// 主轴转速, 4 byte
        /// </summary>
        public float SpindleSpeed { get; set; }

        /// <summary>
        /// 主程序名, 10 byte
        /// </summary>
        public string ProgramName { get; set; } = new string(' ', 10);

        /// <summary>
        /// 加工件数, 2 byte
        /// </summary>
        public ushort PartsCount { get; set; }

        /// <summary>
        /// 运行时间（秒）, 4 byte
        /// </summary>
        public uint TimeOperating { get; set; }

        /// <summary>
        /// 稼动率, 1 byte
        /// </summary>
        public byte Utilization { get; set; }

        /// <summary>
        /// 共 47 byte
        /// </summary>
        /// <returns></returns>
        public byte[] GetDataBytes()
        {
            var data = new byte[47];

            Array.Copy(Encoding.ASCII.GetBytes(SystemModel.PadRight(10)), 0, data, 0, 10);
            Array.Copy(Encoding.ASCII.GetBytes(Name.PadRight(10)), 0, data, 10, 10);

            data[20] = (byte)WorkMode;
            data[21] = (byte)RunState;

            var bs1 = BitConverter.GetBytes(FeedSpeed);
            Array.Reverse(bs1);
            Array.Copy(bs1, 0, data, 22, 4);

            var bs2 = BitConverter.GetBytes(SpindleSpeed);
            Array.Reverse(bs2);
            Array.Copy(bs2, 0, data, 26, 4);

            Array.Copy(Encoding.ASCII.GetBytes(ProgramName.PadRight(10)), 0, data, 30, 10);

            var bs3 = BitConverter.GetBytes(PartsCount);
            Array.Reverse(bs3);
            Array.Copy(bs3, 0, data, 40, 2);

            var bs4 = BitConverter.GetBytes(TimeOperating);
            Array.Reverse(bs4);
            Array.Copy(bs4, 0, data, 42, 4);

            data[46] = Utilization;

            return data;
        }

    }
}
