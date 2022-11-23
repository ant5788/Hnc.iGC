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

        //采购日期
        public DateTime PurchaseDate { get; set; }

        //使用年限
        public int DurableYears { get; set; }

        //保养内容
        public string? Content { get; set; }
        //保养周期
        public int Cycle { get; set; }

        //上次保养时间
        public DateTime LastTime { get; set; }
        
        //计划保养时间
        public DateTime PlannedTime { get; set; }
        
        //实际保养时间
        public DateTime ActualTime { get; set; }
       
        //责任人
        public string PersonLiable { get; set; }
       
        //提前预警时间
        public int EarlyWarningTime { get; set; }
   
        //维保状态
        public int MaintainState { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
