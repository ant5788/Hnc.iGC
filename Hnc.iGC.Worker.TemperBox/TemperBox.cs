using NModbus;
using System.Net.Sockets;
using System;
using System.Net;

namespace Hnc.iGC.Worker.TemperBox
{
    public class TemperBox : Collector<TemperBoxDto>
    {
        //private readonly ModbusClient client = new();

        //public override bool IsConnected => false;

        public override string Protocal { get; } = "MODBUS_TemperBox";

        private static ModbusFactory modbusFactory;
        private static IModbusMaster master;

        private TcpClient tcpClient;
        
        //上海增达
        private Modbus_ZD modbus_ZD;

        //苏轼
        public Socket newclient;


        public override bool Connect(string ip, ushort port, string type)
        {
            try
            {
                //   client.UnitIdentifier = _dcuInfo.siteId;
                //   client.Baudrate = _spHelper.ConfigSerialPort.BaudRate;
                //   client.Parity = (System.IO.Ports.Parity)_spHelper.ConfigSerialPort.Parity;
                //   client.StopBits = (System.IO.Ports.StopBits)_spHelper.ConfigSerialPort.StopBits;
                /*client.ConnectionTimeout = 1000;
                client.IPAddress = ip;
                client.Port = port;
                client.Connect();*/
                if (type == "重庆银河")
                {
                    Console.WriteLine(ip);
                    Console.WriteLine(port);
                    modbusFactory = new ModbusFactory();
                    tcpClient = new TcpClient(ip.Trim(), port);
                    master = modbusFactory.CreateMaster(tcpClient);
                    master.Transport.ReadTimeout = 1000;
                    master.Transport.Retries = 2;
                    isConnected = true;
                }
                if (type == "苏轼四达1000")
                {
                    int port1 = Convert.ToInt32(port);
                    IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), port1);
                    Socket newclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    newclient.Connect(ie);
                    isConnected = true;
                }
                if (type == "上海增达")
                {
                    Console.WriteLine(ip);
                    Console.WriteLine(port);
                    modbus_ZD = new Modbus_ZD();
                    modbus_ZD.timeout = 1000;
                    modbus_ZD.IP = ip;
                    modbus_ZD.port = port;
                    isConnected = modbus_ZD.Connect();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                isConnected = false;
                return IsConnected;
            }
            return IsConnected;
        }

        public override bool Disconnect()
        {

            if (null != newclient && newclient.Connected)
            {
                isConnected = false;
            }
            if (tcpClient != null) 
            {
                tcpClient.Close();
                isConnected = false;
            }
            if (modbus_ZD != null) 
            {
                modbus_ZD.DisConnect();
                isConnected = false;
            }
            return !IsConnected;
        }


        public override void SetDataTo(TemperBoxDto dto)
        {
            //var data = client.ReadDiscreteInputs(0, 16);
            //dto.Emergency = data[0];
            //dto.Alarm = data[1];

            // 运行模式
            int mode;
            // 状态
            int state;

            switch (dto.Description)
            {
                case "苏试四达1000":
                    // 状态
                    state = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("61"), ushort.Parse("1"))[0];
                    dto.State = 0;
                    dto.RunState = state switch
                    {
                        0 => "停机",
                        1 => "运行",
                    };
                    //PVSV(此处TMP是高温,HUM是低温)
                    ushort[] tmp1 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("108"), ushort.Parse("1"));
                    dto.SV_TMP = (float)Convert.ToInt32((tmp1)[0]) / 10;
                    ushort[] hum1 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("109"), ushort.Parse("1"));
                    dto.SV_HUM = (float)Convert.ToInt32((hum1)[0]) / 10;
                    ushort[] pvtmp1 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("100"), ushort.Parse("2"));
                    dto.PV_TMP = ByteArrToFloat(pvtmp1) / 10;
                    ushort[] pvhum1 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("102"), ushort.Parse("2"));
                    dto.PV_HUM = ByteArrToFloat(pvhum1) / 10;
                    // DI输入状态(16bit)
                    dto.DI = new bool[16].ToString();
                    dto.DO = new bool[16].ToString();
                    ushort[] di = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("120"), ushort.Parse("1"));
                    for (int i = 0; i <= 15; i++)
                    {
                        double tempConvert = Math.Pow(2, i);
                        if ((Convert.ToInt64(di[0]) & Convert.ToInt64(tempConvert)) == tempConvert)
                        {
                            dto.DI = "true";
                        }
                        else
                        {
                            dto.DI = "false";
                        }
                    }
                    // DO输出状态(16bit)
                    ushort[] dot = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("123"), ushort.Parse("1"));
                    for (int i = 0; i <= 15; i++)
                    {
                        double tempConvert = Math.Pow(2, i);
                        if ((Convert.ToInt64(dot[0]) & Convert.ToInt64(tempConvert)) == tempConvert)
                        {
                            dto.DO = "true";
                        }
                        else
                        {
                            dto.DO = "false";
                        }
                    }
                    // 步时间
                    TimeSpan t;
                    ushort[] time = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("135"), ushort.Parse("1"));
                    t = TimeSpan.FromMinutes(time[0]);
                    dto.StepTime_Set = string.Format("{0:D3}:{1:D2}", t.Hours + t.Days * 24, t.Minutes);
                    time = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("133"), ushort.Parse("1"));
                    t = TimeSpan.FromMinutes(time[0]);
                    dto.StepTime_Cur = string.Format("{0:D3}:{1:D2}", t.Hours + t.Days * 24, t.Minutes);
                    break;
                case "苏试四达1200":
                    // 状态
                    state = Convert.ToInt32(master.ReadCoils(byte.Parse("1"), ushort.Parse("107"), ushort.Parse("1"))[0]);
                    dto.State = state;
                    dto.RunState = state switch
                    {
                        0 => "停机",
                        1 => "运行",
                    };
                    // 运行模式
                    mode = Convert.ToInt32(master.ReadCoils(byte.Parse("1"), ushort.Parse("108"), ushort.Parse("1"))[0]);
                    dto.ControlMode = mode switch
                    {
                        0 => "定值控制",
                        1 => "程序控制",
                    };
                    // PVSV
                    ushort[] tmp = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("2"), ushort.Parse("1"));
                    dto.SV_TMP = (float)Convert.ToInt32(tmp);
                    ushort[] hum = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("3"), ushort.Parse("1"));
                    dto.SV_HUM = (float)Convert.ToInt32(hum);
                    ushort[] pvtmp = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("0"), ushort.Parse("1"));
                    dto.PV_TMP = (float)Convert.ToInt32(pvtmp);
                    ushort[] pvhum = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("1"), ushort.Parse("1"));
                    dto.PV_HUM = (float)Convert.ToInt32(pvhum);
                    // 步数
                    ushort[] a1 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("6"), ushort.Parse("1"));
                    ushort[] a2 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("7"), ushort.Parse("1"));
                    dto.StepNum_Set = $"{a1}";
                    dto.StepNum_Cur = $"{a2}";
                    // 运行时间
                    ushort[] t1 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("20"), ushort.Parse("1"));
                    ushort[] t2 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("21"), ushort.Parse("1"));
                    dto.RunTime_Cur = string.Format("{0}:{1}", t1, t2);
                    t1 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("18"), ushort.Parse("1"));
                    t2 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("19"), ushort.Parse("1"));
                    dto.RunTime_Set = string.Format("{0}:{1}", t1, t2);
                    break;
                case "上海增达":

                    Console.WriteLine("dds");
                    // 运行模式
                    short[] buff = modbus_ZD.PrintEven(" 00 01", " 00 01");
                    state = buff[0];
                    dto.State = state - 1;
                    Console.WriteLine("dto.State=============" + dto.State);
                    dto.RunState = (state - 1) switch
                    {
                        0 => "停机",
                        1 => "运行",
                        2 => "保持",
                        3 => "跳步",
                    };
                    // 控制模式
                    buff = modbus_ZD.PrintEven(" 00 03", " 00 01");
                    dto.ControlMode = buff[0] switch
                    {
                        0 => "程序控制",
                        1 => "定值控制",
                    };
                    // PVSV
                    buff = modbus_ZD.PrintEven(" 00 04", " 00 04");
                    dto.PV_TMP = (float)(buff[0]) / 10;
                    dto.PV_HUM = (float)(buff[1]) / 10;
                    dto.SV_TMP = (float)(buff[2]) / 10;
                    dto.SV_HUM = (float)(buff[3]) / 10;
                    // 报警
                    int alarmNo = (int)(modbus_ZD.PrintEven(" 00 2C", " 00 01")[0]);
                    dto.Alarmstate = alarmNo == 99 ? false : true;
                    dto.AlarmData = dto.Alarmstate ? "温湿度限制报警:" + alarmNo : "无";
                    alarmNo = (int)(modbus_ZD.PrintEven(" 00 2D", " 00 01")[0]);
                    if (alarmNo != 99)
                    {
                        dto.Alarmstate = true;
                        if (dto.AlarmData == "无")
                        {
                            dto.AlarmData = "设备故障报警";
                        }
                        else
                        {
                            dto.AlarmData += "    设备故障报警";
                        }
                    }
                    // 段数
                    buff = modbus_ZD.PrintEven(" 00 34", " 00 04");
                    dto.Section_Set = $"{buff[0]}";
                    dto.Section_Cur = $"{buff[1]}";
                    // 段循环数
                    dto.Cycle_Section_Set = $"{buff[2]}";
                    dto.Cycle_Section_Cur = $"{buff[3]}";
                    // 程序运行参数(程序控制模式)
                    if (dto.ControlMode == "程序控制")
                    {
                        buff = modbus_ZD.PrintEven(" 00 1C", " 00 08");
                        // 运行时间
                        t = TimeSpan.FromMinutes(buff[0]);
                        dto.RunTime_Set = string.Format("{0:D3}:{1:D2}", t.Hours + t.Days * 24, t.Minutes);
                        t = TimeSpan.FromMinutes(buff[1]);
                        dto.RunTime_Cur = string.Format("{0:D3}:{1:D2}", t.Hours + t.Days * 24, t.Minutes);
                        // 步时间
                        t = TimeSpan.FromMinutes(buff[4]);
                        dto.StepTime_Set = string.Format("{0:D3}:{1:D2}", t.Hours + t.Days * 24, t.Minutes);
                        t = TimeSpan.FromMinutes(buff[5]);
                        dto.StepTime_Cur = string.Format("{0:D3}:{1:D2}", t.Hours + t.Days * 24, t.Minutes);
                        // 循环数
                        dto.Cycle_Set = $"{buff[6]}";
                        dto.Cycle_Cur = $"{buff[7]}";
                        // 步数
                        dto.StepNum_Set = $"{buff[2]}";
                        dto.StepNum_Cur = $"{buff[3]}";
                    }
                    // 定值运行参数(定值运行模式)
                    else if (dto.ControlMode == "定值运行")
                    {
                        buff = modbus_ZD.PrintEven(" 00 18", " 00 02");
                        // 运行时间
                        t = TimeSpan.FromMinutes(buff[0]);
                        dto.RunTime_Set = string.Format("{0:D3}:{1:D2}", t.Hours + t.Days * 24, t.Minutes);
                        t = TimeSpan.FromMinutes(buff[1]);
                        dto.RunTime_Cur = string.Format("{0:D3}:{1:D2}", t.Hours + t.Days * 24, t.Minutes);
                    }
                    break;
                case "重庆银河":
                    // 运行模式
                    state = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("200"), ushort.Parse("1"))[0];
                    dto.RunState = state switch
                    {
                        0 => "停机",
                        1 => "运行",
                        2 => "保持",
                        _ => "无",
                    };

                    //dto.PV_TMP = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("1742"), ushort.Parse("2"))[0];
                    //温度PV
                    ushort[] var = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("1742"), ushort.Parse("2"));
                    dto.PV_TMP = ByteArrToFloat(var);
                    //温度SV
                    ushort[] var1 = master.ReadHoldingRegisters(byte.Parse("1"), ushort.Parse("160"), ushort.Parse("2"));
                    dto.SV_TMP = ByteArrToFloat(var1);
                    // 报警 无提供报警信息
                    break;
                default:
                    break;
            }
        }

        public float ByteArrToFloat(ushort[] var)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(var[1] & 0xFF);
            bytes[1] = (byte)(var[1] >> 8);
            bytes[2] = (byte)(var[0] & 0xFF);
            bytes[3] = (byte)(var[0] >> 8);
            float value = BitConverter.ToSingle(bytes, 0);
            return value;
        }
    }
}
