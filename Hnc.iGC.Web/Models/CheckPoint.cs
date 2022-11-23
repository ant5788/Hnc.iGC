namespace Hnc.iGC.Web
{
    public class CheckPoint
    {
        //主键ID
        public string? Id { get; set; } 

        //编号
        public string? Number { get; set; } 
        //操作者
        public string? Operator { get; set; } 
        //设备名称
        public string? DeviceName { get; set; } 
        //设备编号
        public string? DeviceNumber { get; set; } 
        
        //设备型号
        public string DeviceModel { get; set; }

        //零部件是否齐全
        public int SpareParts { get; set; }

        //液压，润滑，冷却系统
        public int Liquid { get; set; }

        //气压
        public int Pressure { get; set; }

        //各操作手柄
        public int Handle { get; set; }

        //安全装置
        public int SafetyDevices { get; set; }

        //各仪表指示压力
        public int InstrumentPressure { get; set; }

        //各散热风扇及滤网
        public int FanScreen { get; set; }

        //各部驱动电机
        public int DriveMotor { get; set; }

        //漏油、漏气、漏水
        public int LeakageOilGasWater { get; set; }

        //主轴及回转传动机构
        public int PrincipalAxis { get; set; }

        //机床外表
        public int Appearance { get; set; }

        //电器部分
        public int ElectricalPart { get; set; } 
       
        //创建时间
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
   
    }
}
