namespace Hnc.iGC.Web 
{
    public class DeviceRepair
    {
        //主键ID
        public string? Id { get; set; }
        
        //设备名称
        public string DeviceName { get; set; }
        
        //设备类型
        public string DeviceType { get; set; }
        
        //设备编号
        public string? DeviceNumber { get; set; }
        
        //设备型号
        public string? DeviceModel { get; set; }

        //采购日期
        public DateTime PurchaseDate { get; set; }
        //使用年限
        public int DurableYars { get; set; }
        
        //维修开始时间
        public DateTime StartTime { get; set; }
        
        //维修结束时间
        public DateTime EndTime { get; set; }
        
        //维修原因
        public string? reason { get; set; }
        
        //维修负责人
        public string? RepairPersonnel { get; set; }
        
        //维修状态
        public int RepairState { get; set; }
        
        //维修时长
        public double RepairDuration { get; set; }
        
        //维修金额
        public decimal RepairCost { get; set; }
        
        public DateTime CreateTime { get; set; }
        
        public DateTime UpdateTime { get; set; }
    }
}