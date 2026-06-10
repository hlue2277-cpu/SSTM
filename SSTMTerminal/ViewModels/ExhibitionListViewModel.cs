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
	public interface IExhibitionListViewModel : IViewModel
	{
		ICommand SelectExhibitionCommand { get; set; }
        ICommand BackToHomeCommand { get; set; }
        string DateAndDay { get; set; }
        void SetExhibitions(ExhibitionsModel exhibitionsModel);

        ExhibitionItem CurrentExhibitionItem { get; set; }
        ExhibitionTicketType CurrentExhibitionTicketType { get; set; }

        void Reset();
	}

	public class ExhibitionListViewModel : ViewModelBase, IExhibitionListViewModel
    {
		public ExhibitionListViewModel(ILogger logger) :
			base(logger)
		{
            DateAndDay = DateTime.Today.ToString("yyyy-MM-dd dddd");
        }

		public ICommand SelectExhibitionCommand { get; set; }
        public ICommand BackToHomeCommand { get; set; }

        public void Reset()
		{
            Exhibitions.Clear();
		}

        public void SetExhibitions(ExhibitionsModel exhibitionsModel)
        {
            Exhibitions.Clear();
            foreach(var item in exhibitionsModel.Exhibitions)
            {
                Exhibitions.Add(item);
            }
        }

        private ObservableCollection<ExhibitionItem> exhibitions = new ObservableCollection<ExhibitionItem>();
        public ObservableCollection<ExhibitionItem> Exhibitions
        {
            get { return exhibitions; }
            set
            {
                if (value != exhibitions)
                {
                    exhibitions = value;
                    OnPropertyChanged(() => Exhibitions);
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

        public ExhibitionItem CurrentExhibitionItem { get; set; }
        public ExhibitionTicketType CurrentExhibitionTicketType { get; set; }
    }
}
