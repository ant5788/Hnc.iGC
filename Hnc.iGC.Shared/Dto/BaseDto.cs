namespace Hnc.iGC
{
    public class BaseDto
    {
        /// <summary>
        /// 设备Id，要求唯一
        /// </summary>
        public string DeviceId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 设备名称，按用户要求命名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 设备描述，通常写设备型号
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 通信协议
        /// </summary>
        public string Protocal { get; set; } = string.Empty;

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; } = string.Empty;

        /// <summary>
        /// 端口号
        /// </summary>
        public ushort Port { get; set; } = ushort.MinValue;
    }
}
