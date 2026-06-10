using DataService;
using Genesis;
using Genesis.Logging;
using Printing;
using SqliteUtil;
using SSTMTerminal.Helpers;
using SSTMTerminal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
	public interface IPrintingViewModel : IViewModel
	{
		string Message { get; set; }
		ICommand BackToHomeCommand { get; set; }
		void PrintTickets(PrintManager printManager, List<TicketItemViewModel> ticketsToPrint, string tradeNO = null);
		ICommand PrintFinishedCommand { get; set; }
		void OnPrintFinished(PrintManager printManager);

    }

	public class PrintingViewModel : ViewModelBase, IPrintingViewModel
	{
		public PrintingViewModel(ILogger logger) : base(logger)
		{
		}

		public ICommand BackToHomeCommand { get; set; }

		private Visibility backButtonVisibility;

		public Visibility BackButtonVisibility
		{
			get { return backButtonVisibility; }
			set
			{
				if (value != backButtonVisibility)
				{
					backButtonVisibility = value;
					OnPropertyChanged(() => BackButtonVisibility);
				}
			}
		}

		private string msg;
		public string Message
		{
			get { return msg; }
			set
			{
				if (value != msg)
				{
					msg = value;
					OnPropertyChanged(() => Message);
				}
			}
		}

		public ICommand PrintFinishedCommand { get; set; }

		public void PrintTickets(PrintManager printManager, List<TicketItemViewModel> ticketsToPrint, string tradeNO = null)
		{
			if(printManager == null)
			{
				return;
			}

            printManager.DocumentsPrintedEvent -= OnDocumentsPrintedEvent;
            printManager.DocumentsPrintedEvent += OnDocumentsPrintedEvent;

            if (ticketsToPrint == null || ticketsToPrint.Count == 0)
			{
				PrintFinishedCommand.Execute(null);
				return;
			}

			var printModels = printManager.GeneratePrintModels(ticketsToPrint.Select(t => t.TicketFromServer).ToList());
			if (printModels == null || printModels.Count == 0)
			{
				PrintFinishedCommand.Execute(null);
				return;
			}

			Task.Factory.StartNew(() =>
			{
				try
				{
                    printManager.Print(printModels, tradeNO);
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
					PrintFinishedCommand?.Execute(ex);
				}
			});
		}

		public void OnPrintFinished(PrintManager printManager)
		{
			if (printManager != null)
			{
                printManager.DocumentsPrintedEvent -= OnDocumentsPrintedEvent;
			}
        }

        // Happens when all tickets printed ok.
        private void OnDocumentsPrintedEvent(object sender, List<string> ticketsIds)
		{
			var printResults = new List<PrintResultModel>();
			for (int i = 0; i < ticketsIds.Count; i++)
			{
				printResults.Add(
					new PrintResultModel
					{
						hasprinted = true,
						resourceid = ticketsIds[i],
						printedtime = DateTime.Now
					});
			}

			PrintFinishedCommand?.Execute(printResults);
		}
	}
}