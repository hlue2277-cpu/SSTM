using Genesis;
using Genesis.Logging;
using System;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IExhibitionPrintTicketViewModel : IPrintingViewModel
    {
        string DateAndDay { get; set; }
        string WaitingForPrintMessage { get; set; }
		void Reset();
	}

	public class ExhibitionPrintTicketViewModel : PrintingViewModel, IExhibitionPrintTicketViewModel
    {
		public ExhibitionPrintTicketViewModel(ILogger logger) :
			base(logger)
		{
            DateAndDay = DateTime.Today.ToString("yyyy-MM-dd dddd");
        }

        public void Reset()
		{
		}

        private string dateAndDay = string.Empty;
        public string DateAndDay
        {
            get { return dateAndDay; }
            set
            {
                if (value != dateAndDay)
                {
                    dateAndDay = value;
                    OnPropertyChanged(() => DateAndDay);
                }
            }
        }

        private string waitingForPrintMessage = string.Empty;
        public string WaitingForPrintMessage
        {
            get { return waitingForPrintMessage; }
            set
            {
                if (value != waitingForPrintMessage)
                {
                    waitingForPrintMessage = value;
                    OnPropertyChanged(() => WaitingForPrintMessage);
                }
            }
        }
    }
}
