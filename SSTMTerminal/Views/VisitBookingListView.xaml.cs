using SSTMTerminal.Enums;
using SSTMTerminal.Models;
using SSTMTerminal.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SSTMTerminal.Views
{
    /// <summary>
    /// VisitBookingView.xaml 的交互逻辑
    /// </summary>
    public partial class VisitBookingListView : UserControl
	{
		public VisitBookingListView()
		{
			InitializeComponent();
		}

        private void OnTimeSlotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.DataContext != null && this.DataContext is VisitBookingListViewModel vm)
            {
                vm.SelectTimeSlotCommand.Execute(TimeSlotsListBox.SelectedItem);
            }
        }
    }
}
