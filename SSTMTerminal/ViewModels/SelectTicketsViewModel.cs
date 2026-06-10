using Genesis;
using Genesis.Logging;
using SSTMTerminal.Images;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
	public interface ISelectTicketsViewModel : IViewModel
    {
        ICommand SelectAllCommand { get; set; }
        ICommand SelectCommand { get; set; }
        ICommand BackToHomeCommand { get; set; }
        ICommand ConfirmGetTicketCommand { get; set; }
        string IsAllCheckedImageSource { get; set; }
        string ConfirmGetTicketImageSource { get; set; }
        bool IsAllSelected { get; set; }
        ObservableCollection<TicketItemViewModel> Tickets { get; set; }
        void UpdateSelectAllStatus(bool isAllSelected, string checkedImageSource,
            string ticketItemImageSource, string confirmGetTicketImageSource);
        void UpdateSelectAllStatus();
        void Reset(bool needClearTickets);

        List<TicketItemViewModel> GetSelectedTickets();

        int CountDown { get; set; }

        bool IsTimerEnable { get; set; }
    }

    public class SelectTicketsViewModel : ViewModelBase, ISelectTicketsViewModel
    {
        public SelectTicketsViewModel(ILogger logger) :
            base(logger)
        {
            Tickets = new ObservableCollection<TicketItemViewModel>();
        }

        public ICommand SelectCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand BackToHomeCommand { get; set; }
        public ICommand ConfirmGetTicketCommand { get; set; }
        public bool IsAllSelected { get; set; }
        private string isAllCheckedImageSource = ImagePath.Unchecked;
        public string IsAllCheckedImageSource
        {
            get { return isAllCheckedImageSource; }
            set
            {
                if (value != isAllCheckedImageSource)
                {
                    isAllCheckedImageSource = value;
                    OnPropertyChanged(() => IsAllCheckedImageSource);
                }
            }
        }

        private string confirmGetTicketImageSource = ImagePath.ConfirmGetTicketDisabled;
        public string ConfirmGetTicketImageSource
        {
            get { return confirmGetTicketImageSource; }
            set
            {
                if (value != confirmGetTicketImageSource)
                {
                    confirmGetTicketImageSource = value;
                    OnPropertyChanged(() => ConfirmGetTicketImageSource);
                }

                if (confirmGetTicketImageSource == ImagePath.ConfirmGetTicketDisabled)
                {
                    IsEnableConfirm = false;
                }
                else
                {
                    IsEnableConfirm = true;
                }
            }
        }
        public ObservableCollection<TicketItemViewModel> Tickets { get; set; }

        public void UpdateSelectAllStatus(bool isAllSelected,
            string checkedImageSource,
            string ticketItemImageSource,
            string confirmGetTicketImageSource)
        {
            foreach (var ticket in Tickets)
            {
                if (ticket.CanSelect)
                {
                    ticket.UpdateSelectStatus(isAllSelected, checkedImageSource, ticketItemImageSource);
                }
            }

            if(Tickets.All(t => !t.CanSelect))
			{
                IsAllSelected = false;
                IsAllCheckedImageSource = ImagePath.Unchecked;
                ConfirmGetTicketImageSource = ImagePath.ConfirmGetTicketDisabled;
            }
			else
			{
                IsAllSelected = isAllSelected;
                IsAllCheckedImageSource = checkedImageSource;
                ConfirmGetTicketImageSource = confirmGetTicketImageSource;
            }
        }

        public void UpdateSelectAllStatus()
        {
            if (Tickets.Where(t => t.CanSelect).Any(t => !t.IsSelected))
            {
                IsAllSelected = false;
                IsAllCheckedImageSource = ImagePath.Unchecked;
            }
            if (Tickets.Any(t => t.IsSelected))
            {
                ConfirmGetTicketImageSource = ImagePath.ConfirmGetTicket;
            }
            if (Tickets.Where(t => t.CanSelect).All(t => t.IsSelected))
            {
                IsAllSelected = true;
                IsAllCheckedImageSource = ImagePath.Checked;
            }
            if (Tickets.All(t => !t.IsSelected))
            {
                ConfirmGetTicketImageSource = ImagePath.ConfirmGetTicketDisabled;
            }
        }

        public void Reset(bool needClearTickets)
		{
            if(needClearTickets)
			{
                Tickets?.Clear();
			}
            IsAllSelected = false;
            IsAllCheckedImageSource = ImagePath.Unchecked;
            ConfirmGetTicketImageSource = ImagePath.ConfirmGetTicketDisabled;
        }

		public List<TicketItemViewModel> GetSelectedTickets()
		{
            var selectedTickets = new List<TicketItemViewModel>();
            if (Tickets == null || Tickets.Count == 0 )
			{
                return selectedTickets;
            }

            foreach(var ticket in Tickets)
			{
                if(ticket.IsSelected)
				{
                    selectedTickets.Add(ticket);
                }
			}

            return selectedTickets;
		}

        private int countDown;
        public int CountDown
        {
            get { return countDown; }
            set
            {
                if (value != countDown)
                {
                    countDown = value;
                    OnPropertyChanged(() => CountDown);
                }
            }
        }

        private bool isTimerEnable;
        public bool IsTimerEnable
        {
            get { return isTimerEnable; }
            set
            {
                if (value != isTimerEnable)
                {
                    isTimerEnable = value;
                    OnPropertyChanged(() => IsTimerEnable);
                }
            }
        }

        private bool isEnableConfirm;
        public bool IsEnableConfirm
        {
            get { return isEnableConfirm; }
            set
            {
                if (value != isEnableConfirm)
                {
                    isEnableConfirm = value;
                    OnPropertyChanged(() => IsEnableConfirm);
                }
            }
        }
        
    }
}
