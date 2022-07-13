namespace Hnc.iGC.Web
{
    public class DeviceMaintain
    {
        //主键ID
        public string? Id { get; set; }
        //设备编号
        public string? DeviceNumber { get; set; }
        //设备名称
        public string? DeviceName { get; set; }
        //设备型号
        public string? DeviceModel { get; set; }
        //设备类型
        public string? DeviceType { get; set; }
        //设备状态
        public string? DeviceState { get; set; }
        //保养内容
        public string? Content { get; set; }
        //保养周期
        public string? Cycle { get; set; }
        //设备分类
        public string? DeviceClassification { get; set; }
        //责任人
        public string? PersonLiable { get; set; }
        //使用单位
        public string? UserDep { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
