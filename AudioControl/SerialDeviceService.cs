namespace AudioControl
{
    public class SerialDeviceService : ISerialDeviceService
    {
        public event EventHandler? ConnectionStatusChanged;

        public SerialDeviceService(Form1 form)
        {
            USBandCOM.HandOverForm(form);
            USBandCOM.ConnectionStatusChanged += OnConnectionStatusChanged;
        }

        private void OnConnectionStatusChanged(object? sender, EventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, e);
        }

        public bool Connected => USBandCOM.sp_connected();

        public string CurrentPort => USBandCOM.CurrentPort;

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
