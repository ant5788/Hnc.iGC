using System;

using S7.Net;

namespace Hnc.iGC.Worker.S7
{
    internal class S7_CSharp : Collector<BalancerDto>
    {
        private Plc? plc;

        public override string Protocal { get; } = "S7_Balancer";

        public override bool Connect(string ip, ushort port = 102,string type ="")
        {
            try
            {
                plc = new Plc(CpuType.S71200, ip, port, 0, 0);
                plc.Open();
            }
            catch (Exception)
            {
            }
            isConnected = plc?.IsConnected ?? false;
            return IsConnected;
        }

        public override bool Disconnect()
        {
            plc?.Close();
            isConnected = plc?.IsConnected ?? false;
            return !IsConnected;
        }

        public override void SetDataTo(BalancerDto dto)
        {
            if (plc?.IsConnected == false || plc == null)
            {
                return;
            }

            var data_2_12 = plc.ReadBytes(DataType.DataBlock, 100, 0, 2);
            dto.BalanceReadyOK = data_2_12[0].SelectBit(0);
            dto.BalanceERROR = data_2_12[0].SelectBit(1);
            dto.BalanceISafetyGateOpen = data_2_12[0].SelectBit(2);
            dto.BalanceWorking = data_2_12[0].SelectBit(3);
            dto.BalanceISafetyGateClose = data_2_12[0].SelectBit(4);
            dto.BalanceiEmergencyStop = data_2_12[0].SelectBit(5);
            dto.BalanceAllowLoadPiece = data_2_12[0].SelectBit(6);
            dto.BalanceAllowDownPiece = data_2_12[0].SelectBit(7);
            dto.RobotUnloadingDone = data_2_12[1].SelectBit(0);
            dto.RobotLoadingDone = data_2_12[1].SelectBit(1);
            dto.RobotiEmergencyStop = data_2_12[1].SelectBit(2);

            dto.Speed_D20D0 = ((uint)(plc.Read("DB100.DBD2") ?? 0)).ConvertToFloat();
            dto.QuietAmountDataPS = ((uint)(plc.Read("DB100.DBD6") ?? 0)).ConvertToFloat();
            dto.QuietAngleDataPS = ((uint)(plc.Read("DB100.DBD10") ?? 0)).ConvertToFloat();
            dto.QuietPreAmountDataPS = ((uint)(plc.Read("DB100.DBD14") ?? 0)).ConvertToFloat();
            dto.QuietPreAngleDataPS = ((uint)(plc.Read("DB100.DBD18") ?? 0)).ConvertToFloat();
            dto.FMTol = ((uint)(plc.Read("DB100.DBD22") ?? 0)).ConvertToFloat();

            var data_26 = plc.ReadBytes(DataType.DataBlock, 100, 26, 1);
            dto.OK_D20X4416 = data_26[0].SelectBit(0);
            dto.NG_D20X4417 = data_26[0].SelectBit(1);

            dto.OperationMode_D20X4418 = ((ushort)(plc.Read("DB100.DBW28") ?? 0)).ConvertToShort();
            dto.QuietInterface_D20X4419 = plc.ReadBytes(DataType.DataBlock, 100, 30, 1)[0].SelectBit(0);
            dto.OnLine_D20W342 = ((ushort)(plc.Read("DB100.DBW32") ?? 0)).ConvertToShort();
            dto.SNExpire_D20X4607 = plc.ReadBytes(DataType.DataBlock, 100, 34, 1)[0].SelectBit(0);

            dto.ToolLife_D20W332 = ((ushort)(plc.Read("DB100.DBW36") ?? 0)).ConvertToShort();
            dto.ToolResidualLife_D20W322 = ((ushort)(plc.Read("DB100.DBW38") ?? 0)).ConvertToShort();

        }
    }
}
