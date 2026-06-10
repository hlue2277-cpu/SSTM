using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using SSTMTerminal.Helpers;
using SSTMTerminal.Images;
using SSTMTerminal.Models;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IExhibitionManualAddVisitorViewModel : IViewModel
	{
        ICommand InputNumberOrAlphaCommand { get; set; }
        ICommand RemoveLastNumberCommand { get; set; }
        ICommand SwitchKeyboardCommand { get; set; }
        ICommand ClearCommand { get; set; }
        bool ShowNumberKeyboard { get; set; }
        bool ShowAlphaKeyboard { get; set; }

        bool ShowClearButton { get; set; }

        ICommand UseIDCardCommand { get; set; }
        ICommand UsePassportCommand { get; set; }
        ICommand UseResidentCommand { get; set; }
        ICommand UseForeignIdcardCommand { get; set; }
        ICommand ConfirmAddVisitorCommand { get; set; }
        ICommand BackToOwnerCommand { get; set; }

        string ConfirmAddVisitorImageSource { get; set; }
        string UseResidentImageSource { get; set; }
        string UsePassportImageSource { get; set; }
        string UseIDCardImageSource { get; set; }
        string UseForeignIdcardImageSource { get; set; }

        string Name { get; set; }
        string ID { get; set; }
        string CertType { get; set; }

        void Reset();
	}

	public class ExhibitionManualAddVisitorViewModel : ViewModelBase, IExhibitionManualAddVisitorViewModel
    {
		public ExhibitionManualAddVisitorViewModel(ILogger logger) :
			base(logger)
		{
            CertType = CertTypes.IDCard;
            UseIDCardCommand = new DelegateCommand(OnUseIDCardCommand);
            UsePassportCommand = new DelegateCommand(OnUsePassportCommand);
            UseResidentCommand = new DelegateCommand(OnUseResidentCommand);
            UseForeignIdcardCommand = new DelegateCommand(OnUseForeignIdcardCommand);

            InputNumberOrAlphaCommand = new DelegateCommand<object>(OnInputIdNumberOrAlphaCommand);
            SwitchKeyboardCommand = new DelegateCommand(OnSwitchKeyboardCommand);
            RemoveLastNumberCommand = new DelegateCommand(OnRemoveLastNumberOrAlphaCommand);
            ClearCommand = new DelegateCommand(OnClearIDCommand);
        }

        private void OnUseForeignIdcardCommand()
        {
            CertType = CertTypes.ForeignIdcard;
            UseIDCardImageSource = ImagePath.UseIDCardBtnNormal;
            UsePassportImageSource = ImagePath.UsePassportBtnNormal;
            UseResidentImageSource = ImagePath.UseResidentBtnNormal;
            UseForeignIdcardImageSource = ImagePath.UseForeignIdcardBtnSelected;
        }

        private void OnUseResidentCommand()
        {
            CertType = CertTypes.Residence;
            UseIDCardImageSource = ImagePath.UseIDCardBtnNormal;
            UsePassportImageSource = ImagePath.UsePassportBtnNormal;
            UseResidentImageSource = ImagePath.UseResidentBtnSelected;
            UseForeignIdcardImageSource = ImagePath.UseForeignIdcardBtnNormal;
        }

        private void OnUsePassportCommand()
        {
            CertType = CertTypes.Passport;
            UseIDCardImageSource = ImagePath.UseIDCardBtnNormal;
            UsePassportImageSource = ImagePath.UsePassportBtnSelected;
            UseResidentImageSource = ImagePath.UseResidentBtnNormal;
            UseForeignIdcardImageSource = ImagePath.UseForeignIdcardBtnNormal;
        }

        private void OnUseIDCardCommand()
        {
            CertType = CertTypes.IDCard;
            UseIDCardImageSource = ImagePath.UseIDCardBtnSelected;
            UsePassportImageSource = ImagePath.UsePassportBtnNormal;
            UseResidentImageSource = ImagePath.UseResidentBtnNormal;
            UseForeignIdcardImageSource = ImagePath.UseForeignIdcardBtnNormal;
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

        private void OnInputIdNumberOrAlphaCommand(object input)
        {
            if (CertType == CertTypes.IDCard && ID.Length < ModelConstants.LengthOfValidID)
            {
                ID += input.ToString();
            }
            else if(CertType != CertTypes.IDCard)
            {
                ID += input.ToString();
            }
        }

        private void OnClearIDCommand()
        {
            ID = string.Empty;
        }

        public ICommand UseIDCardCommand { get; set; }
        public ICommand UsePassportCommand { get; set; }
        public ICommand UseResidentCommand { get; set; }
        public ICommand UseForeignIdcardCommand { get; set; }
        public ICommand ConfirmAddVisitorCommand { get; set; }
        public ICommand BackToOwnerCommand { get; set; }
        public ICommand InputNumberOrAlphaCommand { get; set; }
        public ICommand RemoveLastNumberCommand { get; set; }
        public ICommand SwitchKeyboardCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        
        public string CertType { get; set; }

        public void Reset()
		{
            CertType = CertTypes.IDCard;
            UseIDCardImageSource = ImagePath.UseIDCardBtnSelected;
            UsePassportImageSource = ImagePath.UsePassportBtnNormal;
            UseResidentImageSource = ImagePath.UseResidentBtnNormal;
            UseForeignIdcardImageSource = ImagePath.UseForeignIdcardBtnNormal;

            Name = string.Empty;
            ID = string.Empty;
            ShowClearButton = false;
            ShowNumberKeyboard = true;
            ShowAlphaKeyboard = false;
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged(() => Name);
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
                }

                NotifyRefreshUIStatus();
            }
        }

        private string useIDCardImageSource = ImagePath.UseIDCardBtnSelected;
        public string UseIDCardImageSource
        {
            get { return useIDCardImageSource; }
            set
            {
                if (value != useIDCardImageSource)
                {
                    useIDCardImageSource = value;
                    OnPropertyChanged(() => UseIDCardImageSource);
                }
            }
        }

        private string usePassportImageSource = ImagePath.UsePassportBtnNormal;
        public string UsePassportImageSource
        {
            get { return usePassportImageSource; }
            set
            {
                if (value != usePassportImageSource)
                {
                    usePassportImageSource = value;
                    OnPropertyChanged(() => UsePassportImageSource);
                }
            }
        }

        private string useResidentImageSource = ImagePath.UseResidentBtnNormal;
        public string UseResidentImageSource
        {
            get { return useResidentImageSource; }
            set
            {
                if (value != useResidentImageSource)
                {
                    useResidentImageSource = value;
                    OnPropertyChanged(() => UseResidentImageSource);
                }
            }
        }

        private string confirmAddVisitorImageSource = ImagePath.ConfirmDisabled;
        public string ConfirmAddVisitorImageSource
        {
            get { return confirmAddVisitorImageSource; }
            set
            {
                if (value != confirmAddVisitorImageSource)
                {
                    confirmAddVisitorImageSource = value;
                    OnPropertyChanged(() => ConfirmAddVisitorImageSource);
                }
            }
        }

        private string useForeignIdcardImageSource = ImagePath.UseForeignIdcardBtnNormal;
        public string UseForeignIdcardImageSource
        {
            get { return useForeignIdcardImageSource; }
            set
            {
                if (value != useForeignIdcardImageSource)
                {
                    useForeignIdcardImageSource = value;
                    OnPropertyChanged(() => UseForeignIdcardImageSource);
                }
            }
        }

        private bool isConfirmAddVisitorEnabled = false;
        public bool IsConfirmAddVisitorEnabled
        {
            get { return isConfirmAddVisitorEnabled; }
            set
            {
                if (value != isConfirmAddVisitorEnabled)
                {
                    isConfirmAddVisitorEnabled = value;
                    OnPropertyChanged(() => IsConfirmAddVisitorEnabled);
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
                IsConfirmAddVisitorEnabled = true;
                ConfirmAddVisitorImageSource = ImagePath.ConfirmRed;
            }
            else
            {
                IsConfirmAddVisitorEnabled = false;
                ConfirmAddVisitorImageSource = ImagePath.ConfirmDisabled;
            }
        }
    }
}
