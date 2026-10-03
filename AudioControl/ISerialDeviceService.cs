namespace AudioControl
{
    public interface ISerialDeviceService
    {
        event EventHandler? ConnectionStatusChanged;
        bool Connected { get; }
        string CurrentPort { get; }
        bool OpenComPort();
        void CloseComPort();
        bool IsConnected();
        void SendData(string data);
        void Shutdown();
    }
}
