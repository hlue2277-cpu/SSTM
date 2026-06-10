using System;
using System.IO.Ports;

namespace BarCodeScanner
{
    public class RS232Scanner
    {
        //串口引用
        public SerialPort SerialPort;

        //存储转换的数据值
        public string Code { get; set; }

        public RS232Scanner()
        {
            SerialPort = new SerialPort();
        }

        //串口状态
        public bool IsOpen
        {
            get
            {
                return SerialPort.IsOpen;
            }
        }

        //打开串口
        public bool Open()
        {
            if (SerialPort.IsOpen)
            {
                Close();
            }
            SerialPort.Open();
            if (SerialPort.IsOpen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //关闭串口
        public void Close()
        {
            SerialPort.Close();
        }

        //定入数据，这里没有用到
        public void WritePort(byte[] send, int offSet, int count)
        {
            if (IsOpen)
            {
                SerialPort.Write(send, offSet, count);
            }
        }

        //获取可用串口
        public string[] GetComName()
        {
            string[] names = SerialPort.GetPortNames(); // 获取所有可用串口的名字
            return names;
        }

        //注册一个串口
        public void RegisterSerialPort(string portName, int baudRate)
        {
            //串口名
            SerialPort.PortName = portName;
            //波特率
            SerialPort.BaudRate = baudRate;
            //数据位
            SerialPort.DataBits = 8;
            //两个停止位
            SerialPort.StopBits = System.IO.Ports.StopBits.One;
            //无奇偶校验位
            SerialPort.Parity = System.IO.Ports.Parity.None;
            SerialPort.ReadTimeout = 100;
            //rs232Scanner.serialPort.WriteTimeout = -1;
        }
    }
}
