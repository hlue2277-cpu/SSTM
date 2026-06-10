using DataService;
using Genesis;
using SSTMTerminal.Images;

namespace SSTMTerminal.ViewModels
{
	public class TicketItemViewModel : ViewModelBase
    {
        public TicketItemViewModel(TicketResponseData ticketData)
		{
            TicketFromServer = ticketData;
            TicketName = TicketFromServer.scheduleCnName;
            TicketDetail = TicketFromServer.seatLabel;
            CanSelect = TicketFromServer.printable;
            ShowStatusText = !TicketFromServer.printable;
            StatusText = TicketFromServer.statusText;
        }

        public void UpdateSelectStatus(bool isSelected,
            string checkedImageSource,
            string ticketItemImageSource)
        {
            IsSelected = isSelected;
            IsCheckedImageSource = checkedImageSource;
            TicketItemBackgroundImageSource = ticketItemImageSource;
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (CanSelect && value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(() => IsSelected);
                }
            }
        }
        
        private string isCheckedImageSource = ImagePath.Unchecked;
        public string IsCheckedImageSource
        {
            get { return isCheckedImageSource; }
            set
            {
                if (value != isCheckedImageSource)
                {
                    isCheckedImageSource = value;
                    OnPropertyChanged(() => IsCheckedImageSource);
                }
            }
        }

        private string ticketItemBackgroundImageSource = ImagePath.TicketItemRectangleDisabled;
        public string TicketItemBackgroundImageSource
        {
            get { return ticketItemBackgroundImageSource; }
            set
            {
                if (value != ticketItemBackgroundImageSource)
                {
                    ticketItemBackgroundImageSource = value;
                    OnPropertyChanged(() => TicketItemBackgroundImageSource);
                }
            }
        }

        private string name;
        public string TicketName
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged(() => TicketName);
                }
            }
        }

        private string ticketDetail;
        public string TicketDetail
        {
            get { return ticketDetail; }
            set
            {
                if (ticketDetail != value)
                {
                    ticketDetail = value;
                    OnPropertyChanged(() => TicketDetail);
                }
            }
        }

        private string statusText;
        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (statusText != value)
                {
                    statusText = value;
                    OnPropertyChanged(() => StatusText);
                }
            }
        }

        private bool showStatusText;
        public bool ShowStatusText
        {
            get { return showStatusText; }
            set
            {
                if (showStatusText != value)
                {
                    showStatusText = value;
                    OnPropertyChanged(() => ShowStatusText);
                }
            }
        }

        private bool canSelect;
        public bool CanSelect
        {
            get { return canSelect; }
            set
            {
                if (canSelect != value)
                {
                    canSelect = value;
                    OnPropertyChanged(() => CanSelect);
                }
            }
        }

        public TicketResponseData TicketFromServer { get; private set; }
    }
}
