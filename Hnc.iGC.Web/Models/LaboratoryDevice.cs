namespace Hnc.iGC.Web
{
    public class LaboratoryDevice
    {
        //主键ID
        public string? Id { get; set; }

        public string? DeviceId { get; set; }

        //设备名称
        public string DeviceName { get; set; }

        //设备类型
        public string DeviceType { get; set; }
        
        //设备编号
        public string DeviceNumber { get; set; }

        //资产编号
        public string AssetNumber { get; set; }

        //归属区域
        public int Area { get; set; }

        //设备图片
        public string DevicePhoto { get; set; }

        //创建时间
        public DateTime CreateTime { get; set; }

        //修改时间
        public DateTime? UpdateTime { get; set; }
    }
}
