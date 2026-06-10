using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using SSTMTerminal.Helpers;
using SSTMTerminal.Images;
using SSTMTerminal.Models;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
	public interface IGetTicketByInputIDViewModel : IViewModel
	{
		ICommand InputNumberOrAlphaCommand { get; set; }
		ICommand BackToHomeCommand { get; set; }
		ICommand ConfirmCommand { get; set; }
		ICommand ClearCommand { get; set; }
		ICommand RemoveLastNumberCommand { get; set; }
		ICommand SwitchKeyboardCommand { get; set; }
		bool IsEnableConfirm { get; set; }
		string ConfirmImageSource { get; set; }
		string ID { get; set; }
		bool ShowClearButton { get; set; }
		bool ShowNumberKeyboard { get; set; }
		bool ShowAlphaKeyboard { get; set; }
		void Reset();
	}

	public class GetTicketByInputIDViewModel : ViewModelBase, IGetTicketByInputIDViewModel
	{
		public GetTicketByInputIDViewModel(ILogger logger) :
			base(logger)
		{
			InputNumberOrAlphaCommand = new DelegateCommand<object>(OnInputIdNumberOrAlphaCommand);
			SwitchKeyboardCommand = new DelegateCommand(OnSwitchKeyboardCommand);
			RemoveLastNumberCommand = new DelegateCommand(OnRemoveLastNumberOrAlphaCommand);
			ClearCommand = new DelegateCommand(OnClearIDCommand);
		}

		public ICommand InputNumberOrAlphaCommand { get; set; }
		public ICommand BackToHomeCommand { get; set; }
		public ICommand ConfirmCommand { get; set; }
		public ICommand ClearCommand { get; set; }
		public ICommand RemoveLastNumberCommand { get; set; }
		public ICommand SwitchKeyboardCommand { get; set; }

		private bool showClearButton = false;
		public bool ShowClearButton
		{
			get { return showClearButton; }
			set
			{
				if (value != showClearButton)
				{
					showClearButton = value;
					OnPropertyChanged(() => ShowClearButton);
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

		private string id = string.Empty;
		public string ID
		{
			get { return id; }
			set
			{
				if (value != id)
				{
					id = value;
					OnPropertyChanged(() => ID);

					NotifyRefreshUIStatus();
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

		private void OnSwitchKeyboardCommand()
		{
			ShowNumberKeyboard = !ShowNumberKeyboard;
			ShowAlphaKeyboard = !ShowAlphaKeyboard;
		}

		private void OnRemoveLastNumberOrAlphaCommand()
		{
			var length = ID.Length;
			if (length > 0)
			{
				ID = ID.Remove(length - 1, 1);
			}
		}

		private void OnClearIDCommand()
		{
			ID = string.Empty;
		}

		private void OnInputIdNumberOrAlphaCommand(object input)
		{
			if (ID.Length < ModelConstants.LengthOfValidID)
			{
				ID += input.ToString();
			}
		}

		#region Private Methods

		private void NotifyRefreshUIStatus()
		{
			UpdateClearAllButtonStatus();

			UpdateConfirmStatus();
		}

		private void UpdateClearAllButtonStatus()
		{
			ShowClearButton = !string.IsNullOrEmpty(ID);
		}

		public void UpdateConfirmStatus()
		{
			var id = ID.Trim();
			if (!string.IsNullOrEmpty(id) && NumberHelper.ValidateIdentityNumber(id))
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
			ID = string.Empty;
			ShowClearButton = false;
			ShowNumberKeyboard = true;
			ShowAlphaKeyboard = false;
		}
	}
}
