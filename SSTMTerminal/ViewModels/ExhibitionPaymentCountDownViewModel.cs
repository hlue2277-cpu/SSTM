using BarCodeScanner;
using Genesis;
using Genesis.Logging;
using System;
using System.Windows;
using System.Windows.Input;

namespace SSTMTerminal.ViewModels
{
    public interface IExhibitionPaymentCountDownViewModel : IViewModel
    {
        void SetValue(Visibility titleVisibility, string titleImageSource, 
            string ticketType, int visitorCount, float totalPrice, string payMethod, 
            Visibility backButtonVisibility, int countDown, Guid requestGuid);

        EventHandler<BarCodeScanDataEventArgs> ScanDataReceivedEvent { get; set; }

        ICommand BackToHomeCommand { get; set; }
        bool IsTimerEnable { get; set; }
        bool ReadyToScan { get; set; }
        Guid RequestGuid { get; set; }
        void Reset();
    }

    public class ExhibitionPaymentCountDownViewModel : ViewModelBase, IExhibitionPaymentCountDownViewModel
    {
        private const float DeltaPrice = 0.000000000000000000001F;

        public EventHandler<BarCodeScanDataEventArgs> ScanDataReceivedEvent { get; set; }

        public void Reset()
        {
            IsTimerEnable = false;
            CountDown = 0;
            ReadyToScan = false;
            RequestGuid = Guid.Empty;
        }

        private bool isTimerEnable;
        public bool IsTimerEnable
        {
            get { return isTimerEnable; }
            set
            {
                if (isTimerEnable != value)
                {
                    isTimerEnable = value;
                    OnPropertyChanged(() => IsTimerEnable);
                }
            }
        }
        public Guid RequestGuid { get; set; }

        public bool ReadyToScan { get; set; }
        public ICommand BackToHomeCommand { get; set; }

        private Visibility backButtonVisibility;

        public Visibility BackButtonVisibility
        {
            get { return backButtonVisibility; }
            set
            {
                if (value != backButtonVisibility)
                {
                    backButtonVisibility = value;
                    OnPropertyChanged(() => BackButtonVisibility);
                }
            }
        }

        private Visibility titleVisibility;

        public Visibility TitleVisibility
        {
            get { return titleVisibility; }
            set
            {
                if (value != titleVisibility)
                {
                    titleVisibility = value;
                    OnPropertyChanged(() => TitleVisibility);
                }
            }
        }

        private string titleImageSource;

        public string TitleImageSource
        {
            get { return titleImageSource; }
            set
            {
                if (value != titleImageSource)
                {
                    titleImageSource = value;
                    OnPropertyChanged(() => TitleImageSource);
                }
            }
        }

        private string messageLine1;

        public string MessageLine1
        {
            get { return messageLine1; }
            set
            {
                if (value != messageLine1)
                {
                    messageLine1 = value;
                    OnPropertyChanged(() => MessageLine1);
                }
            }
        }

        private string messageLine2;

        public string MessageLine2
        {
            get { return messageLine2; }
            set
            {
                if (value != messageLine2)
                {
                    messageLine2 = value;
                    OnPropertyChanged(() => MessageLine2);
                }
            }
        }

        private string contentImageSource;

        public string ContentImageSource
        {
            get { return contentImageSource; }
            set
            {
                if (value != contentImageSource)
                {
                    contentImageSource = value;
                    OnPropertyChanged(() => ContentImageSource);
                }
            }
        }

        private string ticketType;
        public string TicketType
        {
            get { return ticketType; }
            set
            {
                if (value != ticketType)
                {
                    ticketType = value;
                    OnPropertyChanged(() => TicketType);
                }
            }
        }

        private string payMethod;
        public string PayMethod
        {
            get { return payMethod; }
            set
            {
                if (value != payMethod)
                {
                    payMethod = value;
                    OnPropertyChanged(() => PayMethod);
                }
            }
        }

        private int visitorCount;
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
            }
        }

        private float totalPrice;
        public float TotalPrice
        {
            get { return totalPrice; }
            set
            {
                if (value != totalPrice)
                {
                    totalPrice = value;
                    OnPropertyChanged(() => TotalPrice);
                }
            }
        }

        private int countDown;
        public int CountDown
        {
            get { return countDown; }
            set
            {
                if (value != countDown)
                {
                    countDown = value;
                    OnPropertyChanged(() => CountDown);
                }
            }
        }

        public void SetValue(Visibility titleVisibility, string titleImageSource,
            string ticketType, int visitorCount, float totalPrice, string payMethod,
            Visibility backButtonVisibility, int countDown, Guid requestGuid)
		{
            TitleVisibility = titleVisibility;
            TitleImageSource = titleImageSource;
            BackButtonVisibility = backButtonVisibility;
            CountDown = countDown;
            TicketType = ticketType;
            VisitorCount = visitorCount;
            TotalPrice = totalPrice;
            PayMethod = payMethod;
            RequestGuid = requestGuid;
            if (TotalPrice < DeltaPrice)
            {
                MessageLine1 = "正在下单";
                MessageLine2 = "请稍后";
            }
            else
            {
                MessageLine1 = "请将微信、支付宝或者银联付款码";
                MessageLine2 = "置于下方扫码口，扫码完成支付";
            }
        }
    }
}