using System;

namespace Hnc.iGC.Models
{
    public interface IDevice
    {
        Guid Id { get; set; }

        string DeviceId { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Protocal { get; set; }

        string IP { get; set; }

        ushort Port { get; set; }
    }
}