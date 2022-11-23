namespace Hnc.iGC.Web
{
    public class TemperStateTotal
    {
        public string Id { get; set; }

        public string DeviceId { get; set; }
        public string DeviceName { get; set; }

        public string RunState { get; set; }

        public int State { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double Duration { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
