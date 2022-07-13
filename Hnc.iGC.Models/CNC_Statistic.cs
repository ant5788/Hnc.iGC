using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hnc.iGC.Models
{
    public class CNC_Statistic : Entity
    {
        public enum States : uint { 开机 = 0, 加工, 待机, 离线, 报警 }

        /// <summary>
        /// 当日状态
        /// </summary>
        public List<(TimeSpan time, States state)> StateToday { get; } = new List<(TimeSpan time, States state)>();

        /// <summary>
        /// 生产统计（周度），饼图
        /// </summary>
        public Dictionary<States, TimeSpan> LastWeekPie { get; } = new Dictionary<States, TimeSpan>()
        {
            { States.开机, TimeSpan.Zero },
            { States.加工, TimeSpan.Zero },
            { States.待机, TimeSpan.Zero },
            { States.离线, TimeSpan.Zero },
            { States.报警, TimeSpan.Zero },
        };

        /// <summary>
        /// 生产统计（周度），柱状图
        /// </summary>
        public List<(DateTime time, uint count)> LastWeekBar { get; } = new List<(DateTime time, uint count)>();

        /// <summary>
        /// 今日产量
        /// </summary>
        public uint ProductCountToday { get; set; }

        /// <summary>
        /// 获取1970-01-01至dateTime的秒数
        /// unix时间戳是从1970年1月1日（UTC/GMT的午夜）开始所经过的秒数，不考虑闰秒。
        /// </summary>
        public static long GetUnixTimestamp(DateTime dateTime)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)).ToUnixTimeSeconds();
            //DateTime dt1970 = new(1970, 1, 1, 0, 0, 0, 0);
            //return (dateTime.Ticks - dt1970.Ticks) / 10000 / 1000;
        }

        public byte[] Get_iGSData_Bytes()
        {
            List<byte> list = new();

            void AddItem(uint value)
            {
                var bytes = BitConverter.GetBytes(value);
                //Array.Reverse(bytes);
                for (int i = 0; i < bytes.Length; i++)
                {
                    list.Add(bytes[i]);
                }
            }

            #region HEAD
            // Bit8 fileFlag[4]
            var fileFlagBytes = Encoding.ASCII.GetBytes("IGSD");
            for (int i = 0; i < fileFlagBytes.Length; i++)
            {
                list.Add(fileFlagBytes[i]);
            }
            // Bit32 version
            AddItem(10000);
            // Bit32 fileInfoAddr
            AddItem(0);
            // Bit32 fileDataAddr
            AddItem(0);
            // Bit32 verifyType
            AddItem(0);
            // Bit32 verifyCode[8]
            for (int i = 0; i < 8; i++)
            {
                AddItem(0);
            }
            // Bit32 reserve[32]
            for (int i = 0; i < 32; i++)
            {
                AddItem(0);
            }
            #endregion

            AddItem((uint)StateToday.Count);
            for (int i = 0; i < StateToday.Count; i++)
            {
                AddItem((uint)StateToday[i].time.TotalMinutes);
            }
            for (int i = 0; i < StateToday.Count; i++)
            {
                AddItem((uint)StateToday[i].state);
            }

            AddItem((uint)LastWeekPie[States.开机].TotalMinutes);
            AddItem((uint)LastWeekPie[States.加工].TotalMinutes);
            AddItem((uint)LastWeekPie[States.待机].TotalMinutes);
            AddItem((uint)LastWeekPie[States.离线].TotalMinutes);
            AddItem((uint)LastWeekPie[States.报警].TotalMinutes);

            for (int i = 0; i < LastWeekBar.Count; i++)
            {
                AddItem((uint)GetUnixTimestamp(LastWeekBar[i].time));
            }
            for (int i = 0; i < LastWeekBar.Count; i++)
            {
                AddItem(LastWeekBar[i].count);
            }

            AddItem(ProductCountToday);
            return list.ToArray();
        }

        public static byte[] Get_macInfo_Bytes(string deviceId, string deviceType, string deviceName)
        {
            List<byte> list = new();

            void AddItem(uint value)
            {
                var bytes = BitConverter.GetBytes(value);
                //Array.Reverse(bytes);
                for (int i = 0; i < bytes.Length; i++)
                {
                    list.Add(bytes[i]);
                }
            }

            #region HEAD
            // Bit8 fileFlag[4]
            var fileFlagBytes = Encoding.ASCII.GetBytes("IGSD");
            for (int i = 0; i < fileFlagBytes.Length; i++)
            {
                list.Add(fileFlagBytes[i]);
            }
            // Bit32 version
            AddItem(10000);
            // Bit32 fileInfoAddr
            AddItem(0);
            // Bit32 fileDataAddr
            AddItem(0);
            // Bit32 verifyType
            AddItem(0);
            // Bit32 verifyCode[8]
            for (int i = 0; i < 8; i++)
            {
                AddItem(0);
            }
            // Bit32 reserve[32]
            for (int i = 0; i < 32; i++)
            {
                AddItem(0);
            }
            #endregion

            // Bit8 m_devNo[DEV_NAME_LEN=10]
            var devNoBytes = Encoding.ASCII.GetBytes(deviceId?.PadRight(10, '\0') ?? new string('\0', 10));
            for (int i = 0; i < 10; i++)
            {
                list.Add(devNoBytes[i]);
            }
            // Bit8 m_devType[DEV_NAME_LEN=10]
            var devTypeBytes = Encoding.ASCII.GetBytes(deviceType?.PadRight(10, '\0') ?? new string('\0', 10));
            for (int i = 0; i < 10; i++)
            {
                list.Add(devTypeBytes[i]);
            }
            // Bit8 m_devName[DEV_NAME_LEN=10]
            var devNameBytes = Encoding.ASCII.GetBytes(deviceName?.PadRight(10, '\0') ?? new string('\0', 10));
            for (int i = 0; i < 10; i++)
            {
                list.Add(devNameBytes[i]);
            }

            return list.ToArray();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(StateToday.Count.ToString());
            for (int i = 0; i < StateToday.Count; i++)
            {
                sb.AppendLine(((uint)StateToday[i].time.TotalMinutes).ToString());
            }
            for (int i = 0; i < StateToday.Count; i++)
            {
                sb.AppendLine(((uint)StateToday[i].state).ToString());
            }

            sb.AppendLine(((uint)LastWeekPie[States.开机].TotalMinutes).ToString());
            sb.AppendLine(((uint)LastWeekPie[States.加工].TotalMinutes).ToString());
            sb.AppendLine(((uint)LastWeekPie[States.待机].TotalMinutes).ToString());
            sb.AppendLine(((uint)LastWeekPie[States.离线].TotalMinutes).ToString());
            sb.AppendLine(((uint)LastWeekPie[States.报警].TotalMinutes).ToString());

            for (int i = 0; i < LastWeekBar.Count; i++)
            {
                sb.AppendLine(((uint)GetUnixTimestamp(LastWeekBar[i].time)).ToString());
            }
            for (int i = 0; i < LastWeekBar.Count; i++)
            {
                sb.AppendLine(LastWeekBar[i].count.ToString());
            }
            sb.AppendLine(ProductCountToday.ToString());

            return sb.ToString();
        }
    }
}
