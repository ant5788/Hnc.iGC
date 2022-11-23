namespace Hnc.iGC.Web
{
    public class CalibrationPlan
    {
        //主键ID
        public string? Id { get; set; }

        //校准计划名称
        public string? PlanName { get; set; }
        //设备名称
        public string? DeviceName { get; set; }
        //设备编号
        public string? DeviceNumber { get; set; }
        //设备型号
        public string? DeviceType { get; set; }
        //资产编号
        public string? AssetNumber { get; set; }
        //校准计划开始时间
        public DateTime StartTime {get;set; }
        //校准计划结束时间
        public DateTime EndTime {get;set; }

        public int PlanState { get; set; }

        public DateTime UpdateTime { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
