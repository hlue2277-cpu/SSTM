using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using SSTMTerminal.Enums;
using SSTMTerminal.Images;
using SSTMTerminal.Models;
using System;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
	public interface IGetTicketByCellAndOrderViewModel : IViewModel
	{
		ICommand InputNumberOrAlphaCommand { get; set; }
		ICommand BackToHomeCommand { get; set; }
		ICommand ConfirmCommand { get; set; }
		ICommand ClearCellCommand { get; set; }
		ICommand RemoveLastNumberCommand { get; set; }
		ICommand SwitchKeyboardCommand { get; set; }
		ICommand FieldFocusedCommand { get; set; }
		string ConfirmImageSource { get; set; }
		string CellNumber { get; set; }
		string OrderNumber { get; set; }
		bool ShowClearCellButton { get; set; }
		bool IsEnableConfirm { get; set; }
		InputNumberType InputNumberType { get; set; }
		bool ShowNumberKeyboard { get; set; }
		bool ShowAlphaKeyboard { get; set; }

		void Reset();
	}

	public class GetTicketByCellAndOrderViewModel : ViewModelBase, IGetTicketByCellAndOrderViewModel
	{
		public GetTicketByCellAndOrderViewModel(ILogger logger) :
			base(logger)
		{
			FieldFocusedCommand = new DelegateCommand<object>(OnFieldFocusedCommand);
			RemoveLastNumberCommand = new DelegateCommand(OnRemoveLastNumberCommand);
			ClearCellCommand = new DelegateCommand(OnClearCellNumberCommand);
			InputNumberOrAlphaCommand = new DelegateCommand<object>(OnInputIdNumberCommand);
			SwitchKeyboardCommand = new DelegateCommand(OnSwitchKeyboardCommand);
		}

		public ICommand InputNumberOrAlphaCommand { get; set; }
		public ICommand BackToHomeCommand { get; set; }
		public ICommand ConfirmCommand { get; set; }
		public ICommand ClearCellCommand { get; set; }
		public ICommand RemoveLastNumberCommand { get; set; }
		public ICommand SwitchKeyboardCommand { get; set; }
		public ICommand FieldFocusedCommand { get; set; } 
		public InputNumberType InputNumberType { get; set; } = InputNumberType.CellNumber;
		
		private bool showClearButton = false;
		public bool ShowClearCellButton
		{
			get { return showClearButton; }
			set
			{
				if (value != showClearButton)
				{
					showClearButton = value;
					OnPropertyChanged(() => ShowClearCellButton);
				}
			}
		}

		private string cellNumber = string.Empty;
		public string CellNumber
		{
			get { return cellNumber; }
			set
			{
				if (value != cellNumber)
				{
					cellNumber = value;
					OnPropertyChanged(() => CellNumber);

					NotifyRefreshUIStatus();
				}
			}
		}

		private string orderNumber = string.Empty;
		public string OrderNumber
		{
			get { return orderNumber; }
			set
			{
				if (value != orderNumber)
				{
					orderNumber = value;
					OnPropertyChanged(() => OrderNumber);

					NotifyRefreshUIStatus();
				}
			}
		}

		bool showNumberKeyboard = true;
		public bool ShowNumberKeyboard
		{
			get { return showNumberKeyboard; }
			set
			{
				if (value != showNumberKeyboard)
				{
					showNumberKeyboard = value;
					OnPropertyChanged(() => ShowNumberKeyboard);
				}
			}
		}

		bool showAlphaKeyboard = false;
		public bool ShowAlphaKeyboard
		{
			get { return showAlphaKeyboard; }
			set
			{
				if (value != showAlphaKeyboard)
				{
					showAlphaKeyboard = value;
					OnPropertyChanged(() => ShowAlphaKeyboard);
				}
			}
		}

		private bool isEnableConfirm { get; set; }
		public bool IsEnableConfirm
		{
			get { return isEnableConfirm; }
			set
			{
				if (value != isEnableConfirm)
				{
					isEnableConfirm = value;
					OnPropertyChanged(() => IsEnableConfirm);
				}
			}
		}

		private string confirmImageSource = ImagePath.ConfirmDisabled;
		public string ConfirmImageSource
		{
			get { return confirmImageSource; }
			set
			{
				if (value != confirmImageSource)
				{
					confirmImageSource = value;
					OnPropertyChanged(() => ConfirmImageSource);
				}
			}
		}

		private void OnFieldFocusedCommand(object inputType)
		{
			if (Enum.TryParse(inputType.ToString(), out InputNumberType type))
			{
				InputNumberType = type;
			}
		}

		private void OnRemoveLastNumberCommand()
		{
			var inputNumberType = InputNumberType;
			switch (inputNumberType)
			{
				case InputNumberType.CellNumber:
					var lengthOfCell = CellNumber.Length;
					if (lengthOfCell > 0)
					{
						CellNumber = CellNumber.Remove(lengthOfCell - 1, 1);
					}
					break;
				case InputNumberType.OrderNumber:
					var lengthOfOrder = OrderNumber.Length;
					if (lengthOfOrder > 0)
					{
						OrderNumber = OrderNumber.Remove(lengthOfOrder - 1, 1);
					}
					break;
				default:
					break;
			}
		}

		private void OnClearCellNumberCommand()
		{
			CellNumber = string.Empty;
			InputNumberType = InputNumberType.CellNumber;
		}

		private void OnInputIdNumberCommand(object number)
		{
			var inputNumberType = InputNumberType;
			switch (inputNumberType)
			{
				case InputNumberType.CellNumber:
					if (CellNumber.Length < ModelConstants.LengthOfValidCellNo)
					{
						CellNumber += number.ToString();
					}
					break;
				case InputNumberType.OrderNumber:
					if (OrderNumber.Length < ModelConstants.LengthOfValidOrderNo)
					{
						OrderNumber += number.ToString();
					}
					break;
				default:
					break;
			}
		}

		private void OnSwitchKeyboardCommand()
		{
			ShowNumberKeyboard = !ShowNumberKeyboard;
			ShowAlphaKeyboard = !ShowAlphaKeyboard;
		}

		#region Private Methods

		private void NotifyRefreshUIStatus()
		{
			UpdateClearAllButtonStatus();

			UpdateConfirmStatus();
		}

		private void UpdateClearAllButtonStatus()
		{
			ShowClearCellButton = !string.IsNullOrEmpty(CellNumber);
		}

		private void UpdateConfirmStatus()
		{
			if (CellNumber.Length == ModelConstants.LengthOfValidCellNo && 
				OrderNumber.Length == ModelConstants.LengthOfValidOrderNo)
			{
				ConfirmImageSource = ImagePath.Confirm;
				IsEnableConfirm = true;
			}
			else
			{
				ConfirmImageSource = ImagePath.ConfirmDisabled;
				IsEnableConfirm = false;
			}
		}

		#endregion

		public void Reset()
		{
			CellNumber = string.Empty;
			OrderNumber = string.Empty;
			ShowClearCellButton = false;
			ShowNumberKeyboard = true;
			ShowAlphaKeyboard = false;
			InputNumberType = InputNumberType.CellNumber;
		}
	}
}
