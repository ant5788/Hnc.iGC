using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Hnc.iGC.Models
{
    public class AppFile
    {
        public int Id { get; set; }

        public byte[]? Content { get; set; }

        [Display(Name = "Directory Name")]
        public string? DirectoryName { get; set; }

        [Display(Name = "File Name")]
        public string? UntrustedName { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Size { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DisplayFormat(DataFormatString = "{0:G}")]
        public DateTime UploadDT { get; set; }
    }

    public class AirDashboard_Cnc : Entity
    {
        public enum SystemModel : byte { _808D = 0, _818B = 1, _848D = 2, fanuc = 3 }
        public enum WorkMode : byte { 自动 = 0, 手动 = 1 }
        public enum WorkStatus : byte { 运行 = 0, 空闲 = 1, 进给保持 = 2 }

        public static readonly List<byte> FEED_OVERRIDE_List = new() { 0, 1, 2, 4, 6, 8, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 95, 100, 105, 110, 120 };

        public static readonly List<byte> SPINDLE_OVERRIDE_List = new() { 50, 60, 70, 80, 90, 100, 110, 120 };

        public string NcLinkName { get; set; }

        public SystemModel System_Model { get; set; } = new SystemModel();
        public string Machine_Name { get; set; }
        public WorkMode Work_Mode { get; set; } = WorkMode.自动;
        public WorkStatus Work_Status { get; set; } = WorkStatus.运行;

        //public string Current_Program { get; set; }
        public byte FEED_OVERRIDE { get; set; } = 100;
        public byte SPINDLE_OVERRIDE { get; set; } = 100;
        public ushort Process_Count { get; set; } = 0;
        public byte Progress { get; set; } = 0;
        public TimeSpan Remaining => new(Remaining_Hour, Remaining_Minute, Remaining_Second);
        public byte Remaining_Hour { get; set; } = 0;
        public byte Remaining_Minute { get; set; } = 0;
        public byte Remaining_Second { get; set; } = 0;

        public string ProgramName { get; set; } = "";

        public byte[] GetProgramNameBytes()
        {
            var data = Enumerable.Repeat((byte)0, 10).ToArray();
            var name = ProgramName?.Split('/').Last() ?? "";
            if (name.Length > 10)
            {
                name = name.Substring(0, 10);
            }
            var nameBytes = Encoding.ASCII.GetBytes(name);
            for (int i = 0; i < data.Length && i < nameBytes.Length; i++)
            {
                data[i] = nameBytes[i];
            }
            return data;
        }

        public ushort ProgramLineNumber { get; set; } = 1;

        public float Current { get; set; } = new float();

        public ushort AlarmNumber { get; set; } = new ushort();

        public uint LoopTime { get; set; } = new uint();

        public byte[] GetDataBytes() => new byte[12] {
            (byte)System_Model,
            byte.Parse(Machine_Name.Substring(Machine_Name.Length-2,2)),
            (byte)Work_Mode,
            (byte)Work_Status,
            FEED_OVERRIDE,
            SPINDLE_OVERRIDE,
            (byte)(Process_Count >> 8),
            (byte)(Process_Count & 255),
            Progress,
            Remaining_Hour,
            Remaining_Minute,
            Remaining_Second
        };

        /// <summary>
        /// Length: 25
        /// </summary>
        /// <returns></returns>
        public byte[] GetDataBytes2()
        {
            var data = new byte[25];
            data[0] = (byte)Work_Status;
            var nameBytes = GetProgramNameBytes();
            for (int i = 0; i < 10; i++)
            {
                data[i + 1] = nameBytes[i];
            }
            data[11] = (byte)(ProgramLineNumber >> 8);
            data[12] = (byte)(ProgramLineNumber & 255);
            var currentBytes = BitConverter.GetBytes(Current);
            for (int i = 0; i < currentBytes.Length; i++)
            {
                data[13 + i] = currentBytes[i];
            }
            data[17] = (byte)(Process_Count >> 8);
            data[18] = (byte)(Process_Count & 255);
            data[19] = (byte)(AlarmNumber >> 8);
            data[20] = (byte)(AlarmNumber & 255);
            var loopTimeBytes = BitConverter.GetBytes(LoopTime);
            for (int i = 0; i < loopTimeBytes.Length; i++)
            {
                data[21 + i] = loopTimeBytes[i];
            }
            return data;
        }

        #region CncData


        /// <summary>
        /// G代码
        /// </summary>
        public List<AppFile> GCode { get; set; } = new List<AppFile>();

        /// <summary>
        /// 测量记录文件
        /// </summary>
        public List<AppFile> MeasurementRecordFile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 工艺文件
        /// </summary>
        public List<AppFile> ProcessFile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 固定循环
        /// </summary>
        public List<AppFile> FixedLoop { get; set; } = new List<AppFile>();

        /// <summary>
        /// 用户宏配置文件
        /// </summary>
        public List<AppFile> UserMacroProfile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 用户变量值文件
        /// </summary>
        public List<AppFile> UserVariableValueFile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 示波器数据
        /// </summary>
        public List<AppFile> OscilloscopeData { get; set; } = new List<AppFile>();

        /// <summary>
        /// 日志数据
        /// </summary>
        public List<AppFile> LogData { get; set; } = new List<AppFile>();

        /// <summary>
        /// 刀具信息文件
        /// </summary>
        public List<AppFile> ToolInformationFile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 误差补偿文件
        /// </summary>
        public List<AppFile> ErrorCompensationFile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 车刀测量数据文件
        /// </summary>
        public List<AppFile> KnifeMeasurementDataFile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 故障录像配置文件
        /// </summary>
        public List<AppFile> FaultRecordingProfile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 故障录像文件
        /// </summary>
        public List<AppFile> FaultRecordingFile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 参数配置文件
        /// </summary>
        public List<AppFile> ParameterProfile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 多语言配置
        /// </summary>
        public List<AppFile> MultiLanguageConfiguration { get; set; } = new List<AppFile>();

        /// <summary>
        /// 菜单配置文件
        /// </summary>
        public List<AppFile> MenuProfile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 热键配置文件
        /// </summary>
        public List<AppFile> HotkeyProfile { get; set; } = new List<AppFile>();


        /// <summary>
        /// MCP面板配置文件
        /// </summary>
        public List<AppFile> MCP_PanelProfile { get; set; } = new List<AppFile>();

        /// <summary>
        /// 第二加工代码
        /// </summary>
        public List<AppFile> TheSecondProcessingCode { get; set; } = new List<AppFile>();

        #endregion
    }
}
