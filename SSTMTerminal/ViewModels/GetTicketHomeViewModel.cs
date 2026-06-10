using DataService;
using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using SSTMTerminal.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IGetTicketHomeViewModel : IViewModel
    {
        ICommand GetTicketBySwipingIDCommand { get; set; }
        ICommand GetTicketByInputIDCommand { get; set; }
        ICommand GetTicketByCellAndOrderCommand { get; set; }
        ICommand BackToHomeCommand { get; set; }
        bool ShowGetTicketBySwipIDButton { get; set; }
        bool ShowGetTicketByInputIDButton { get; set; }
        bool ShowGetTicketByCellAndOrderButton { get; set; }
        bool IsSwipingIDEnabled { get; set; }
        string Version { get; set; }
        string NoticeImagePath { get; set; }
    }

    public class GetTicketHomeViewModel : ViewModelBase, IGetTicketHomeViewModel
    {
        public ICommand GetTicketBySwipingIDCommand { get; set; }
        public ICommand GetTicketByInputIDCommand { get; set; }
        public ICommand GetTicketByCellAndOrderCommand { get; set; }
        public ICommand BackToHomeCommand { get; set; }

        public ITicketService TicketService { get; set; }

        public GetTicketHomeViewModel(ILogger logger) : base(logger)
        {
            try
            {
                ShowGetTicketBySwipIDButton = bool.Parse(ConfigurationManager.AppSettings.Get("ShowGetTicketBySwipIDButton"));
                ShowGetTicketByInputIDButton = bool.Parse(ConfigurationManager.AppSettings.Get("ShowGetTicketByInputIDButton"));
                ShowGetTicketByCellAndOrderButton = bool.Parse(ConfigurationManager.AppSettings.Get("ShowGetTicketByCellAndOrderButton"));
            }
            catch(Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "自助取票主界面按钮配置异常！", null);
            }
        }

        private bool isSwipingIDEnabled;

        public bool IsSwipingIDEnabled
        {
            get { return isSwipingIDEnabled; }
            set
            {
                if (value != isSwipingIDEnabled)
                {
                    isSwipingIDEnabled = value;
                    OnPropertyChanged(() => IsSwipingIDEnabled);
                }
            }
        }

        private bool showGetTicketBySwipIDButton;

        public bool ShowGetTicketBySwipIDButton
        {
            get { return showGetTicketBySwipIDButton; }
            set
            {
                if (value != showGetTicketBySwipIDButton)
                {
                    showGetTicketBySwipIDButton = value;
                    OnPropertyChanged(() => ShowGetTicketBySwipIDButton);
                }
            }
        }

        private bool showGetTicketByInputIDButton;

        public bool ShowGetTicketByInputIDButton
        {
            get { return showGetTicketByInputIDButton; }
            set
            {
                if (value != showGetTicketByInputIDButton)
                {
                    showGetTicketByInputIDButton = value;
                    OnPropertyChanged(() => ShowGetTicketByInputIDButton);
                }
            }
        }

        private bool showGetTicketByCellAndOrderButton;

        public bool ShowGetTicketByCellAndOrderButton
        {
            get { return showGetTicketByCellAndOrderButton; }
            set
            {
                if (value != showGetTicketByCellAndOrderButton)
                {
                    showGetTicketByCellAndOrderButton = value;
                    OnPropertyChanged(() => ShowGetTicketByCellAndOrderButton);
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
