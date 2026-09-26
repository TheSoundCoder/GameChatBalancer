namespace AudioControl
{
    public class SerialDeviceService : ISerialDeviceService
    {
        public SerialDeviceService(Form1 form)
        {
            USBandCOM.HandOverForm(form);
        }

        public bool OpenComPort()
        {
            return USBandCOM.OpenComPort();
        }

        public void CloseComPort()
        {
            USBandCOM.CloseComPort();
        }

        public bool IsConnected()
        {
            return USBandCOM.sp_connected();
        }

        public void SendData(string data)
        {
            USBandCOM.sp_SendData(data);
        }

        public void Shutdown()
        {
            USBandCOM.Shutdown();
        }
    }
}
