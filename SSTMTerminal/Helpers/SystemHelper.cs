using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace SSTMTerminal.Helpers
{
    public static class SystemHelper
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        public static void SetAutoRun(string filePath, bool isAutoRun = true)
        {
            RegistryKey reg = null;//定义一个注册表节点
            try
            {
                if (!System.IO.File.Exists(filePath))//若不存在该文件名，退出方法
                    return;

                string name = "SSTMTerminal_I";
                //给该注册表子项设置访问权限
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)//若不存在，则新创建一个注册表子项
                {
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                }
                if (isAutoRun)//若开机自动运行为true，重新赋值
                {
                    reg.SetValue(name, filePath);
                }
                else//否则，返回空
                {
                    reg.SetValue(name, "");
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (reg != null)
                {
                    reg.Close();//结束注册表相关操作
                }
            }
        }
    }
}