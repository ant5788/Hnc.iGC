namespace Hnc.iGC.Web
{
    public class PassRate
    {
        //ID
        public string? Id { get; set; }
        //ERP指令单号
        public string? OrderNo { get; set; }
        //零件订单号
        public string? PartOrder { get; set; }

        //部件名称
        public string? PartName { get; set; }
        
        //检验数量
        public int? InspectionQuantity { get; set; }

        //不合格数量
        public int? UnqualifiedQuantity { get; set; }
        
        //不合格问题描述
        public string? Problem { get; set; }
        
        //工序
        public string? WorkingProcedure { get; set; }
       
        //原因分类1级
        public string? Cause1 { get; set; }
        
        //原因分类2级
        public string? Cause2 { get; set; }

        //责任人
        public string? PersonLiable { get; set; }

        //解决措施
        public string? Solutions { get; set; }

        //检验日期
        public DateTime InspectionDate { get; set; }

        //检验员
        public string? Inspector { get; set; }

        //备注
        public string? remarks { get; set; }
        
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
