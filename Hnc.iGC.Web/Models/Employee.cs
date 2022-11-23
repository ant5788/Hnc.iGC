namespace Hnc.iGC.Web
{
    /// <summary>
    /// 员工信息表
    /// </summary>
    public class Employee
    {
        //主键ID
        public string Id { get; set; }
        //编号
        public string? Number { get; set; }

        //姓名
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks{ get; set; }

       
    }

}