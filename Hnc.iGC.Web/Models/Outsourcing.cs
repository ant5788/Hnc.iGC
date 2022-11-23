namespace Hnc.iGC.Web
{
    public class Outsourcing
    {
        //主键ID
        public string? Id { get; set; }

        //委外订单
        public string? OutsourcingOrder { get; set; }

        //供应商
        public string? Supplier { get; set; }
        //物料编码
        public string? ItemNumber { get; set; }
        // 物料描述 
        public string? ItemDescription { get; set; }
        //数量
        public int Number { get; set; }

        //已入库数量
        public int ReceivedQuantity { get; set; }

        //合格数量
        public int QualifiedQuantity { get; set; }
        
        //待检数量
        public int ToBeInspected { get; set; }
        
        //委外申请创建日期
        public DateTime OutsourcingTime { get; set; }
        
        //实际要求到货日期
        public DateTime ActualRequiredDate { get; set; }
        
        //合同签订日期
        public DateTime ContractSigningDate { get; set; }

        //合同要求到货日期
        public DateTime ContractArrivalDate { get; set; }
 
        //委外出库时间
        public DateTime DeliveryTime { get; set; }
 
        //提前预警时间
        public int EarlyWarning { get; set; }
        
        //实际到货日期
        public DateTime ActualArrivalDate { get; set; }
        
        //采购组
        public string? ProcurementTeam { get; set; }
 
        //备注
        public string? Remarks { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}