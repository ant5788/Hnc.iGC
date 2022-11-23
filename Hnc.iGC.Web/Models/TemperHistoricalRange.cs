namespace Hnc.iGC.Web
{
    public class TemperHistoricalRange
    {
        public string Id { get; set; }

        public string DeviceId { get; set; }

        public string Make { get; set; }

        public int State { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime CreateTime { get; set; } 
    }
}
