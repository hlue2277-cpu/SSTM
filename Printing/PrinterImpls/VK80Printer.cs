using Custom.CuCustomWndAPI;
using Genesis.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Printing
{
	public class VK80Printer : PrinterBase, IDisposable
	{
		private static CuCustomWndAPIWrap customWndAPIWrap = null;
		private static CuCustomWndDevice vk80Device = null;
		private static USBDevice vk80UsbDevice = null;
		private bool disposed = false;

		public VK80Printer()
		{
			Initialize();
		}

		private void Initialize()
		{
			if (customWndAPIWrap == null)
			{
				customWndAPIWrap = new CuCustomWndAPIWrap(CuCustomWndAPIWrap.CcwLogVerbosity.CCW_LOG_DEEP_DEBUG, null);
				//Init the library
				customWndAPIWrap.InitLibrary();

				var devices = customWndAPIWrap.EnumUSBDevices();
				if (devices != null && devices.Length > 0)
				{
					var configuredPrinterName = ConfigurationManager.AppSettings.Get("PrinterName");
					vk80UsbDevice = devices.FirstOrDefault(p => p.PrinterName == configuredPrinterName);
					if (vk80UsbDevice == null)
					{
						Logger.Error("No VK80 Printer named ZTP80 L32 E02 found");
					}
					else
					{
						vk80Device = customWndAPIWrap.OpenPrinterUSB(vk80UsbDevice);
						Logger.Information($"{vk80UsbDevice.ToString()} is open.");
					}
				}
				else
				{
					Logger.Error("No Printer found by calling EnumUSBDevices");
				}
			}
		}

		protected override bool CheckImpl(out string cause)
		{
			cause = string.Empty;
			bool isVK80OK = false;
			if(vk80Device == null)
			{
				cause = "连接并打开打印机失败";
				return false;
			}

			try
			{
				PrinterStatus status = vk80Device.GetPrinterFullStatus();
				Logger.Debug($"VK80 Printer Status:{status.ToString()}.");
				if(status.StsPAPERJAM)
				{
					cause = "卡纸";
				}
				if (status.StsNOPAPER)
				{
					cause = "纸尽";
				}
				if (status.StsOVERTEMP)
				{
					cause = "打印头过热"; // ?
				}
				if (status.StsNOCOVER)
				{
					cause = "上盖打开"; // ?
				}
				if (status.StsNOHEAD)
				{
					cause = "打印头错误"; // ?
				}
				if (status.StsRAMERROR)
				{
					cause = "RAM错误"; // ?
				}
				if (status.StsEEPROMERROR)
				{
					cause = "EEPROM错误";
				}
				if (status.StsCUTERROR)
				{
					cause = "切刀错误";
				}
				if (status.StsHLVOLT)
				{
					cause = "电压错误";
				}


				if(string.IsNullOrEmpty(cause))
				{
					isVK80OK = true;
				}
				//if (status.StsPAPERROLLING)
				//{
				//	cause = "折纸"; // ?
				//}
				//if (status.StsTICKETOUT)
				//{
				//	cause = ""; // ?
				//}
				//if (status.StsLFPRESSED)
				//{
				//	cause = ""; // ?
				//}
				//if (status.StsFFPRESSED)
				//{
				//	cause = ""; // ?
				//}
			}
			catch (Exception ex)
			{
				Logger.Error("打印机检测失败：" + ex.Message);
				cause = "系统故障";
				isVK80OK = false;
			}

			return isVK80OK;
		}

		#region Implement IDisposable

		public void Dispose()
		{
			// Dispose of unmanaged resources.
			Dispose(true);
			// Suppress finalization.
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}

			if (disposing)
			{

				vk80Device?.Terminate();
				vk80Device = null;

				customWndAPIWrap?.DeInitLibrary();
				customWndAPIWrap = null;
			}

			disposed = true;
		}

		#endregion
	}
}
