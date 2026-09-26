namespace AudioControl
{
    public class UsbWatcherService : IUsbWatcherService
    {
        public void Start()
        {
            USBandCOM.Initialize_USB_Watcher();
        }

        public void Stop()
        {
            USBandCOM.StopUsbWatcher();
        }
    }
}
