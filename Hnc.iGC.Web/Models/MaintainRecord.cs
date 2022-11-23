namespace Hnc.iGC.Web
{
    public class MaintainRecord
    {
        //主键ID
        public string? Id { get; set; }
        //设备名称
        public string? DeviceName { get; set; }
        //设备编号
        public string? DeviceNumber { get; set; }
        //设备型号
        public string? DeviceModel { get; set; }

        //设备负责人
        public string? PersonInCharge { get; set; }

        //保养类别
        public string? MaintainCategory { get; set; }

        //保养周期
        public string? Cycle { get; set; }

        //保养内容
        public string? Content { get; set; }

        //外保养
        public int ExternalMaintenance { get; set; }

        //主轴 刀库
        public int AxisCutters { get; set; }

        //刀把刀套
        public int KnifeHandleHolster { get; set; }

        //润滑油
        public int Liquid { get; set; }

        //冷却液
        public int Coolant { get; set; }

        //风管气枪
        public int AirGun { get; set; }

        //电脑
        public int Computer { get; set; }

        //电控箱
        public int ElectricControl { get; set; }

        //工具箱
        public int ToolBox { get; set; }

        //保养人
        public string? Maintainer { get; set; }

        //保养时间
        public DateTime MaintanTime { get; set; }

        //保养情况确认
        public string? Confirm { get; set; }

        //确认人
        public string? ConfirmPeople { get; set; }

        //确认时间
        public DateTime ConfirmTime { get; set; }
        //创建时间
        public DateTime CreateTime { get; set; }
        //
        public DateTime UpdateTime { get; set; }

    }
}
