using SSTMTerminal.Enums;
using SSTMTerminal.Models;
using SSTMTerminal.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SSTMTerminal.Views
{
    /// <summary>
    /// ExhibitionTicketDetailView.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionTicketTimeSlotsView : UserControl
	{
		public ExhibitionTicketTimeSlotsView()
		{
			InitializeComponent();
		}

        private void OnExhibitionTicketTimeSlotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if(this.DataContext != null && this.DataContext is ExhibitionTicketTimeSlotsViewModel vm)
            //{
            //    vm.SelectExhibitionTicketTimeSlotCommand.Execute(ExhibitionTicketTimeSlotListBox.SelectedItem);  
            //}
        }
    }
}
