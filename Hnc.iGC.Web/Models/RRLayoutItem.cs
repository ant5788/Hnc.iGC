using System.Text.Json.Serialization;

namespace Hnc.iGC.Web.Models
{
    public class RRLayoutItem
    {
        public RRLayoutItem(string name, string? displayText = null)
        {
            Name = name;
            DisplayText = displayText ?? name;
        }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = "button";

        [JsonPropertyName("displayText")]
        public string DisplayText { get; set; }

        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; } = "#d3ebef";

        [JsonPropertyName("color")]
        public string Color { get; set; } = "#000000";

        [JsonPropertyName("width")]
        public int Width { get; set; } = 50;

        [JsonPropertyName("height")]
        public int Height { get; set; } = 50;

        [JsonPropertyName("left")]
        public int Left { get; set; } = 100;

        [JsonPropertyName("top")]
        public int Top { get; set; } = 30;

        [JsonPropertyName("lightOn")]
        public string? LightOn { get; set; }

        [JsonPropertyName("lightOff")]
        public string? LightOff { get; set; }

        [JsonPropertyName("keydown")]
        public string? KeyDown { get; set; }

        [JsonPropertyName("keyup")]
        public string? KeyUp { get; set; }
    }
}
