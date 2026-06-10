using SSTMTerminal.Enums;
using SSTMTerminal.Models;
using SSTMTerminal.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SSTMTerminal.Views
{
    /// <summary>
    /// ExhibitionManualAddVisitorView.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionManualAddVisitorView : UserControl
	{
		public ExhibitionManualAddVisitorView()
		{
			InitializeComponent();
		}

        private void InputNumberOrAlpha_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string text = txtID.Text;
            var idx = txtID.SelectionStart;
            text = text.Insert(idx, button.Tag.ToString());

            txtID.Text = text;
            txtID.SelectionStart = idx + 1;
            txtID.CaretIndex = idx + 1;
            txtID.Focus();
        }

        private void RemoveNumberOrAlpha_Click(object sender, RoutedEventArgs e)
        {
            string s = txtID.Text;
            var idx = txtID.SelectionStart;
            if (idx > 0)
            {
                txtID.Text = s.Substring(0, idx - 1) + s.Substring(idx);
                txtID.SelectionStart = idx - 1;
                txtID.CaretIndex = idx - 1;
            }
            else
            {
                txtID.SelectionStart = 0;
                txtID.CaretIndex = 0;
            }
            txtID.Focus();
        }
    }
}
