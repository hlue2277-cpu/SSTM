using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using SSTMTerminal.Enums;
using SSTMTerminal.Images;
using SSTMTerminal.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
	public interface IVisitBookingOrderViewModel : IViewModel
	{
		ICommand ManualAddVisitorCommand { get; set; }
		ICommand SwipAddVisitorCommand { get; set; }
        ICommand SwipAddForeignerVisitorCommand { get; set; }
        ICommand DeleteVisitorCommand { get; set; }

        // need to go back to previous page (List Page).
        ICommand BackToHomeCommand { get; set; }
        ICommand ConfirmBookingCommand { get; set; }

        bool IsConfirmBookingEnabled { get; set; }
        string ConfirmBookingImageSource { get; set; }
        int VisitorCount { get; set; }
        string TimeSlot {  get; set; }
        // the ID card reader is ready or not.
        bool IsSwipingIDEnabled { get; set; }
        bool CanAddVisitor { get; set; }
        SlotItem SlotItem { get;}
        ObservableCollection<VisitorModel> Visitors { get; }

        void SetTimeSlot(SlotItem timeSlot);
        void AddVisitor(VisitorModel model);

		void Reset();
	}

	public class VisitBookingOrderViewModel : ViewModelBase, IVisitBookingOrderViewModel
    {
        public VisitBookingOrderViewModel(ILogger logger) :
			base(logger)
		{
            DateAndDay = DateTime.Today.ToString("yyyy-MM-dd dddd");
            DeleteVisitorCommand = new DelegateCommand<object>(OnDeleteVisitorCommand);
            RegionName = RegionNames.HomeRegion;
        }

        private void OnDeleteVisitorCommand(object obj)
        {
            if(obj != null && obj is VisitorModel visitor)
            {
                Visitors.Remove(visitor);
                VisitorCount--;
            }
        }

        public void AddVisitor(VisitorModel model)
        {
            if(model == null || string.IsNullOrWhiteSpace(model.ID)
                ||Visitors.Any(v => v.ID?.ToLower() == model.ID?.ToLower()))
            {
                Logger?.Information("列表中已经存在相同ID的游客！");
                return;
            }
            Visitors.Add(model);
            VisitorCount++;
        }

        public ICommand ManualAddVisitorCommand { get; set; }
        public ICommand SwipAddVisitorCommand { get; set; }
        public ICommand SwipAddForeignerVisitorCommand { get; set; }
        public ICommand DeleteVisitorCommand { get; set; }

        public ICommand BackToHomeCommand { get; set; }
        public ICommand ConfirmBookingCommand { get; set; }

        public void Reset()
		{
            Visitors.Clear();
            TimeSlot = string.Empty;
            VisitorCount = 0;
            IsConfirmBookingEnabled = false;
            ConfirmBookingImageSource = ImagePath.BookingTicketDisabled;
        }

        public void SetTimeSlot(SlotItem timeSlotItem)
        {
            TimeSlot = timeSlotItem.TimeRange;
            SlotItem = timeSlotItem;
        }

        public SlotItem SlotItem { get; private set; }


        private ObservableCollection<VisitorModel> visitors = new ObservableCollection<VisitorModel>();
        public ObservableCollection<VisitorModel> Visitors
        {
            get { return visitors; }
            set
            {
                if (value != visitors)
                {
                    visitors = value;
                    OnPropertyChanged(() => Visitors);
                }
            }
        }

        private int visitorCount = 0;
        public int VisitorCount
        {
            get { return visitorCount; }
            set
            {
                if (value != visitorCount)
                {
                    visitorCount = value;
                    OnPropertyChanged(() => VisitorCount);
                }
                if(visitorCount == 0)
                {
                    IsConfirmBookingEnabled=false;
                    ConfirmBookingImageSource=ImagePath.BookingTicketDisabled;
                }
                else
                {
                    IsConfirmBookingEnabled=true;
                    ConfirmBookingImageSource = ImagePath.BookingTicket;
                }

                if (visitorCount >= 5)
                {
                    CanAddVisitor = false;
                }
                else
                {
                    CanAddVisitor = true;
                }
            }
        }

        private string timeSlot;
        public string TimeSlot
        {
            get { return timeSlot; }
            set
            {
                if (value != timeSlot)
                {
                    timeSlot = value;
                    OnPropertyChanged(() => TimeSlot);
                }
            }
        }

        private bool isConfirmBookingEnabled;
        public bool IsConfirmBookingEnabled
        {
            get { return isConfirmBookingEnabled; }
            set
            {
                if (value != isConfirmBookingEnabled)
                {
                    isConfirmBookingEnabled = value;
                    OnPropertyChanged(() => IsConfirmBookingEnabled);
                }
            }
        }

        private bool isSwipingIDEnabled;
        public bool IsSwipingIDEnabled
        {
            get { return isSwipingIDEnabled; }
            set
            {
                if (value != isSwipingIDEnabled)
                {
                    isSwipingIDEnabled = value;
                    OnPropertyChanged(() => IsSwipingIDEnabled);
                }
            }
        }

        private bool canAddVisitor = true;
        public bool CanAddVisitor
        {
            get { return canAddVisitor; }
            set
            {
                if (value != canAddVisitor)
                {
                    canAddVisitor = value;
                    OnPropertyChanged(() => CanAddVisitor);
                }
            }
        }

        private string confirmBookingImageSource = ImagePath.BookingTicketDisabled;
        public string ConfirmBookingImageSource
        {
            get { return confirmBookingImageSource; }
            set
            {
                if (value != confirmBookingImageSource)
                {
                    confirmBookingImageSource = value;
                    OnPropertyChanged(() => ConfirmBookingImageSource);
                }
            }
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
    }
}
