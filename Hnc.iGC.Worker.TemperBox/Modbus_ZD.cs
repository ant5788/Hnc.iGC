using System.Net;
using System.Net.Sockets;
using System;

namespace Hnc.iGC.Worker.TemperBox
{
    internal class Modbus_ZD
    {
        static Socket? clientSocket;
        public string IP = "127.0.0.1";
        public int port = 503;
        public int timeout = 1000;

        public Modbus_ZD()
        {
            //Connect();
            /////创建一个线程的委托
            //ThreadStart childref = new ThreadStart(PrintEven);

            //Console.WriteLine("In Main: Creating the Child thread");

            ////创建线程的实例
            //Thread childThread = new Thread(childref);
            //childThread.Start();
            //Console.ReadKey();
        }
        ~Modbus_ZD()
        {
            DisConnect();
        }
        public bool Connect()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Console.WriteLine("開始發送數據。。。");
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP), port)); //配置服务器ip和端口
                //TcpClient tcpClient = new TcpClient(); 
                Console.WriteLine("服务器连接成功");
                return true;
            }
            catch
            {
                Console.WriteLine("连接服务器失败\n");
                return false;
            }
        }
        public void DisConnect()
        {
            clientSocket.Close();//关闭连接释放资源
            Console.WriteLine("关闭成功");
        }
        public short[] PrintEven(string startNO, string len)
        {
            int fnu = 4;
            string a = fnu.ToString("x2");
            string sendCodeMeg1 = "02 00 00 00 00 06 01 03" + startNO + len;
            short[] data1 = null;
            try
            {
                Console.WriteLine("开始！");
                for (int num = 0; num < 5; num++)
                {
                    Console.WriteLine("第{0}次调用Get_Fan函数！\n", num + 1);
                    byte[] aa = HexString(len);
                    byte[] bb = { aa[1], aa[0] };
                    int length = (int)System.BitConverter.ToInt16(bb, 0);
                    data1 = SendPack(sendCodeMeg1, IP, port, length);
                    if (data1 != null)
                    {
                        break;
                    }
                }
                Console.WriteLine("结束");
                //  Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据接受发送异常" + ex);
            }
            return data1;
        }

        #region 字节转换为16进制字符
        /// <summary>
        /// 字节转换为16进制字符
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>jiang
        static string ByteToHexString(byte[] data)
        {
            string strTemp = "";
            for (int i = 2; i < data.Length; i++)
            {
                string a = Convert.ToString(data[i], 16).PadLeft(2, '0');
                strTemp = strTemp + a;
            }
            return strTemp.Substring(0, 100);
        }
        #endregion
        #region 16进制字符转换为字节
        /// <summary>
        /// 16进制字符转换为字节
        /// </summary>
        /// <param name="hs"></param>
        /// <returns></returns>
        static byte[] HexString(string hs)
        {
            hs = hs.Replace(" ", "");
            string strTemp = "";
            byte[] b = new byte[hs.Length / 2];
            for (int i = 0; i < hs.Length / 2; i++)
            {
                strTemp = hs.Substring(i * 2, 2);
                b[i] = Convert.ToByte(strTemp, 16);
            }
            return b;
        }
        #endregion
        #region 发送、接收报文并返回报文
        /// <summary>
        /// 發送或接受風機指令
        /// </summary>
        /// <param name="sendCodeMeg">指令</param>
        /// <param name="IpAddress">IP地址</param>
        /// <param name="panelIP">面板IP</param>
        /// <returns></returns>
        short[] SendPack(string sendCodeMeg, string IpAddress, int port, int length)
        {
            short[] b = new short[1024];
            string sendMessage = sendCodeMeg; // "6B 00 00 00 00 06 02 06 05 10 00 01"; //发送到服务端的内容
            var sendData = HexString(sendMessage);
            byte[] recvBytes = new byte[1024];
            string recvStr = null;
            int bytes;
            try
            {
                Console.WriteLine("发送报文：{0}", sendMessage);
                clientSocket.Send(sendData);//向服务器发送数据 
                                            //连接时长500ms
                clientSocket.ReceiveTimeout = timeout;
                bytes = clientSocket.Receive(recvBytes, recvBytes.Length, 0); //服务端接受返回信息
                recvStr = ByteToHexString(recvBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务器接法数据出现错误！\n{0}\n\n", ex);
                recvStr = null;
            }
            if (recvStr != null)
            {
                for (int i = 0; i < length; i++)
                {
                    byte[] k = { recvBytes[10 + 2 * i], recvBytes[9 + 2 * i] };
                    //Console.WriteLine("获取成功！第{0}个数据:{1}\n", i, System.BitConverter.ToInt16(k, 0));
                    b[i] = System.BitConverter.ToInt16(k, 0);
                }
            }
            //clientSocket.Close();//关闭连接释放资源
            return b;
        }
        #endregion
    }
}