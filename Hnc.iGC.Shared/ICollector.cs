namespace Hnc.iGC
{
    public interface ICollector<TDto>
    {
        string Protocal { get; }
        bool IsConnected { get; }
        bool Connect(string ip, ushort port);
        bool Disconnect();
        void SetDataTo(TDto dto);
    }
}
