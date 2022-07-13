using System.ComponentModel.DataAnnotations;

namespace Hnc.iGC.Web.Options
{
    public class MQTTOptions
    {
        public const string MQTT = nameof(MQTT);

        public string IP { get; set; } = "127.0.0.1";

        [Range(0, 65535)]
        public int Port { get; set; } = 1883;

        public bool EnableMock { get; set; }

        public string RR_NcLink_Id { get; set; } = "";

        public string Register { get; set; } = "D_32_0_12;";
    }
}
