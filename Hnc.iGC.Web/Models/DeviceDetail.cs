namespace Hnc.iGC.Web
{
    public class DeviceDetail
    {
        public string? Id { get; set; }
        //设备ID
        public string? DeviceId { get; set; }
        //设备型号
        public string? DeviceType { get; set; }
        //设备名称
        public string? DeviceName { get; set; }
       //设备编号
        public string? DeviceNumber { get; set; }
        public string? AssetsNumber { get; set; }

        //采集日期
        public DateTime AcquisitionDate { get; set; }
        
        //班次
        public string Shift { get; set; }
        //设备保有时间
        public int RetentionTime { get; set; }
        
        //设备运动时间
        public double RunningTime { get; set; }

        //嫁动率
        public string CropRate { get; set; }
        
        //备注
        public string remarks { get; set; }

        public string? DevicePhoto { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }   
    }
}
