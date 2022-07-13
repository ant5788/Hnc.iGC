namespace Hnc.iGC.Web
{
    public class DeviceList
    {
        //主键ID
        public string Id { get; set; }
        //设备ID
        public string? DeviceId { get; set; }

        //设备名称
        public string? DeviceName { get; set; }

        public string? Description { get; set; }

        public string? Ip { get; set; }

        public string? Port { get; set; }
    }
}
