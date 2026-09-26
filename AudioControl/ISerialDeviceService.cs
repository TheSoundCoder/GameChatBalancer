namespace AudioControl
{
    public interface ISerialDeviceService
    {
        bool OpenComPort();
        void CloseComPort();
        bool IsConnected();
        void SendData(string data);
        void Shutdown();
    }
}
