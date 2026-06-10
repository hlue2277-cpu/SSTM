using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using SSTMTerminal.Enums;
using SSTMTerminal.Images;
using SSTMTerminal.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
	public interface IVisitBookingListViewModel : IViewModel
	{
		ICommand SelectTimeSlotCommand { get; set; }
        ICommand BackToHomeCommand { get; set; }
        string DateAndDay { get; set; }
        void SetTimeSlots(TimeSlotsModel timeSlotModel);

		void Reset();
	}

	public class VisitBookingListViewModel : ViewModelBase, IVisitBookingListViewModel
    {
		public VisitBookingListViewModel(ILogger logger) :
			base(logger)
		{
            DateAndDay = DateTime.Today.ToString("yyyy-MM-dd dddd");
        }

		public ICommand SelectTimeSlotCommand { get; set; }
        public ICommand BackToHomeCommand { get; set; }

        public void Reset()
		{
            TimeSlots.Clear();
		}

        public void SetTimeSlots(TimeSlotsModel timeSlotModel)
        {
            TimeSlots.Clear();
            foreach(var item in timeSlotModel.Slots)
            {
                TimeSlots.Add(item);
            }
        }

        private ObservableCollection<SlotItem> timeSlots = new ObservableCollection<SlotItem>();
        public ObservableCollection<SlotItem> TimeSlots
        {
            get { return timeSlots; }
            set
            {
                if (value != timeSlots)
                {
                    timeSlots = value;
                    OnPropertyChanged(() => TimeSlots);
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
