using Genesis.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Printing
{
	public class BTP6206Printer : PrinterBase, IDisposable
	{
		private bool disposed = false;
		private bool isPrinterOpen = false;

		public BTP6206Printer()
		{
			isPrinterOpen = Printer.BTP_6206Helper.Open(out var msg);
			Logger.Debug("btp6206 open port: " + msg);
		}

		protected override bool CheckImpl(out string cause)
		{
			cause = string.Empty;
			bool isBTP6206OK = false;
			try
			{
				if (isPrinterOpen)
				{
					var status = Printer.BTP_6206Helper.GetStatus();
					switch (status)
					{
						case Printer.PrinterStatus.PortNotOpen:
							cause = "端口异常";
							break;

						case Printer.PrinterStatus.PaperIsEnd:
							cause = "纸尽";
							break;

						case Printer.PrinterStatus.DeviceIsBusy:
							cause = "打印机忙";
							break;

						case Printer.PrinterStatus.DeviceIsPause:
							cause = "打印机暂停中";
							break;

						case Printer.PrinterStatus.TPHIsHotter:
							cause = "打印头过热";
							break;

						case Printer.PrinterStatus.TPHIsOpened:
							cause = "打印头抬起";
							break;

						case Printer.PrinterStatus.CutterIsError:
							cause = "切刀错误";
							break;

						case Printer.PrinterStatus.OK:
							isBTP6206OK = true;
							break;

						case Printer.PrinterStatus.DeviceSerialCommunicationError:
							cause = "设备通信错误";
							break;
						case Printer.PrinterStatus.GetStatusDataError:
							cause = "获取打印机状态数据错误";
							break;
						case Printer.PrinterStatus.RibbonIsEnd:
							cause = "色带用尽";
							break;
						default:
							cause = "驱动故障";
							break;
					}
				}
				else
				{
					cause = "端口异常，打印机打开失败";
				}

				Logger.Debug("监测btp6206状态: " + isBTP6206OK);
			}
			catch (Exception ex)
			{
				Logger.Error("打印机检测失败:" + ex.Message);
				cause = "系统故障";
			}

			return isBTP6206OK;
		}

		protected override void RetryConnectToPrinter()
		{
			base.RetryConnectToPrinter();
			try
			{
				Printer.BTP_6206Helper.Close(out var msg);
				Logger.Debug("关闭btp6206端口: " + msg);

				Thread.Sleep(1000);

				isPrinterOpen = Printer.BTP_6206Helper.Open(out msg);
				Logger.Debug("打开btp6206端口: " + msg);
			}
			catch(Exception ex)
			{
				Logger.Debug("调用RetryToConnect()失败。" + ex.Message);
			}
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
				Printer.BTP_6206Helper.Close(out var msg);
				Logger.Debug("关闭btp6206端口:" + msg);
			}

			disposed = true;
		}

		#endregion
	}
}
