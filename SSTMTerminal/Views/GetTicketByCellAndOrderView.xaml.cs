using SSTMTerminal.Enums;
using SSTMTerminal.Models;
using System.Windows;
using System.Windows.Controls;

namespace SSTMTerminal.Views
{
	/// <summary>
	/// GetTicketByCellAndOrderView.xaml 的交互逻辑
	/// </summary>
	public partial class GetTicketByCellAndOrderView : UserControl
	{
		public GetTicketByCellAndOrderView()
		{
			InitializeComponent();
		}

		private void InputNumberOrAlpha_Click(object sender, RoutedEventArgs e)
		{
			if (!FieldHasMaxCharacters())
			{
				TextBox textBox = GetFocusedTextBox();
				var focusTextBox = textBox;

				var button = sender as Button;
				string text = focusTextBox.Text;
				var idx = focusTextBox.SelectionStart;
				text = text.Insert(idx, button.Tag.ToString());

				focusTextBox.Text = text;
				focusTextBox.SelectionStart = idx + 1;
				focusTextBox.CaretIndex = idx + 1;
				focusTextBox.Focus();
			}

			TryFocusOrderTextBox();
		}

		private void RemoveNumberOrAlpha_Click(object sender, RoutedEventArgs e)
		{
			TextBox focusTextBox = GetFocusedTextBox();

			string s = focusTextBox.Text;
			var idx = focusTextBox.SelectionStart;
			if (idx > 0)
			{
				focusTextBox.Text = s.Substring(0, idx - 1) + s.Substring(idx);
				focusTextBox.SelectionStart = idx - 1;
				focusTextBox.CaretIndex = idx - 1;
			}
			else
			{
				focusTextBox.SelectionStart = 0;
				focusTextBox.CaretIndex = 0;
			}
			focusTextBox.Focus();
		}

		private TextBox GetFocusedTextBox()
		{
			return InputNumberType == InputNumberType.CellNumber ? txtCell : txtOrder;
		}

		private void TryFocusOrderTextBox()
		{
			if(InputNumberType == InputNumberType.CellNumber 
				&& FieldHasMaxCharacters())
			{
				txtOrder.SelectionStart = 0;
				txtOrder.CaretIndex = 0;
				txtOrder.Focus();
			}
		}

		private bool FieldHasMaxCharacters()
		{
			if (InputNumberType == InputNumberType.CellNumber)
			{
				return txtCell.Text.Length >= ModelConstants.LengthOfValidCellNo;
			}
			else if (InputNumberType == InputNumberType.OrderNumber)
			{
				return txtOrder.Text.Length >= ModelConstants.LengthOfValidOrderNo;
			}

			return true;
		}

		private InputNumberType InputNumberType = InputNumberType.CellNumber;
		private void CellOrOrderTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (textBox == txtCell)
				{
					InputNumberType = InputNumberType.CellNumber;
				}
				else if (textBox == txtOrder)
				{
					InputNumberType = InputNumberType.OrderNumber;
				}
			}
		}
	}
}
