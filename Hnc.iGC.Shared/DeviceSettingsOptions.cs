namespace Hnc.iGC
{
    public class DeviceSettingsOptions
    {
        public const string DeviceSettings = nameof(DeviceSettings);

        public IList<DeviceSetting> Devices { get; set; } = new List<DeviceSetting>();

        public class DeviceSetting
        {
            public bool EnableDataCollect { get; set; }

            public int IntervalInMilliseconds { get; set; }

            public string DeviceId { get; set; } = Guid.NewGuid().ToString();

            public string Name { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            public string Protocal { get; set; } = string.Empty;

            public string IP { get; set; } = string.Empty;

            public ushort Port { get; set; } = ushort.MinValue;

            public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
