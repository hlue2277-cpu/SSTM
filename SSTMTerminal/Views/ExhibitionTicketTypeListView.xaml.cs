using SSTMTerminal.Enums;
using SSTMTerminal.Models;
using SSTMTerminal.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SSTMTerminal.Views
{
    /// <summary>
    /// ExhibitionTicketTypeView.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionTicketTypeListView : UserControl
	{
		public ExhibitionTicketTypeListView()
		{
			InitializeComponent();
		}

        private void OnExhibitionTicketTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.DataContext != null && this.DataContext is ExhibitionTicketTypeListViewModel vm)
            {
                vm.SelectExhibitionTicketTypeCommand.Execute(ExhibitionTicketTpyeListBox.SelectedItem);
            }
        }
    }
}
