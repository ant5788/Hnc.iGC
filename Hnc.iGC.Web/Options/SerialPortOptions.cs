﻿using System.IO.Ports;

namespace Hnc.iGC.Web.Options
{
    public class SerialPortOptions
    {
        public const string SerialPort = nameof(SerialPort);

        public string PortName { get; set; } = "COM1";

        public int BaudRate { get; set; } = 9600;

        public Parity Parity { get; set; } = Parity.None;

        public int DataBits { get; set; } = 8;

        public StopBits StopBits { get; set; } = StopBits.One;

        public Handshake Handshake { get; set; } = Handshake.None;

        public int ReadTimeout { get; set; } = 500;

        public int WriteTimeout { get; set; } = 500;

    }
}
