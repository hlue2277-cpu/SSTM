using Genesis.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BarCodeScanner
{
    public class TwoDimensionCodeScanner
    {
        public static EventHandler<ScanDataEventArgs> OnScanDataReceived;

        private static RS232Scanner rs232Scanner;
        private static ILogger Logger = new GenesisLogger();

        private static bool isHardwareInitialized = false;
        public static string LastError = "";

        private static string scannedData = "";
        private static long lastScannedTicks = 0;
        private static object locker = new object();

        private static Queue<string> requestQueue = new Queue<string>();
        private static object queueLocker = new object();

        public static void RequestScanning(string request)
        {
            lock (queueLocker)
            {
                requestQueue.Enqueue(request);
            }
        }

        private static string DequeueRequest()
        {
            lock (queueLocker)
            {
                if (requestQueue.Count > 0)
                {
                    return requestQueue.Dequeue();
                }
            }
            return string.Empty;
        }

        public static (int ErrorCode, string ErrorMsg) Initialize(ushort comPortNumber)
        {
            if (rs232Scanner != null)
            {
                try
                {
                    rs232Scanner.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Closing old serial port failed before Initializing it. {ex.Message}. {ex.StackTrace}");
                    LastError = ex.Message;
                    return (-3, "有未关闭的连接");
                }
            }

            try
            {
                rs232Scanner = new RS232Scanner();
                //注册串口
                rs232Scanner.RegisterSerialPort($"COM{comPortNumber}", 115200);
                //打开串口
                if (rs232Scanner.Open())
                {
                    //关联事件处理程序
                    rs232Scanner.SerialPort.DataReceived -= OnSerialPortDataReceived;
                    rs232Scanner.SerialPort.DataReceived += OnSerialPortDataReceived;
                }
                else
                {
                    return (-1, $"打开COM{comPortNumber}失败");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Initializing TwoDimensionCodeScanner failed. {ex.Message}. {ex.StackTrace}");
                LastError = ex.Message;
                return (-2, "初始化硬件时发生未知异常");
            }

            return (0, "");
        }

        //串口接收接收事件处理程序
        //每当串口讲到数据后激发
        private static void OnSerialPortDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            var request = DequeueRequest();

            try
            {
                Thread.Sleep(300);

                lastScannedTicks = DateTime.Now.Ticks;
                byte[] m_recvBytes = new byte[rs232Scanner.SerialPort.BytesToRead];//定义缓冲区大小
                int result = rs232Scanner.SerialPort.Read(m_recvBytes, 0, m_recvBytes.Length);//从串口读取数据
                if (result <= 0)
                    return;

                lock (locker)
                {
                    scannedData = "";
                    rs232Scanner.Code = Encoding.ASCII.GetString(m_recvBytes, 0, m_recvBytes.Length);//对数据进行转换
                    scannedData = rs232Scanner.Code;
                    scannedData = scannedData.Replace("\r\n", "");

                    rs232Scanner.SerialPort.DiscardInBuffer();

                    OnScanDataReceived?.Invoke(null, new ScanDataEventArgs { Request = request, ErrorCode = 0, ScannedData = scannedData });
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error happens in OnSerialPortDataReceived. {ex.Message}. {ex.StackTrace}");
                LastError = ex.Message;
                OnScanDataReceived?.Invoke(null, new ScanDataEventArgs { Request = request, ErrorCode = 1, ScannedData = "" });
            }
        }

        public static int Shutdown()
        {
            try
            {
                rs232Scanner?.Close();
                rs232Scanner = null;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error happens in OnSerialPortDataReceived. {ex.Message}. {ex.StackTrace}");
                LastError = ex.Message;
            }

            return 0;
        }
    }

    public class ScanDataEventArgs : EventArgs
    {
        public string Request { get; set; }
        public int ErrorCode { get; set; }
        public string ScannedData { get; set; }
    }

}
