namespace Hnc.iGC.Models
{
    public abstract class Device : Entity, IDevice
    {
        public string DeviceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Protocal { get; set; }

        public string IP { get; set; }

        public ushort Port { get; set; }

    }
}
