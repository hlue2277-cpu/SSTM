using Genesis;
using Genesis.Logging;
using System;
using System.Configuration;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IHomeViewModel : IViewModel
    {
        ICommand BuyExhibitionTicketCommand { get; set; }
        ICommand BuyExhibitionTicket2Command { get; set; }
        
        ICommand NavigateToGetTicketHomeCommand { get; set; }
        ICommand VisitBookingCommand { get; set; }
        string Version { get; set; }
        string NoticeImagePath { get; set; }
        bool ShowVisitButton { get; set; }
        bool ShowExhibitionButton { get; set; }
        bool ShowExhibition2Button { get; set; }
        bool ShowGetTicketButton { get; set; }

    }

    public class HomeViewModel : ViewModelBase, IHomeViewModel
    {
        public ICommand BuyExhibitionTicketCommand { get; set; }
        public ICommand BuyExhibitionTicket2Command { get; set; }

        public ICommand NavigateToGetTicketHomeCommand { get; set; }
        public ICommand VisitBookingCommand { get; set; }

        public HomeViewModel(ILogger logger) : base(logger)
        {
            try
            {
                ShowVisitButton = bool.Parse(ConfigurationManager.AppSettings.Get("ShowVisitButton"));
                ShowExhibitionButton = bool.Parse(ConfigurationManager.AppSettings.Get("ShowExhibitionButton"));
                ShowExhibition2Button = bool.Parse(ConfigurationManager.AppSettings.Get("ShowExhibition2Button"));
                ShowGetTicketButton = bool.Parse(ConfigurationManager.AppSettings.Get("ShowGetTicketButton"));
            }
            catch (Exception ex)
            {
                ShowVisitButton = false;
                ShowExhibitionButton = true;
                ShowExhibition2Button = true;
                ShowGetTicketButton = false;
                Logger.Log(LogLevel.Error, ex, "自助取票主界面按钮配置异常！", null);
            }
        }

        private bool showVisitButton;

        public bool ShowVisitButton
        {
            get { return showVisitButton; }
            set
            {
                if (value != showVisitButton)
                {
                    showVisitButton = value;
                    OnPropertyChanged(() => ShowVisitButton);
                }
            }
        }

        private bool showExhibitionButton;

        public bool ShowExhibitionButton
        {
            get { return showExhibitionButton; }
            set
            {
                if (value != showExhibitionButton)
                {
                    showExhibitionButton = value;
                    OnPropertyChanged(() => ShowExhibitionButton);
                }
            }
        }

        private bool showExhibition2Button;

        public bool ShowExhibition2Button
        {
            get { return showExhibition2Button; }
            set
            {
                if (value != showExhibition2Button)
                {
                    showExhibition2Button = value;
                    OnPropertyChanged(() => ShowExhibition2Button);
                }
            }
        }

        private bool showGetTicketButton;

        public bool ShowGetTicketButton
        {
            get { return showGetTicketButton; }
            set
            {
                if (value != showGetTicketButton)
                {
                    showGetTicketButton = value;
                    OnPropertyChanged(() => ShowGetTicketButton);
                }
            }
        }

        private string version;
        public string Version
        {
            get { return version; }
            set
            {
                if (value != version)
                {
                    version = value;
                    OnPropertyChanged(() => Version);
                }
            }
        }

        private string noticeImagePath;
        public string NoticeImagePath
        {
            get { return noticeImagePath; }
            set
            {
                if (value != noticeImagePath)
                {
                    noticeImagePath = value;
                    OnPropertyChanged(() => NoticeImagePath);
                }
            }
        }

    }
}
