using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using SSTMTerminal.Enums;
using SSTMTerminal.Images;
using SSTMTerminal.Models;
using SSTMTerminal.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
	public interface IExhibitionTicketTypeListViewModel : IViewModel
	{
		ICommand SelectExhibitionTicketTypeCommand { get; set; }
        ICommand BackToHomeCommand { get; set; }
        string DateAndDay { get; set; }
        void SetExhibitionTimeSlotAndTicketTypes(SlotItem ticketTimeSlot, ExhibitionTicketTypesModel exhibitionTicketTypesModel);

        string ExhibitionName { get; set; }
        SlotItem TicketTimeSlot { get; set; }

        double TicketTypeDescriptionFontSize { get; set; }

        void Reset();
	}

	public class ExhibitionTicketTypeListViewModel : ViewModelBase, IExhibitionTicketTypeListViewModel
{
		public ExhibitionTicketTypeListViewModel(ILogger logger) :
			base(logger)
		{
            DateAndDay = DateTime.Today.ToString("yyyy-MM-dd dddd");
            TicketTypeDescriptionFontSize = double.Parse(ConfigurationManager.AppSettings.Get("ExhibitionTicketTypeDescriptionFontSize"));
        }

        public SlotItem TicketTimeSlot { get; set; }
        public ICommand SelectExhibitionTicketTypeCommand { get; set; }
        public ICommand BackToHomeCommand { get; set; }

        public void Reset()
		{
            // TicketTimeSlot should not be reset to null here. It is used after Reset
            ExhibitionName = string.Empty;
            ExhibitionTicketTypes.Clear();
		}

        public void SetExhibitionTimeSlotAndTicketTypes(SlotItem ticketTimeSlot, ExhibitionTicketTypesModel exhibitionTicketTypesModel)
        {
            ExhibitionName = string.Empty;
            TicketTimeSlot = ticketTimeSlot;
            ExhibitionTicketTypes.Clear();

            ExhibitionName = exhibitionTicketTypesModel.ExhibitionName;
            foreach (var item in exhibitionTicketTypesModel.ExhibitionTicketTypes)
            {
                ExhibitionTicketTypes.Add(item);
            }
        }

        private ObservableCollection<ExhibitionTicketType> exhibitionTicketTypes = new ObservableCollection<ExhibitionTicketType>();
        public ObservableCollection<ExhibitionTicketType> ExhibitionTicketTypes
        {
            get { return exhibitionTicketTypes; }
            set
            {
                if (value != exhibitionTicketTypes)
                {
                    exhibitionTicketTypes = value;
                    OnPropertyChanged(() => ExhibitionTicketTypes);
                }
            }
        }

        private string exhibitionName = string.Empty;
        public string ExhibitionName
        {
            get { return exhibitionName; }
            set
            {
                if (value != exhibitionName)
                {
                    exhibitionName = value;
                    OnPropertyChanged(() => ExhibitionName);
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

        private double ticketTypeDescriptionFontSize = 20.0;
        public double TicketTypeDescriptionFontSize
        {
            get { return ticketTypeDescriptionFontSize; }
            set
            {
                if (value != ticketTypeDescriptionFontSize)
                {
                    ticketTypeDescriptionFontSize = value;
                    OnPropertyChanged(() => TicketTypeDescriptionFontSize);
                }
            }
        }
    }
}
