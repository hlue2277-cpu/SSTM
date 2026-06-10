using DataService;
using Genesis;
using Genesis.Logging;
using SqliteUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;

namespace Printing
{
    public class PrintManager : IPrintManager
	{
		private static Border PrintContainer { get; set; }

		private ILogger Logger;
		private PrinterBase printer;
		private PrintWindow printWindow;
		private ITicketService ticketService;
		private TicketNotifyInfoDao notifyDao;
		private Dictionary<string, bool> CurrentBatchPrintStatus;
		private int printedTicketNumInCurrentBatch = 0;

        public event EventHandler<List<string>> DocumentsPrintedEvent;

		public PrintManager(ILogger logger, ITicketService service, TicketNotifyInfoDao dao)
		{
			Logger = logger;
			ticketService = service;
			notifyDao = dao;

			CurrentBatchPrintStatus = new Dictionary<string, bool>();

			var printerType = ConfigurationManager.AppSettings.Get("PrinterDevice");
			var configFileName = "PrintConfig.xml";
			var cofigFilePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, configFileName);
			var printConfig = XmlCofigHelper.Load<PrintConfig>(cofigFilePath);
			var printSettingConfig = printConfig.PrintSettingConfigs.FirstOrDefault(c => c.PrinterType == printerType);
			printWindow = new PrintWindow(printSettingConfig);
			printWindow.DocumentPrintedEvent += OnDocumentPrinted;

			printer = PrinterFactory.Create(printerType);
		}

		private void OnDocumentPrinted(object sender, PrintModel printModel)
		{
			Logger.Information($"第{printModel.PrintIndex + 1}张门票({printModel.ToBePrintedID})打印成功");
			NotifyServerTicketPrinted(printModel);
			if(CurrentBatchPrintStatus.ContainsKey(printModel.ToBePrintedID))
			{
				CurrentBatchPrintStatus[printModel.ToBePrintedID] = true;
			}
			if (CurrentBatchPrintStatus.Count > 0 
				&& !CurrentBatchPrintStatus.Any(s => s.Value == false))
			{
				DocumentsPrintedEvent?.Invoke(this, CurrentBatchPrintStatus.Keys.ToList());
			}
		}

		#region Public Methods

		public static void SetPrintContainer(Border printBoard)
		{
			PrintContainer = printBoard;
		}

		public void Print(List<PrintModel> printModels, string tradeNO = null)
		{
            printedTicketNumInCurrentBatch = 0;
            CurrentBatchPrintStatus.Clear();
			printModels.ForEach(m => CurrentBatchPrintStatus[m.ToBePrintedID] = false);

			for (int i = 0; i < printModels.Count; i++)
			{
				var checkResult = printer.CheckPrinter();
				if (!checkResult.IsOK)
				{
                    Logger.Error($"打印第{printModels[i].PrintIndex + 1}张门票({printModels[i].ToBePrintedID})时，打印机出现问题：{checkResult.Message}");
                    throw new PrintException(printModels.Count, printedTicketNumInCurrentBatch, tradeNO, checkResult.Message);
				}

				Logger.Information($"开始打印第{printModels[i].PrintIndex + 1}张门票({printModels[i].ToBePrintedID})");

				try
				{
					printWindow.Print(printModels[i]);
					++printedTicketNumInCurrentBatch;
                    Thread.Sleep(1000);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, $"打印第{printModels[i].PrintIndex + 1}张门票({printModels[i].ToBePrintedID})过程中发生错误");
					throw new PrintException(printModels.Count, printedTicketNumInCurrentBatch, tradeNO, $"打印门票过程中发生错误");
				}
			}
		}

		public List<PrintModel> GeneratePrintModels(List<TicketResponseData> tickets)
		{
			var printModels = new List<PrintModel>();
			try
			{
				for (int i = 0; i < tickets.Count; ++i)
				{
					var ticket = tickets[i];
					if (ticket == null)
					{
						continue;
					}

					Dictionary<string, string> imageSettings = new Dictionary<string, string>();
					PrintModel printModel = new PrintModel { PrintIndex = i, ToBePrintedID = ticket.uuid, ToBeNotifiedID = ticket.seatid, ImageSetting = imageSettings };

					string templateXml = GetTicketPrintTemplate(ticket.scheduleId.ToString());
					if (string.IsNullOrEmpty(templateXml))
					{
						Logger.Error($"获取门票（{ticket.uuid}）的打印模板失败。该门票打印中止。");
						continue;
					}

					if (templateXml.Contains("xmlns=\"\""))
					{
						templateXml = templateXml.Replace("xmlns=\"\"", "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
					}

					////TEST CODE
					//var templatePath = AppDomain.CurrentDomain.BaseDirectory + "TicketPrintTemplates\\TicketCanvas.xml";
					//var templateXml = File.ReadAllText(templatePath);

					XmlDocument xml = new XmlDocument();
					xml.LoadXml(templateXml);

					Canvas ticketPrintCanvas = ConvertXmlToCanvas(xml);

					try
					{
						AssignValueToTemplateXml(ticketPrintCanvas, ticket);
					}
					catch (Exception ex)
					{
						Logger.Error(ex, $"替换门票打印模板中的值失败，打印门票（{ticket.uuid}）打印中止。");

						continue;
					}
					CollectImageSettings(printModel, ticketPrintCanvas, ticket);

					ticketPrintCanvas.UpdateLayout();
					printModel.XDocument = XDocument.Parse(XamlWriter.Save(ticketPrintCanvas));
					printModels.Add(printModel);

					Logger.Debug("门票号：" + ticket.uuid + ";打印布局生成指令时间：" + DateTime.Now);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "生成门票打印模板列表失败");
			}

			return printModels;
		}

		public (bool IsOK, string Message) CheckPrinter()
		{
			return printer.CheckPrinter();
		}

		public void Dispose()
		{
			if (printer is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		#endregion

		#region Private Methods
		private void NotifyServerTicketPrinted(PrintModel printModel)
		{
			var response = ticketService.NotifyTicketPrinted(printModel.ToBePrintedID);
			if (response != null && response.success)
			{
				Logger.Information($"门票（{printModel.ToBePrintedID}）打印成功并成功更新门票在服务器中的状态。");
			}
			else
			{
				Logger.Information($"门票（{printModel.ToBePrintedID}）打印成功，但是更新门票在服务器中的状态失败。");

				var model = new TicketNotifyInfoModel
				{
					TicketUUID = printModel.ToBePrintedID,
					TicketSeatId = printModel.ToBeNotifiedID,
				};
				try
				{
					notifyDao.Insert(model);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, $"将通知服务器更新状态失败的门票（{printModel.ToBePrintedID}）插入本地数据库失败！");
				}
			}
		}

		private string GetTicketPrintTemplate(string scheduleId)
		{
			try
			{
				return ticketService.QueryTicketPrintTemplate(scheduleId)?.data?.layoutData;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, $"查询{scheduleId}对应的门票打印模板失败");

				return null;
			}
		}

		private void AssignValueToTemplateXml(Canvas ticketPrintLayout, TicketResponseData ticket)
		{
			try
			{
				AssignDynamicValue(ticketPrintLayout, ticket);
			}
			catch (Exception ex)
			{
				Logger.Error(ex, $"替换门票打印模板中的值失败");
			}
		}

		private static Canvas ConvertXmlToCanvas(XmlDocument xml)
		{
			//转化成面板
			using (MemoryStream ms = new MemoryStream())
			{
				xml.Save(ms);
				ms.Position = 0;

				var printLayoutContainer = PrintContainer;
				var ticketPrintCanvas = XamlReader.Load(ms) as Canvas;
				printLayoutContainer.Child = ticketPrintCanvas;
				printLayoutContainer.Width = ticketPrintCanvas.Width;
				printLayoutContainer.Height = ticketPrintCanvas.Height;

				return ticketPrintCanvas;
			}
		}

		private void AssignDynamicValue(Canvas ticketPrintLayout, TicketResponseData ticket)
		{
			foreach (UIElement element in ticketPrintLayout.Children)
			{
				if (!(element is TextBlock textblock) || string.IsNullOrEmpty(textblock.Name) || !textblock.Name.StartsWith("DT_"))
				{
					continue;
				}

				var startIndex = textblock.Name.IndexOf("_");
				var endIndex = textblock.Name.LastIndexOf("_");

				if (startIndex != -1 && endIndex != -1)
				{
					var propertyName = textblock.Name.Substring(startIndex + 1, endIndex - startIndex - 1);
					var type = ticket.GetType();
					var property = type.GetProperty(propertyName);
					if (property == null)
					{
						Logger.Error($"在{nameof(TicketResponseData)}中没找到查询{propertyName}对应的属性。");
						continue;
					}
					var value = property.GetValue(ticket);
					if (value == null)
					{
						Logger.Error($"在{nameof(TicketResponseData)} {propertyName}对应的值为null。");
						continue;
					}
					textblock.Text = value.ToString();
				}
			}
		}

		private void CollectImageSettings(PrintModel printModel, Canvas ticketPrintLayout, TicketResponseData ticket)
		{
			foreach (UIElement element in ticketPrintLayout.Children)
			{
				if (!(element is Image))
				{
					continue;
				}

				Image img = element as Image;
				var imgName = img.Name;
				if (imgName.Contains("two_") && !string.IsNullOrEmpty(ticket.uuid))
				{
					printModel.ImageSetting.Add(imgName, ticket.uuid);
				}
				//else if (imgName.Contains("two_Static_") && img.ToolTip != null && !string.IsNullOrEmpty(img.ToolTip.ToString()))
				//{
				//	printModel.ImageSetting.Add(imgName, img.ToolTip.ToString());
				//}
			}
		}

		#endregion
	}
}