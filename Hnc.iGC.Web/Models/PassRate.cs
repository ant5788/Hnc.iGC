namespace Hnc.iGC.Web
{
    public class PassRate
    {
        //ID
        public string? Id { get; set; }
        //合格率统计分析编号
        public string? Number { get; set; }
        //合格率统计分析名称
        public string? Name { get; set; }
        //日期
        public DateTime Date { get; set; }
        public int FeedingNumber { get; set; }
        public int WasteNumber { get; set; }
        public string? Pass_Rate { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
