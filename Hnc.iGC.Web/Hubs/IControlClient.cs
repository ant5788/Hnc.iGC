namespace Hnc.iGC.Web.Hubs
{
    public interface IControlClient
    {
        Task MessageInfo(string message);
        Task MessageWarning(string message);
        Task MessageError(string message);
        Task MessageSuccess(string message);

        Task ShowSTATUS(string deviceId, string value);

        Task ShowFEEDSPEED(string deviceId, double value);

        Task ShowFEEDOVERRIDE(string deviceId, int value);

        Task ShowSPDLOVERRIDE(string deviceId, int value);

        Task ShowRAPIDOVERRIDE(string deviceId, int value);

        Task ShowPARTCOUNT(string deviceId, int value);

        Task ShowMAGTOOLNO(string deviceId, int value);

        Task ShowSPDTOOLNO(string deviceId, int value);

        Task RegisterValueChanged(string deviceId, string value);

        Task ShowAXIS(string deviceId, string value);

        Task ShowMacroVar(string deviceId, int index, double value);

        Task ShowWARNING(string deviceId, string value);
    }
}
