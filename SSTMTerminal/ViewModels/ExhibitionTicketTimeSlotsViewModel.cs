using Genesis;
using Genesis.Logging;
using SSTMTerminal.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IExhibitionTicketTimeSlotsViewModel : IViewModel
	{
		ICommand SelectExhibitionTicketTimeSlotCommand { get; set; }
        ICommand BackToHomeCommand { get; set; }
        string DateAndDay { get; set; }
        void SetTimeSlots(TimeSlotsModel exhibitionTicketTimeSlotsModel);

        void Reset();
	}

	public class ExhibitionTicketTimeSlotsViewModel : ViewModelBase, IExhibitionTicketTimeSlotsViewModel
    {
		public ExhibitionTicketTimeSlotsViewModel(ILogger logger) :
			base(logger)
		{
            DateAndDay = DateTime.Today.ToString("yyyy-MM-dd dddd");
        }

		public ICommand SelectExhibitionTicketTimeSlotCommand { get; set; }
        public ICommand BackToHomeCommand { get; set; }

        public void Reset()
		{
            TimeSlots.Clear();
		}

        public void SetTimeSlots(TimeSlotsModel exhibitionTicketTimeSlotsModel)
        {
            TimeSlots.Clear();

            foreach (var item in exhibitionTicketTimeSlotsModel.Slots)
            {
                TimeSlots.Add(item);
            }
        }

        //public void SetTimeSlots(TimeSlotsModel exhibitionTicketTimeSlotsModel)
        //{
        //    TimeSlots.Clear();
        //    if (exhibitionTicketTimeSlotsModel?.Slots == null) return;

        //    foreach (var item in exhibitionTicketTimeSlotsModel.Slots)
        //    {
        //        // 只显示特展相关（根据你的业务调整条件）
        //        if (item.type == "4" || item.type == "5" || item.sessionName.Contains("马丘比丘") || item.sessionName.Contains("玛雅"))
        //        {
        //            TimeSlots.Add(item);
        //        }
        //    }
        //}

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
