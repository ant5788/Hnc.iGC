namespace Hnc.iGC.Web
{
    /// <summary>
    /// 报警信息
    /// </summary>
    public class DictCode
    {
        public int Id { get; set; }

        //代码组
        public int CodeGroup { get; set; }

        //代码
        public int Code { get; set; }
        /// <summary>
        /// 代码信息
        /// </summary>
        public string? CodeMsg { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? remark { get; set; }

        
    }

}