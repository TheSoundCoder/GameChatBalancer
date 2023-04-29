//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
using System.Diagnostics;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
//using System.Linq;
//using System.Security.Policy;
//using System.Text;
//using System.Threading.Tasks;

namespace AudioControl
{

    public class USBandCOM
    {
        #region Global variable declaration

        static AudioControl.Form1 MainForm;     //holds a referende to Form1
        static SerialPort _serialPort = new SerialPort();
        static ManagementEventWatcher watcher = new ManagementEventWatcher();
        static WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");

        static string cComPort = "";            //used COM Port
        //bool Connected = false;                 //Connection open or closed?
        static string buffer = "";

        #endregion

        public static void HandOverForm(Form1 f)
        {
            MainForm = f;
        }

        #region COM port handling

        private static string AutodetectArduinoPort()
        {
            /* Get all active com ports and enumerate.
             * Send initialization bit to Arduino and receive "ack" after 500 ms
            */
            int WaitTime = 1500;
            string answer = "";
            foreach (string ComPort in SerialPort.GetPortNames())
            {
                MainForm.SendToLog("Trying Port " + ComPort);
                //_serialPort = new SerialPort();
                if (_serialPort.IsOpen) { CloseComPort(); }
                _serialPort.PortName = ComPort;//Set your board COM
                _serialPort.BaudRate = 115200;
                _serialPort.DtrEnable = true;
                //_serialPort.DataReceived -= new SerialDataReceivedEventHandler(sp_DataReceived);
                //_serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived_Init);
                try
                {
                    _serialPort.Open();
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.WriteLine("syn");
                        System.Threading.Thread.Sleep(WaitTime);
                        answer = _serialPort.ReadExisting();
                        MainForm.SendToLog("Answer after 1500ms: " + answer.Replace("\r\n",""));
                    }
                }
                catch
                {
                    MainForm.SendToLog(ComPort + " not available.");
                }
                finally { if (_serialPort.IsOpen) { CloseComPort(); } }
                if (answer.IndexOf("ack") > -1)
                {
                    return ComPort;
                }
                //if (_serialPort.IsOpen) { _serialPort.Close(); }
            }

            return "Auto";
        }
        public static bool OpenComPort()
        {
            bool RetVal = false;
            int Retry = 3;
            int i = 1;
            //_serialPort = new SerialPort();
            if (_serialPort.IsOpen) { CloseComPort(); }
            while (!RetVal && i < Retry)
            {
                i++;
                if (Properties.Settings.Default.ComPort.ToUpper() == "AUTO")
                {
                    cComPort = AutodetectArduinoPort();
                }
                else
                {
                    cComPort = Properties.Settings.Default.ComPort;
                }
                if (cComPort.ToUpper() == "AUTO") { return false; }
                buffer = "";
                _serialPort.PortName = cComPort;//Set your board COM
                _serialPort.BaudRate = 115200;
                _serialPort.DtrEnable = true;
                _serialPort.NewLine = "\r\n";
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived); //event based reading
                //_serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(sp_Error);
                try
                {
                    _serialPort.Open();
                    MainForm.SendToLog("Connected to: " + cComPort);
                    RetVal = true;
                    MainForm.SystrayCom = cComPort;
                    MainForm.Connected = true;
                }
                catch
                {
                    MainForm.SendToLog("Connection failed");
                }
                finally { }
            }
            return RetVal;
        }

        
        public static void CloseComPort()
        {
            if (_serialPort.IsOpen) { _serialPort.Close(); }
            _serialPort.DataReceived -= new SerialDataReceivedEventHandler(sp_DataReceived); //event based reading
            buffer = "";
            MainForm.SystrayCom = "disconnected";
            MainForm.Connected = false;
        }

        public static void sp_SendData(string Data)
        {
            _serialPort.WriteLine(Data);
        }

        private static void sp_Reconnect()
        {
            CloseComPort();
            MainForm.Initialized = false;       //if not set to false, Fill_ddl_ComPort() will also Open the connection -> suppress
            MainForm.Fill_ddl_ComPort();        //refill DDL for ComPort selection
            MainForm.Initialized = true;
            OpenComPort();
            if (sp_connected())
            {
                sp_SendData("get");
                MainForm.Send_NoiseReducion_Value();
            }
        }

        
        private static void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _serialPort.DataReceived -= new SerialDataReceivedEventHandler(sp_DataReceived);
            //if (MainForm.Debug) { MainForm.SendToLog("Starting continous read."); }
            while (_serialPort.BytesToRead > 0)
            {
                buffer += _serialPort.ReadExisting();
                if (buffer != null && buffer != "")
                {
                    //string[] packets1 = buffer.Split("\r\n");
                    //int arrlength = packets1.Length - 1;
                    if (buffer.IndexOf("NR=")>-1)
                    {
                        //confirmation that NoiseReduction was successfully set
                        MainForm.ConfirmNR();
                        MainForm.SendToLog("Noise reduction confirmed by Hardware.");
                        buffer.Replace("NR=", "");
                        buffer = Regex.Replace(buffer, "NR=.\\r\\n", "");
                        //buffer = "";
                    }
                    if (buffer != null && buffer != "")
                    {
                        //Extract latest volume level from strem
                        string[] packets1 = buffer.Split("\r\n");
                        int arrlength = packets1.Length - 1;
                        if (arrlength > 0)
                        {
                            if (MainForm.Debug) { MainForm.SendToLog("Value: " + packets1[arrlength - 1]); }
                            MainForm.controlVolume(float.Parse(packets1[arrlength - 1]));
                        }
                        //SetMasterVolume(float.Parse(packets1[arrlength - 1])); }
                        if (buffer.EndsWith("\n"))
                        {
                            buffer = "";
                        }
                        else
                        {
                            buffer = packets1[arrlength];
                        }
                    }
                }
            }
            //if (MainForm.Debug) { MainForm.SendToLog("Ending continous read."); }
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);

            /*
            if (MainForm.Debug) { MainForm.SendToLog("ThreatCount: " + Threatcount.ToString()); }
            buffer += _serialPort.ReadExisting();
            //if (Debug) { textBox1.AppendText("Buffer: " + buffer + "\r\n"); }
            if (buffer == null) { return; }

            string[] packets1 = buffer.Split("\r\n");
            int arrlength = packets1.Length - 1;
            if (arrlength > 0)
            { MainForm.controlVolume(float.Parse(packets1[arrlength - 1])); }
            //SetMasterVolume(float.Parse(packets1[arrlength - 1])); }
            if (buffer.EndsWith("\n"))
            {
                buffer = "";
            }
            else
            {
                buffer = packets1[arrlength];
            }
            */

        }


        public static bool sp_connected() { return _serialPort.IsOpen; }

        private void sp_Error(object sender, SerialErrorReceivedEventArgs e)
        {
            //if (Debug) { textBox1.AppendText("Serial port disconnected.\r\n"); }
        }

        #endregion

        #region USB Device Event-Handling
        public static void Initialize_USB_Watcher()
        {
            //WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType=2");
            watcher.EventArrived += new EventArrivedEventHandler(USB_Event);
            watcher.Query = query;
            watcher.Start();
            //watcher.WaitForNextEvent();

        }

        private static void USB_Event(object sender, EventArrivedEventArgs e)
        {
            //watcher.EventArrived -= new EventArrivedEventHandler(USB_Connect);
            //watcher.Stop();

            /*
            textBox1.AppendText("\r\n" + "USB device event: " + "\r\n");

            foreach (var _property in e.NewEvent.Properties)
            {
                //textBox1.AppendText(_property.Name + " = " + _property.Value + "\r\n");
            }
            foreach (var _property in e.NewEvent.Qualifiers)
            {
                //textBox1.AppendText(_property.Name + " = " + _property.Value + "\r\n");
            }
            foreach (var _property in e.NewEvent.SystemProperties)
            {
                textBox1.AppendText(_property.Name + " = " + _property.Value + "\r\n");
            }
            */

            if (e.NewEvent.SystemProperties["__CLASS"].Value.ToString() == "__InstanceDeletionEvent")
            {
                MainForm.SendToLog("A USB device was disconnected.");
                //Do I need to do anything? Yes. Check whether the connection is still open
                sp_Reconnect();
            }
            else if (e.NewEvent.SystemProperties["__CLASS"].Value.ToString() == "__InstanceCreationEvent")
            {
                MainForm.SendToLog("A USB device was connected.");
                if (!MainForm.Connected)
                {
                    sp_Reconnect();
                }
            }


            /*
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            foreach (var property in instance.Properties)
            {
                textBox1.AppendText(property.Name + " = " + property.Value + "\r\n");
            }
            */


            //watcher.EventArrived += new EventArrivedEventHandler(USB_Disconnect);
            //watcher.Query = query_disconnect;
            //watcher.Start();
        }

        #endregion

    }
}
