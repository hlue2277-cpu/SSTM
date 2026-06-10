using Genesis.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Printing
{
	public abstract class PrinterBase
	{
		private const int MaxRetryCount = 3;
		protected ILogger Logger = new GenesisLogger();
		public string PrinterStatus { get; set; }

		public (bool IsOK, string Message) CheckPrinter()
		{
			Logger.Debug("开始监测打印机");

			var cause = string.Empty;
			bool isCheckedOk = false;
			for (int repeat = 0; repeat < MaxRetryCount; repeat++)
			{
				isCheckedOk = CheckImpl(out cause);
				if (isCheckedOk)
					break;

				Logger.Error($"打印机第{repeat + 1}次监测失败");

				if (repeat < MaxRetryCount - 1)
				{
					Thread.Sleep(1000);

					RetryConnectToPrinter();
				}
			}

			Logger.Debug("打印机监测结束，结果：" + (isCheckedOk ? "正常" : "异常"));

			PrinterStatus = cause;
			return (isCheckedOk, cause);
		}

		protected virtual bool CheckImpl(out string cause)
		{
			cause = string.Empty;
			return false;
		}

		protected virtual void RetryConnectToPrinter()
		{
			Logger.Debug("尝试重新连接打印机...");
		}
	}
}
