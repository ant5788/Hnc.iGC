namespace Hnc.iGC.Web
{
    public class CheckPoint
    {
        //主键ID
        public string? Id { get; set; } 
        //设备名称
        public string? DeviceName { get; set; } 
        //设备编号
        public string? DeviceNumber { get; set; } 
        //资产编号
        public string? AssetNumber { get; set; } 
        //检点类型
        public int Type  { get; set; } 
        //检点状态
        public int State { get; set; } 
        //检点开始时间
        public DateTime StartTime { get; set; } 
        //检点结束时间
        public DateTime EndTime { get; set; } 
        //检点详细情况
        public string? Details { get; set; } 
        //检点人员
        public string? Inspector { get; set; }
        //创建时间
        public DateTime CreateTime { get; set; }
        //修改时间
        public DateTime UpdateTime { get; set; }
    }
}
