namespace Hnc.iGC.Web
{
    public class Outsourcing
    {
        //主键ID
        public string? Id;
        // 委外日期
        public DateTime OutsourceDate;
        //返回日期
        public DateTime ReturnDate;
        //加工数量
        public int Number;
        //加工零件
        public string? Components;
        //加工质量
        public string? Quality;

        public DateTime CreateTime;
        public DateTime UpdateTime;
    }
}