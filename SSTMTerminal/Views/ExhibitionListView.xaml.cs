using SSTMTerminal.Enums;
using SSTMTerminal.Models;
using SSTMTerminal.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SSTMTerminal.Views
{
    /// <summary>
    /// ExhibitionListView.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionListView : UserControl
	{
		public ExhibitionListView()
		{
			InitializeComponent();
		}

        private void OnExhibitionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.DataContext != null && this.DataContext is ExhibitionListViewModel vm)
            {
                vm.SelectExhibitionCommand.Execute(ExhibitionListBox.SelectedItem);
            }
        }
    }
}
