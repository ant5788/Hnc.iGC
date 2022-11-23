using EasyModbus;

namespace Hnc.iGC.Worker
{
    public class ModbusTCP_DistributedIO : Collector<CNCDto>
    {
        private readonly ModbusClient client = new();

        public override bool IsConnected => client?.Connected ?? false;

        public override string Protocal { get; } = "ModbusTCP_DistributedIO";

        public override bool Connect(string ip, ushort port, string type)
        {
            client?.Connect(ip, port);
            return IsConnected;
        }

        public override bool Disconnect()
        {
            client?.Disconnect();
            return !IsConnected;
        }

        /// <summary>
        /// 读取DI值
        /// </summary>
        /// <param name="index">1-16</param>
        /// <returns></returns>
        public bool GetDI(int index)
        {
            if (index < 1 || index > 16)
            {
                return false;
            }
            return client.ReadDiscreteInputs(index - 1, 1)[0];
        }

        public override void SetDataTo(CNCDto dto)
        {
            var data = client.ReadDiscreteInputs(0, 16);
            dto.Emergency = data[0];
            dto.Alarm = data[1];
        }
    }
}
