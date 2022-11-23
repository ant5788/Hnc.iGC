using System.Text.Json.Serialization;
namespace Hnc.iGC.Web 
{
    public class AreaList
    {
        public string Id { get; set; }
        public string AreaName { get; set; }

        public int AreaCode { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}