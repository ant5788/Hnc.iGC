using System.Text.Json.Serialization;
namespace Hnc.iGC.Web 
{
    public class CutterTotal
    {
        public CutterTotal()
        {
            
        }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }

        [JsonPropertyName("cutter_number")]
        public int CutterNumber { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }
        
        [JsonPropertyName("use_duration")]
        public double UseDuration { get; set; }

        [JsonPropertyName("create_time")]
        public DateTime CreateTime { get; set; }
    }
}