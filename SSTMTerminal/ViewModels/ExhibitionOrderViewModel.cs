using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using SSTMTerminal.Images;
using SSTMTerminal.Models;
using SSTMTerminal.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IExhibitionOrderViewModel : IViewModel
	{
		ICommand PayForTicketsCommand { get; set; }
        ICommand UseAliPayCommand { get; set; }
        ICommand UseWeChatPayCommand { get; set; }
        ICommand BackToHomeCommand { get; set; }
        ICommand ManualAddVisitorCommand { get; set; }
        ICommand SwipAddVisitorCommand { get; set; }
        ICommand SwipAddForeignerVisitorCommand { get; set; }
        ICommand DeleteVisitorCommand { get; set; }

        string DateAndDay { get; set; }
        string ExhibitionTicketType { get; set; }
        bool IsPayForTicketsEnabled { get; set; }
        string PayForTicketsBgImageSource { get; set; }
        string AliPayCheckImageSource { get; set; }
        string WeChatPayCheckImageSource { get; set; }
        int VisitorCount { get; set; }
        string DateTime { get; set; }
        // the ID card reader is ready or not.
        bool IsSwipingIDEnabled { get; set; }
        bool CanAddVisitor { get; set; }
        ExhibitionTicketType TicketType { get; }
        ObservableCollection<VisitorModel> Visitors { get; }
        string PayMethod { get; }
        string PayTicketCommandText {  get; set; }
        float TotalPrice { get; }

        void SetExhibitionTicketType(ExhibitionTicketType ticketType);
        void AddVisitor(VisitorModel model);
        void Reset();
	}

	public class ExhibitionOrderViewModel : ViewModelBase, IExhibitionOrderViewModel
    {
        private const string PayCommandTextFormat = "支付￥{0}";

        public ICommand PayForTicketsCommand { get; set; }
        public ICommand UseAliPayCommand { get; set; }
        public ICommand UseWeChatPayCommand { get; set; }
        public ICommand BackToHomeCommand { get; set; }
        public ICommand ManualAddVisitorCommand { get; set; }
        public ICommand SwipAddVisitorCommand { get; set; }
        public ICommand SwipAddForeignerVisitorCommand { get; set; }
        public ICommand DeleteVisitorCommand { get; set; }

        public ExhibitionOrderViewModel(ILogger logger) :
			base(logger)
		{
            PayMethod = PayMethods.WeChatPay;
            DateAndDay = System.DateTime.Today.ToString("yyyy-MM-dd dddd");
            DeleteVisitorCommand = new DelegateCommand<object>(OnDeleteVisitorCommand);
            UseAliPayCommand = new DelegateCommand(OnUseAliPayCommand);
            UseWeChatPayCommand = new DelegateCommand(OnUseWeChatPayCommand);
            RegionName = RegionNames.HomeRegion;
        }

        private void OnUseAliPayCommand()
        {
            AliPayCheckImageSource = ImagePath.Checked;
            WeChatPayCheckImageSource = ImagePath.Unchecked;
            PayMethod = PayMethods.AliPay;
        }

        private void OnUseWeChatPayCommand()
        {
            AliPayCheckImageSource = ImagePath.Unchecked;
            WeChatPayCheckImageSource = ImagePath.Checked;
            PayMethod = PayMethods.WeChatPay;
        }

        private void OnDeleteVisitorCommand(object obj)
        {
            if (obj != null && obj is VisitorModel visitor)
            {
                Visitors.Remove(visitor);
                VisitorCount--;
            }
        }

        public void AddVisitor(VisitorModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ID)
                || Visitors.Any(v => v.ID?.ToLower() == model.ID?.ToLower()))
            {
                Logger?.Information("列表中已经存在相同ID的游客！");
                return;
            }
            Visitors.Add(model);
            VisitorCount++;
        }

        public void Reset()
        {
            Visitors.Clear();
            DateTime = string.Empty;
            VisitorCount = 0;
            IsPayForTicketsEnabled = false;
            PayTicketCommandText = string.Format(PayCommandTextFormat, 0);
            PayForTicketsBgImageSource = ImagePath.PayTicketDisabledBg;
            WeChatPayCheckImageSource = ImagePath.Checked;
            AliPayCheckImageSource = ImagePath.Unchecked;
            PayMethod = PayMethods.WeChatPay;
        }

        public void SetExhibitionTicketType(ExhibitionTicketType ticketType)
        {
            TicketType = ticketType;
            ExhibitionTicketType = ticketType.TicketType;
            ExhibitionTicketPrice = ticketType.TicketPriceString;
            DateTime = $"{System.DateTime.Today.ToString("yyyy-MM-dd")} {TicketType.TimeRange}";
        }

        public float TotalPrice { get; private set; }

        public string PayMethod { get; private set; }

        public ExhibitionTicketType TicketType { private set; get; }

        private string payTicketCommandText = string.Format(PayCommandTextFormat, 0);
        public string PayTicketCommandText
        {
            get { return payTicketCommandText; }
            set
            {
                if (value != payTicketCommandText)
                {
                    payTicketCommandText = value;
                    OnPropertyChanged(() => PayTicketCommandText);
                }
            }
        }

        private string exhibitionTicketType = string.Empty;
        public string ExhibitionTicketType
        {
            get { return exhibitionTicketType; }
            set
            {
                if (value != exhibitionTicketType)
                {
                    exhibitionTicketType = value;
                    OnPropertyChanged(() => ExhibitionTicketType);
                }
            }
        }

        private string exhibitionTicketPrice = string.Empty;
        public string ExhibitionTicketPrice
        {
            get { return exhibitionTicketPrice; }
            set
            {
                if (value != exhibitionTicketPrice)
                {
                    exhibitionTicketPrice = value;
                    OnPropertyChanged(() => ExhibitionTicketPrice);
                }
            }
        }

        private ObservableCollection<VisitorModel> visitors = new ObservableCollection<VisitorModel>();
        public ObservableCollection<VisitorModel> Visitors
        {
            get { return visitors; }
            set
            {
                if (value != visitors)
                {
                    visitors = value;
                    OnPropertyChanged(() => Visitors);
                }
            }
        }

        private int visitorCount = 0;
        public int VisitorCount
        {
            get { return visitorCount; }
            set
            {
                if (value != visitorCount)
                {
                    visitorCount = value;
                    OnPropertyChanged(() => VisitorCount);
                }
                if (visitorCount == 0)
                {
                    IsPayForTicketsEnabled = false;
                    PayForTicketsBgImageSource = ImagePath.PayTicketDisabledBg;
                    TotalPrice = 0;
                    PayTicketCommandText = string.Format(PayCommandTextFormat, 0);
                }
                else
                {
                    IsPayForTicketsEnabled = true;
                    PayForTicketsBgImageSource = ImagePath.PayTicketBg;
                    TotalPrice = visitorCount * TicketType.TicketPrice;
                    PayTicketCommandText = string.Format(PayCommandTextFormat, TotalPrice);
                }

                if (visitorCount >= 5)
                {
                    CanAddVisitor = false;
                }
                else
                {
                    CanAddVisitor = true;
                }
            }
        }

        private string dateTime;
        public string DateTime
        {
            get { return dateTime; }
            set
            {
                if (value != dateTime)
                {
                    dateTime = value;
                    OnPropertyChanged(() => DateTime);
                }
            }
        }

        private bool isPayForTicketsEnabled;
        public bool IsPayForTicketsEnabled
        {
            get { return isPayForTicketsEnabled; }
            set
            {
                if (value != isPayForTicketsEnabled)
                {
                    isPayForTicketsEnabled = value;
                    OnPropertyChanged(() => IsPayForTicketsEnabled);
                }
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

        private bool canAddVisitor = true;
        public bool CanAddVisitor
        {
            get { return canAddVisitor; }
            set
            {
                if (value != canAddVisitor)
                {
                    canAddVisitor = value;
                    OnPropertyChanged(() => CanAddVisitor);
                }
            }
        }

        private string payForTicketsBgImageSource = ImagePath.PayTicketDisabledBg;
        public string PayForTicketsBgImageSource
        {
            get { return payForTicketsBgImageSource; }
            set
            {
                if (value != payForTicketsBgImageSource)
                {
                    payForTicketsBgImageSource = value;
                    OnPropertyChanged(() => PayForTicketsBgImageSource);
                }
            }
        }

        private string wechatPayCheckImageSource = ImagePath.Checked;
        public string WeChatPayCheckImageSource
        {
            get { return wechatPayCheckImageSource; }
            set
            {
                if (value != wechatPayCheckImageSource)
                {
                    wechatPayCheckImageSource = value;
                    OnPropertyChanged(() => WeChatPayCheckImageSource);
                }
            }
        }

        private string aliPayCheckImageSource = ImagePath.Unchecked;
        public string AliPayCheckImageSource
        {
            get { return aliPayCheckImageSource; }
            set
            {
                if (value != aliPayCheckImageSource)
                {
                    aliPayCheckImageSource = value;
                    OnPropertyChanged(() => AliPayCheckImageSource);
                }
            }
        }

        private string dateAndDay = string.Empty;
        public string DateAndDay
        {
            get { return dateAndDay; }
            set
            {
                if (value != dateAndDay)
                {
                    dateAndDay = value;
                    OnPropertyChanged(() => DateAndDay);
                }
            }
        }
    }
}
