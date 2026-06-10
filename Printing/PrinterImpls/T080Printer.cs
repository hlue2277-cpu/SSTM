using Genesis.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Printing
{
	public class T080Printer : PrinterBase
	{
		protected override bool CheckImpl(out string cause)
		{
			cause = string.Empty;
			bool isT080OK = false;
			string msg;
			try
			{
				var isSuccess = TKIOSK.OpenPort(out msg);
				Logger.Debug("t080 open port:" + msg);

				if (isSuccess)
				{
					var status = TKIOSK.GetStatus(out msg);
					switch (status)
					{
						case StatusEnum.QueryFail:
							cause = "连接故障";
							break;

						case StatusEnum.Ok:
							isT080OK = true;
							break;

						case StatusEnum.HeadUp:
							cause = "上盖打开";
							break;

						case StatusEnum.PaperEnd:
							cause = "纸尽";
							break;

						case StatusEnum.CutterError:
							cause = "切刀错误";
							break;

						case StatusEnum.TPHTooHot:
							cause = "打印头过热";
							break;

						case StatusEnum.PaperNearEnd:
							cause = "纸将尽";
							isT080OK = true;
							break;

						case StatusEnum.PaperJam:
							cause = "卡纸";
							break;

						default:
							cause = "驱动故障";
							break;
					}
				}
				else
				{
					cause = "端口异常";
				}

				Logger.Debug("监测t080状态:" + msg + isT080OK);
			}
			catch (Exception ex)
			{
				Logger.Error("打印机检测失败：" + ex.Message);
				cause = "系统故障";
			}
			finally
			{
				TKIOSK.ClosePort(out msg);
				Logger.Debug("关闭t080端口:" + msg);
			}

			return isT080OK;
		}
	}
}
