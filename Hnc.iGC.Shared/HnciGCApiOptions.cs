namespace Hnc.iGC
{
    public class HnciGCApiOptions
    {
        public const string HnciGCApi = nameof(HnciGCApi);

        public string BaseAddress { get; set; } = "";

        public string GetCNC { get; set; } = "";

        public string PostCNC { get; set; } = "";

        public string DeleteCNC { get; set; } = "";

        public string GetBalancer { get; set; } = "";

        public string PostBalancer { get; set; } = "";

        public string DeleteBalancer { get; set; } = "";
        public string GetTemperBox { get; set; } = "";

        public string PostTemperBox { get; set; } = "";

        public string DeleteTemperBox { get; set; } = "";
    }
}
