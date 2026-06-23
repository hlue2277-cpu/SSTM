using Autofac;
using DataService;
using Genesis;
using Genesis.Commands;
using Genesis.Logging;
using Genesis.Utilities;
using Printing;
using SqliteUtil;
using SSTMTerminal.Helpers;
using SSTMTerminal.Images;
using SSTMTerminal.Models;
using SSTMTerminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using IDCardReader = CVR_Reader.CVR_Reader;

namespace SSTMTerminal
{
    public class Loader : LoaderBase, ILoader
    {
        private SoundPlayer printedSound = null;//出票提示音

        public static bool IsOneToOne { get; set; }
        public ITicketService TicketService { get; set; }
        public TicketNotifyInfoDao NotifyInfoDao { get; set; }

        private Dispatcher Dispatcher;
        private PrintManager PrintManager { get; set; }

        private Timer renotifyServerTimer;

        private IDCardReaderUsage idCardReaderUsage = IDCardReaderUsage.None;

        private Guid paymentRequestGuid = Guid.Empty;

        private int maxCharNumberInOneLine = 17;

        #region Views

        public IHomeViewModel HomeView { get; set; }
        public IGetTicketHomeViewModel GetTicketHomeView { get; set; }
        public IMsgViewModel MsgView { get; set; }
        public IPrintingViewModel PrintingView { get; set; }
        public IMsgWithoutCountDownViewModel MsgWithoutCountDownView { get; set; }
        public ISelectTicketsViewModel SelectTicketsView { get; set; }
        public IGetTicketByInputIDViewModel GetTicketByInputIDView { get; set; }
        public IGetTicketByCellAndOrderViewModel GetTicketByCellAndOrderView { get; set; }
        public IVisitBookingListViewModel VisitBookingListView { get; set; }
        public IVisitBookingOrderViewModel VisitBookingOrderView { get; set; }
        public IVisitBookingManualAddVisitorViewModel VisitBookingManualAddVisitorView { get; set; }
        public IVisitBookingPrintTicketViewModel VisitBookingPrintTicketView { get; set; }

        public IExhibitionTicketTimeSlotsViewModel ExhibitionTicketTimeSlotsView { get; set; }
        public IExhibitionTicketTypeListViewModel ExhibitionTicketTypeListView { get; set; }
        public IExhibitionOrderViewModel ExhibitionOrderView { get; set; }
        public IExhibitionManualAddVisitorViewModel ExhibitionManualAddVisitorView { get; set; }
        public IExhibitionPaymentCountDownViewModel ExhibitionPaymentCountDownView { get; set; }
        public IExhibitionPrintTicketViewModel ExhibitionPrintTicketView { get; set; }

        #endregion

        #region ctor

        public Loader() : base()
        {
            Dispatcher = Application.Current.Dispatcher;
            int interval = int.Parse(ConfigurationManager.AppSettings.Get("RenotifyServerTimerInterval"));
            renotifyServerTimer = new Timer(interval);
            renotifyServerTimer.Elapsed += OnRenotifyServerTimerElapsed;
            renotifyServerTimer.Enabled = false;

            int.TryParse(ConfigurationManager.AppSettings.Get("ExhibitionTicketTypeDescriptionCharsInOneLine"), out maxCharNumberInOneLine);
        }

        #endregion

        #region Override Methods
        protected override void Setup()
        {
            base.Setup();

            PrintManager = Container.Resolve<PrintManager>();

            TicketService = Container.Resolve<ITicketService>();
            NotifyInfoDao = Container.Resolve<TicketNotifyInfoDao>();

            HomeView = Container.Resolve<IHomeViewModel>();
            GetTicketHomeView = Container.Resolve<IGetTicketHomeViewModel>();

            MsgView = Container.Resolve<IMsgViewModel>();
            PrintingView = Container.Resolve<IPrintingViewModel>();
            MsgWithoutCountDownView = Container.Resolve<IMsgWithoutCountDownViewModel>();
            SelectTicketsView = Container.Resolve<ISelectTicketsViewModel>();
            GetTicketByInputIDView = Container.Resolve<IGetTicketByInputIDViewModel>();
            GetTicketByCellAndOrderView = Container.Resolve<IGetTicketByCellAndOrderViewModel>();
            VisitBookingListView = Container.Resolve<IVisitBookingListViewModel>();
            VisitBookingOrderView = Container.Resolve<IVisitBookingOrderViewModel>();
            VisitBookingManualAddVisitorView = Container.Resolve<IVisitBookingManualAddVisitorViewModel>();
            VisitBookingPrintTicketView = Container.Resolve<IVisitBookingPrintTicketViewModel>();

            //ExhibitionListView = Container.Resolve<IExhibitionListViewModel>();
            ExhibitionTicketTypeListView = Container.Resolve<IExhibitionTicketTypeListViewModel>();
            ExhibitionTicketTimeSlotsView = Container.Resolve<IExhibitionTicketTimeSlotsViewModel>();
            ExhibitionManualAddVisitorView = Container.Resolve<IExhibitionManualAddVisitorViewModel>();
            ExhibitionOrderView = Container.Resolve<IExhibitionOrderViewModel>();
            ExhibitionPaymentCountDownView = Container.Resolve<IExhibitionPaymentCountDownViewModel>();
            ExhibitionPrintTicketView = Container.Resolve<IExhibitionPrintTicketViewModel>();

            HomeView.Version = "V1.3";
            HomeView.VisitBookingCommand = new DelegateCommand<object>(OnVisitBookingCommand);
            HomeView.NavigateToGetTicketHomeCommand = new DelegateCommand<object>(OnNavigateToGetTicketHomeCommand);
            HomeView.BuyExhibitionTicketCommand = new DelegateCommand<object>(OnListExhibitionTimeSlotsCommand);

            GetTicketHomeView.NoticeImagePath = Path.Combine(Environment.CurrentDirectory, "ConfigurableImages", "Notice.png");
            GetTicketHomeView.GetTicketBySwipingIDCommand = new DelegateCommand<object>(OnGetTicketBySwipingIDCommand);
            GetTicketHomeView.GetTicketByInputIDCommand = new DelegateCommand<object>(OnGetTicketByInputIDCommand);
            GetTicketHomeView.GetTicketByCellAndOrderCommand = new DelegateCommand<object>(OnGetTicketByCellAndOrderCommand);
            GetTicketHomeView.BackToHomeCommand = new DelegateCommand<object>(OnBackToHomeCommand);

            MsgView.BackToHomeCommand = new DelegateCommand<object>(OnMsgViewBackToHomeCommand);

            MsgWithoutCountDownView.BackToHomeCommand = new DelegateCommand<object>(OnBackToHomeCommand);

            PrintingView.BackToHomeCommand = new DelegateCommand<object>(OnBackToHomeCommand);
            PrintingView.PrintFinishedCommand = new DelegateCommand<object>(OnPrintFinishedCommand);

            SelectTicketsView.BackToHomeCommand = new DelegateCommand<object>(OnBackToHomeCommand);
            SelectTicketsView.SelectAllCommand = new DelegateCommand<object>(OnSelectAllCommand);
            SelectTicketsView.ConfirmGetTicketCommand = new DelegateCommand<object>(OnConfirmGetTicketCommand);
            SelectTicketsView.SelectCommand = new DelegateCommand<object>(OnSelectCommand);

            GetTicketByInputIDView.BackToHomeCommand = new DelegateCommand<object>(OnBackToHomeCommand);
            GetTicketByInputIDView.ConfirmCommand = new DelegateCommand<object>(OnConfirmIDCommand);

            GetTicketByCellAndOrderView.BackToHomeCommand = new DelegateCommand<object>(OnBackToHomeCommand);
            GetTicketByCellAndOrderView.ConfirmCommand = new DelegateCommand<object>(OnConfirmCellAndOrderCommand);

            VisitBookingListView.SelectTimeSlotCommand = new DelegateCommand<object>(OnSelectVisitBookingTimeSlotCommand);
            VisitBookingListView.BackToHomeCommand = new DelegateCommand<object>(OnBackToHomeCommand);

            VisitBookingOrderView.SwipAddVisitorCommand = new DelegateCommand(OnBookingSwipAddVisitorCommand);
            VisitBookingOrderView.SwipAddForeignerVisitorCommand = new DelegateCommand(OnBookingSwipAddForeignerVisitorCommand);
            VisitBookingOrderView.ManualAddVisitorCommand = new DelegateCommand(OnBookingManualAddVisitorCommand);
            VisitBookingOrderView.BackToHomeCommand = new DelegateCommand<object>(OnVisitBookingBackToPreviousPageCommand);
            VisitBookingOrderView.ConfirmBookingCommand = new DelegateCommand(OnConfirmBookingCommand);

            VisitBookingManualAddVisitorView.BackToOwnerCommand = new DelegateCommand(OnBackToVisitBookingOrderCommand);
            VisitBookingManualAddVisitorView.ConfirmAddVisitorCommand = new DelegateCommand(OnConfirmAddVisitorForVisitBookingCommand);

            VisitBookingPrintTicketView.PrintFinishedCommand = new DelegateCommand<object>(OnPrintFinishedCommand);

            ExhibitionTicketTimeSlotsView.BackToHomeCommand = new DelegateCommand<object>(OnBackToHomeCommand);
            ExhibitionTicketTimeSlotsView.SelectExhibitionTicketTimeSlotCommand = new DelegateCommand<object>(OnSelectExhibitionTimeSlotCommand);

            ExhibitionTicketTypeListView.BackToHomeCommand = new DelegateCommand<object>(OnBackToExhibitionTicketTimeSlotsCommand);
            ExhibitionTicketTypeListView.SelectExhibitionTicketTypeCommand = new DelegateCommand<object>(OnSelectExhibitionTicketTypeCommand);

            ExhibitionManualAddVisitorView.BackToOwnerCommand = new DelegateCommand(OnBackToExhibitionOrderCommand);
            ExhibitionManualAddVisitorView.ConfirmAddVisitorCommand = new DelegateCommand(OnConfirmAddVisitorForExhibitionCommand);

            ExhibitionOrderView.SwipAddVisitorCommand = new DelegateCommand(OnExhibitionSwipAddVisitorCommand);
            ExhibitionOrderView.SwipAddForeignerVisitorCommand = new DelegateCommand(OnExhibitionSwipAddForeignerVisitorCommand);
            ExhibitionOrderView.ManualAddVisitorCommand = new DelegateCommand(OnExhibitionManualAddVisitorCommand);
            ExhibitionOrderView.BackToHomeCommand = new DelegateCommand<object>(OnBackToExhibitionTicketTypeListCommand);
            ExhibitionOrderView.PayForTicketsCommand = new DelegateCommand(OnPayForTicketsCommand);

            ExhibitionPaymentCountDownView.BackToHomeCommand = new DelegateCommand<object>(OnBackToExhibitionOrderCommand);
            ExhibitionPrintTicketView.PrintFinishedCommand = new DelegateCommand<object>(OnPrintFinishedCommand);

            ExhibitionPaymentCountDownView.ScanDataReceivedEvent += OnPaymentScanDataReceived;
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            EventAggregator.GetEvent<StartupEvent>().Subscribe(OnStartupEvent);
            EventAggregator.GetEvent<ClosingEvent>().Subscribe(OnClosingEvent);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            EventAggregator.GetEvent<StartupEvent>().Unsubscribe(OnStartupEvent);
            EventAggregator.GetEvent<ClosingEvent>().Unsubscribe(OnClosingEvent);
        }

        #endregion

        #region Command Handlers

        private void OnGetTicketBySwipingIDCommand(object payload)
        {
            StartReadingIDCard(IDCardReaderUsage.QueryTicket, Visibility.Visible, ImagePath.SwipeIDPageTitle, null);
        }

        private void OnGetTicketByInputIDCommand(object payload)
        {
            AttachOnlyView(RegionNames.CenterRegion, GetTicketByInputIDView);
        }

        private void OnGetTicketByCellAndOrderCommand(object payload)
        {
            AttachOnlyView(RegionNames.CenterRegion, GetTicketByCellAndOrderView);
        }

        private void OnListExhibitionTimeSlotsCommand(object obj)
        {
            AttachOnlyView(RegionNames.HomeRegion, ExhibitionTicketTimeSlotsView);

            var timeSlots = GetExhibitionTimeSlots();
            if (timeSlots != null)
            {
                ExhibitionTicketTimeSlotsView.SetTimeSlots(timeSlots);
            }
            else
            {
                // TEST code.
                // Ticket Service to query the time slots
                var mockedTimeSlots = new TimeSlotsModel();
                var item1 = new SlotItem();
                item1.DisplayName = "09:00-10:00 测试 (余票10张)";
                item1.TimeRange = "09:00-10:00";
                item1.IsAvailable = true;
                item1.ScheduleId = 19;
                item1.ReservePeriodId = 1271;
                var item2 = new SlotItem();
                item2.DisplayName = "14:00-15:00 测试 (余票10张)";
                item2.TimeRange = "14:00-15:00";
                item2.IsAvailable = false;
                item2.ScheduleId = 19;
                item2.ReservePeriodId = 1271;
                item2.TimeSlotImagePath = ImagePath.ItemRectangleDisabled;
                mockedTimeSlots.Slots.Add(item1);
                mockedTimeSlots.Slots.Add(item2);

                ExhibitionTicketTimeSlotsView.SetTimeSlots(mockedTimeSlots);
            }
        }

        private void OnSelectExhibitionTimeSlotCommand(object param)
        {
            if (param is SlotItem slotItem && slotItem.IsAvailable)
            {
                var exhibitionTicketTypes = GetTicketTypes(slotItem.ReservePeriodId, slotItem.TimeRange);
                if (exhibitionTicketTypes != null)
                {
                    ExhibitionTicketTypeListView.SetExhibitionTimeSlotAndTicketTypes(slotItem, exhibitionTicketTypes);
                }
                else
                {
                    // TEST code.
                    // Ticket Service to query the special exhibition
                    var mockedExhibitionTicketTypes = new ExhibitionTicketTypesModel();
                    mockedExhibitionTicketTypes.ExhibitionName = slotItem.DisplayName;
                    var item1 = new ExhibitionTicketType();
                    item1.TicketType = "全价票";
                    item1.TicketPrice = 100;
                    item1.TicketPriceString = "100元";
                    item1.TicketTypeDescription = "适用于18-60岁成年人购买";
                    item1.IsAvailable = true;
                    item1.ScheduleId = 19;
                    item1.ReservePeriodId = 1271;
                    item1.TimeRange = slotItem.TimeRange;
                    var item2 = new ExhibitionTicketType();
                    item2.TicketType = "优惠票";
                    item2.TicketPrice = 50;
                    item2.TicketPriceString = "50元";
                    item2.TicketTypeDescription = GetTicketTypeDescriptionWithLineBreak("适用于60岁(含)以上老年人或者军人购买，入场需提供相关证件!!适用于60岁(含)以上老年人或者军人购买，入场需提供相关证件");
                    item2.IsAvailable = true;
                    item2.ScheduleId = 19;
                    item2.ReservePeriodId = 1271;
                    item2.TimeRange = slotItem.TimeRange;
                    var item3 = new ExhibitionTicketType();
                    item3.TicketType = "免费票";
                    item3.TicketPrice = 0;
                    item3.TicketPriceString = "0元";
                    item3.TicketTypeDescription = "适用于身高小于1.3米的小朋友购买";
                    item3.IsAvailable = true;
                    item3.ScheduleId = 19;
                    item3.ReservePeriodId = 1271;
                    item3.TimeRange = slotItem.TimeRange;
                    mockedExhibitionTicketTypes.ExhibitionTicketTypes.Add(item1);
                    mockedExhibitionTicketTypes.ExhibitionTicketTypes.Add(item2);
                    mockedExhibitionTicketTypes.ExhibitionTicketTypes.Add(item3);
                    ExhibitionTicketTypeListView.SetExhibitionTimeSlotAndTicketTypes(slotItem, mockedExhibitionTicketTypes);
                }

                AttachOnlyView(RegionNames.HomeRegion, ExhibitionTicketTypeListView);
            }
        }

        private void OnBackToExhibitionTicketTypeListCommand(object obj)
        {
            MsgView.Reset();
            ExhibitionOrderView.Reset();
            ExhibitionTicketTypeListView.Reset();
            OnSelectExhibitionTimeSlotCommand(ExhibitionTicketTypeListView.TicketTimeSlot);
        }

        private void OnSelectExhibitionTicketTypeCommand(object payload)
        {
            if (payload != null && payload is ExhibitionTicketType ticketType)
            {
                AttachOnlyView(RegionNames.HomeRegion, ExhibitionOrderView);
                ExhibitionOrderView.SetExhibitionTicketType(ticketType);
            }
        }

        private void OnNavigateToGetTicketHomeCommand(object obj)
        {
            AttachOnlyView(RegionNames.HomeRegion, GetTicketHomeView);
        }

        private void OnVisitBookingCommand(object payload)
        {
            AttachOnlyView(RegionNames.HomeRegion, VisitBookingListView);

            var timeSlots = GetVisitBookingTimeSlots();
            if (timeSlots != null)
            {
                VisitBookingListView.SetTimeSlots(timeSlots);
            }
            else
            {
                // TEST code.
                // Ticket Service to query the time slots
                var mockedTimeSlots = new TimeSlotsModel();
                var item1 = new SlotItem();
                item1.DisplayName = "09:00-10:00 测试 (余票10张)";
                item1.TimeRange = "09:00-10:00";
                item1.IsAvailable = true;
                item1.ScheduleId = 19;
                item1.ReservePeriodId = 1271;
                var item2 = new SlotItem();
                item2.DisplayName = "14:00-15:00 测试 (余票10张)";
                item2.TimeRange = "14:00-15:00";
                item2.IsAvailable = false;
                item2.ScheduleId = 19;
                item2.ReservePeriodId = 1271;
                item2.TimeSlotImagePath = ImagePath.ItemRectangleDisabled;
                mockedTimeSlots.Slots.Add(item1);
                mockedTimeSlots.Slots.Add(item2);

                VisitBookingListView.SetTimeSlots(mockedTimeSlots);
            }
        }

        private void OnSelectVisitBookingTimeSlotCommand(object payload)
        {
            if (payload != null && payload is SlotItem slotItem && slotItem.IsAvailable)
            {
                AttachOnlyView(RegionNames.HomeRegion, VisitBookingOrderView);
                VisitBookingOrderView.SetTimeSlot(slotItem);
            }
        }

        private void OnConfirmBookingCommand()
        {
            try
            {
                var hasOrderVisitorIDs = CheckVisitorsHasSameTicketTypeOrder(VisitBookingOrderView.Visitors.ToList(), true);
                if (hasOrderVisitorIDs.Count > 0)
                {
                    var shortIds = new StringBuilder();
                    foreach (var id in hasOrderVisitorIDs)
                    {
                        if (id.Length > 6)
                        {
                            var last6InID = id.Substring(id.Length - 6);
                            shortIds.AppendLine(last6InID);
                        }
                        else
                        {
                            shortIds.AppendLine(id);
                        }
                    }

                    MsgView.SetValue(Visibility.Hidden,
                            string.Empty,
                            ImagePath.Error,
                            $"身份证后6位为下列号码的游客有未完成订单：{shortIds}",
                            Visibility.Visible, 30,
                            VisitBookingOrderView);
                    AttachOnlyView(RegionNames.CenterRegion, MsgView);
                    MsgView.IsTimerEnable = true;
                }
                else
                {
                    // Send request to server;
                    var requestModel = CreateRequestVisitBookingModel(VisitBookingOrderView.SlotItem.ReservePeriodId,
                        "test1", VisitBookingOrderView.SlotItem.ScheduleId,
                        VisitBookingOrderView.VisitorCount, VisitBookingOrderView.Visitors);

                    var response = TicketService.PlaceVistBookingOrder(requestModel);
                    if (response != null && response.success)
                    {
                        var queryTicketsResponse = TicketService.QueryTickets(VisitBookingOrderView.Visitors[0]?.ID);
                        if (queryTicketsResponse != null && queryTicketsResponse.success && queryTicketsResponse.data.Count > 0)
                        {
                            var ticketsToPrint = new List<TicketItemViewModel>();
                            foreach (var ticketData in queryTicketsResponse.data)
                            {
                                if (string.Compare(ticketData.voucherNo, response.data.tradeNo, true) == 0)
                                {
                                    var ticket = new TicketItemViewModel(ticketData);
                                    ticketsToPrint.Add(ticket);
                                }
                                else
                                {
                                    Logger.Information($"Ticket {ticketData.uuid} is not printed as its order number {ticketData.voucherNo} is different from {response.data.tradeNo}");
                                }
                            }
                            AttachOnlyView(RegionNames.HomeRegion, VisitBookingPrintTicketView);
                            VisitBookingPrintTicketView.WaitingForPrintMessage = $"正在打印{ticketsToPrint.Count}张门票。\n请稍等...";
                            VisitBookingPrintTicketView.PrintTickets(PrintManager, ticketsToPrint, response.data.tradeNo);
                        }
                        else
                        {
                            MsgView.SetValue(Visibility.Hidden,
                                string.Empty,
                                ImagePath.Error,
                                $"参观预约已经成功，但是获取票务信息失败。\n订单号码：{response.data.tradeNo}。\n请联系管理员！",
                                Visibility.Visible, 60);
                            AttachOnlyView(RegionNames.CenterRegion, MsgView);
                            MsgView.IsTimerEnable = true;
                        }
                    }
                    else
                    {
                        MsgView.SetValue(Visibility.Hidden,
                          string.Empty,
                          ImagePath.Error,
                          "参观预约失败，请重试或者联系管理员！",
                          Visibility.Visible, 30);
                        AttachOnlyView(RegionNames.CenterRegion, MsgView);
                        MsgView.IsTimerEnable = true;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, ex.StackTrace);
                MsgView.SetValue(Visibility.Hidden,
                          string.Empty,
                          ImagePath.Error,
                          "参观预约失败，请重试或者联系管理员！",
                          Visibility.Visible, 30);
                AttachOnlyView(RegionNames.CenterRegion, MsgView);
                MsgView.IsTimerEnable = true;
            }
        }

        private void OnBookingManualAddVisitorCommand()
        {
            // Show dialog to input the ID.
            AttachOnlyView(RegionNames.WorkingCenterRegion, VisitBookingManualAddVisitorView);
        }

        private void OnBackToVisitBookingOrderCommand()
        {
            VisitBookingManualAddVisitorView.Reset();
            AttachOnlyView(RegionNames.HomeRegion, VisitBookingOrderView);
        }

        private void OnConfirmAddVisitorForVisitBookingCommand()
        {
            var visitor = new VisitorModel();
            visitor.CertType = VisitBookingManualAddVisitorView.CertType;
            visitor.ID = VisitBookingManualAddVisitorView.ID;
            visitor.Name = VisitBookingManualAddVisitorView.Name;
            AttachOnlyView(RegionNames.HomeRegion, VisitBookingOrderView);
            VisitBookingOrderView.AddVisitor(visitor);
            VisitBookingManualAddVisitorView.Reset();
        }

        private void OnBackToExhibitionOrderCommand()
        {
            ExhibitionManualAddVisitorView.Reset();
            AttachOnlyView(RegionNames.HomeRegion, ExhibitionOrderView);
        }

        private void OnConfirmAddVisitorForExhibitionCommand()
        {
            var visitor = new VisitorModel();
            visitor.CertType = ExhibitionManualAddVisitorView.CertType;
            visitor.ID = ExhibitionManualAddVisitorView.ID;
            visitor.Name = ExhibitionManualAddVisitorView.Name;
            AttachOnlyView(RegionNames.HomeRegion, ExhibitionOrderView);
            ExhibitionOrderView.AddVisitor(visitor);
            ExhibitionManualAddVisitorView.Reset();
        }

        private void OnBookingSwipAddVisitorCommand()
        {
            StartReadingIDCard(IDCardReaderUsage.VisitBookingIDCard, Visibility.Collapsed, string.Empty, VisitBookingOrderView);
        }

        private void OnBookingSwipAddForeignerVisitorCommand()
        {
            StartReadingIDCard(IDCardReaderUsage.VisitBookingForeignIDCard, Visibility.Collapsed, string.Empty, VisitBookingOrderView);
        }

        private void StartReadingIDCard(IDCardReaderUsage usage, Visibility titleVisibility, string titleImageSource, IViewModel owner)
        {
            idCardReaderUsage = usage;
            MsgView.SetValue(titleVisibility,
                titleImageSource,
                ImagePath.WaitingForIDCard,
                "请将证件放在右侧刷卡区",
                Visibility.Visible, 30,
                owner);
            AttachOnlyView(RegionNames.CenterRegion, MsgView);
            MsgView.IsTimerEnable = true;

            var isConnected = IDCardReader.IsConnected;
            if (isConnected)
            {
                IDCardReader.BeginRead(out var r, true);
            }
            else
            {
                if (CheckIDCardReader(out var result))
                {
                    IDCardReader.BeginRead(out var r, true);
                }
            }
        }

        private const float DeltaPrice = 0.000000000000000000001F;

        private void OnPayForTicketsCommand()
        {
            var hasOrderVisitorIDs = CheckVisitorsHasSameTicketTypeOrder(ExhibitionOrderView.Visitors.ToList(), false);
            if (hasOrderVisitorIDs.Count > 0)
            {
                var shortIds = new StringBuilder();
                foreach (var id in hasOrderVisitorIDs)
                {
                    if (id.Length > 6)
                    {
                        var last6InID = id.Substring(id.Length - 6);
                        shortIds.AppendLine(last6InID);
                    }
                    else
                    {
                        shortIds.AppendLine(id);
                    }
                }

                MsgView.SetValue(Visibility.Hidden,
                        string.Empty,
                        ImagePath.Error,
                        $"身份证后6位为下列号码的游客有未完成订单：{shortIds}",
                        Visibility.Visible, 30,
                        VisitBookingOrderView);
                AttachOnlyView(RegionNames.CenterRegion, MsgView);
                MsgView.IsTimerEnable = true;
            }
            else
            {
                // Free ticket is the same as booking
                if (ExhibitionOrderView.TotalPrice < DeltaPrice)
                {
                    BuyExhibitionFreeTickets();
                }
                else
                {
                    BuyExhibitionNonFreeTickets();
                }
            }
        }

        private void OnBackToExhibitionOrderCommand(object obj)
        {
            ResetIDCardReader();
            MsgView.Reset();
            ExhibitionPaymentCountDownView.Reset();
            AttachOnlyView(RegionNames.HomeRegion, ExhibitionOrderView);
        }

        private void OnBackToExhibitionTicketTimeSlotsCommand(object obj)
        {
            ResetIDCardReader();
            MsgView.Reset();
            ExhibitionOrderView.Reset();
            ExhibitionTicketTimeSlotsView.Reset();
            OnListExhibitionTimeSlotsCommand(null);
        }

        private void OnExhibitionManualAddVisitorCommand()
        {
            // Show dialog to input the ID.
            AttachOnlyView(RegionNames.WorkingCenterRegion, ExhibitionManualAddVisitorView);
        }

        private void OnExhibitionSwipAddForeignerVisitorCommand()
        {
            StartReadingIDCard(IDCardReaderUsage.ExhibitionForeignIDCard, Visibility.Collapsed, string.Empty, ExhibitionOrderView);
        }

        private void OnExhibitionSwipAddVisitorCommand()
        {
            StartReadingIDCard(IDCardReaderUsage.ExhibitionIDCard, Visibility.Collapsed, string.Empty, ExhibitionOrderView);
        }

        private void OnConfirmCellAndOrderCommand(object obj)
        {
            bool isCellNumberValid = NumberHelper.ValidateCellNumber(GetTicketByCellAndOrderView.CellNumber);
            if (!isCellNumberValid)
            {
                MsgView.SetValue(Visibility.Hidden,
                                  string.Empty,
                                  ImagePath.Error,
                                  "手机号输入有误请重试",
                                  Visibility.Visible, 10);
                AttachOnlyView(RegionNames.CenterRegion, MsgView);
                MsgView.IsTimerEnable = true;

                return;
            }

            var queryTicketResponse = TicketService.QueryTickets(GetTicketByCellAndOrderView.CellNumber, GetTicketByCellAndOrderView.OrderNumber);
            HandleQueryTicketResponse(queryTicketResponse);
        }

        private void OnConfirmIDCommand(object obj)
        {
            bool isIDValid = true;//TODO Validate ID

            if (!isIDValid)
            {
                MsgView.SetValue(Visibility.Hidden,
                  string.Empty,
                  ImagePath.Error,
                  "身份证件信息有误请重试",
                  Visibility.Visible, 10);
                AttachOnlyView(RegionNames.CenterRegion, MsgView);
                MsgView.IsTimerEnable = true;
            }
            else
            {
                var queryTicketResponse = TicketService.QueryTickets(GetTicketByInputIDView.ID);
                HandleQueryTicketResponse(queryTicketResponse);
            }
        }

        private void OnConfirmGetTicketCommand(object obj)
        {
            SelectTicketsView.IsTimerEnable = false;
            var ticketsToPrint = SelectTicketsView.GetSelectedTickets();
            AttachOnlyView(RegionNames.CenterRegion, PrintingView);
            PrintingView.PrintTickets(PrintManager, ticketsToPrint);
        }

        private void OnSelectCommand(object obj)
        {
            if (obj is TicketItemViewModel ticket)
            {
                if (ticket.IsSelected)
                {
                    ticket.UpdateSelectStatus(false, ImagePath.Unchecked, ImagePath.TicketItemRectangleDisabled);
                }
                else
                {
                    ticket.UpdateSelectStatus(true, ImagePath.Checked, ImagePath.TicketItemRectangle);
                }
                SelectTicketsView.UpdateSelectAllStatus();
            }
        }

        private void OnSelectAllCommand(object obj)
        {
            if (SelectTicketsView.Tickets != null && SelectTicketsView.Tickets.Count > 0)
            {
                if (SelectTicketsView.IsAllSelected)
                {
                    SelectTicketsView.UpdateSelectAllStatus(false, ImagePath.Unchecked,
                        ImagePath.TicketItemRectangleDisabled,
                        ImagePath.ConfirmGetTicketDisabled);
                }
                else
                {
                    SelectTicketsView.UpdateSelectAllStatus(true, ImagePath.Checked,
                        ImagePath.TicketItemRectangle,
                        ImagePath.ConfirmGetTicket);
                }
            }
            else
            {
                SelectTicketsView.Reset(false);
            }
        }

        private void OnMsgViewBackToHomeCommand(object obj)
        {
            if (MsgView.Owner == null)
            {
                BackToHome();
            }
            else
            {
                AttachOnlyView(string.IsNullOrEmpty(MsgView.Owner.RegionName) ? RegionNames.CenterRegion : MsgView.Owner.RegionName, MsgView.Owner);
            }
        }

        private void OnVisitBookingBackToPreviousPageCommand(object obj)
        {
            ResetIDCardReader();
            MsgView.Reset();
            VisitBookingListView.Reset();
            VisitBookingOrderView.Reset();
            OnVisitBookingCommand(obj);
        }

        private void OnBackToHomeCommand(object obj)
        {
            BackToHome();
        }

        private void BackToHome()
        {
            ResetIDCardReader();
            MsgView.Reset();
            ResetGetTicketViews();
            VisitBookingListView.Reset();
            VisitBookingOrderView.Reset();
            ExhibitionTicketTimeSlotsView.Reset();
            ExhibitionTicketTypeListView.Reset();
            ExhibitionOrderView.Reset();
            ExhibitionPaymentCountDownView.Reset();
            StopSound();
            AttachOnlyView(RegionNames.HomeRegion, HomeView);
        }

        private void ResetIDCardReader()
        {
            idCardReaderUsage = IDCardReaderUsage.None;
            if (IDCardReader.IsConnected)
            {
                GetTicketHomeView.IsSwipingIDEnabled = true;
                VisitBookingOrderView.IsSwipingIDEnabled = true;
                IDCardReader.StopRead();
            }
            else
            {
                bool canOpen = CheckIDCardReader(out var result);
                GetTicketHomeView.IsSwipingIDEnabled = canOpen;
                VisitBookingOrderView.IsSwipingIDEnabled = canOpen;
            }
        }

        private void ResetGetTicketViews()
        {
            SelectTicketsView.IsTimerEnable = false;
            SelectTicketsView.Reset(true);
            GetTicketByCellAndOrderView.Reset();
            GetTicketByInputIDView.Reset();
        }

        private void OnPrintFinishedCommand(object obj)
        {
            if (obj is List<PrintResultModel>)
            {
                Dispatcher.Invoke(() =>
                {
                    ShowMessageWithCountDownView(
                                Visibility.Visible,
                                ImagePath.PrintTicketFinishedPageTitle,
                                ImagePath.TicketsPrintedSuccessfully,
                                "出票成功，请在出票口取票。",
                                Visibility.Visible, 30);
                    if (printedSound != null)
                    {
                        printedSound.Play();
                    }
                });
            }
            else if (obj is PrintException printEx)
            {
                Dispatcher.Invoke(() =>
                {
                    var failedTicketNum = printEx.TotalTicketNum - printEx.PrintedTicketNum;
                    string message = string.Empty;
                    if (string.IsNullOrEmpty(printEx.TradeNO))
                    {
                        message = $"{failedTicketNum}张门票打印失败。请联系人工处理。";
                    }
                    else
                    {
                        message = $"{failedTicketNum}张门票打印失败。\n订单编号：{printEx.TradeNO}。\n请将此信息拍照留存。\n凭订单号联系人工处理。";
                    }
                    ShowMessageWithoutCountDownView(
                                Visibility.Collapsed,
                                ImagePath.PrintTicketFinishedPageTitle,
                                ImagePath.Error,
                                message,
                                Visibility.Visible);
                });
            }
            else if (obj is SystemException || obj is Exception)
            {
                Dispatcher.Invoke(() =>
                {
                    ShowMessageWithoutCountDownView(
                                Visibility.Collapsed,
                                ImagePath.PrintTicketFinishedPageTitle,
                                ImagePath.Error,
                                $"门票打印失败。\n请联系管理员。",
                                Visibility.Visible);
                });
            }
            else if (obj == null)
            {
                ShowMessageWithoutCountDownView(
                                Visibility.Collapsed,
                                ImagePath.PrintTicketFinishedPageTitle,
                                ImagePath.Error,
                                $"在准备打印数据的过程中发生错误。\n请重试。",
                                Visibility.Visible);
            }

            ExhibitionPrintTicketView.OnPrintFinished(PrintManager);
            VisitBookingPrintTicketView.OnPrintFinished(PrintManager);
            PrintingView.OnPrintFinished(PrintManager);
        }

        #endregion Bussiness Command

        #region Private Methods

        private void OnStartupEvent(object payload)
        {
            IsOneToOne = ConfigurationManager.AppSettings.Get(Constants.PRINT_TYPE) == "1";

            Login();

            InitHome();

            Logger.Information("机器初始化成功");
        }

        private void OnClosingEvent(object obj)
        {
            ExhibitionPaymentCountDownView.ScanDataReceivedEvent -= OnPaymentScanDataReceived;
            renotifyServerTimer.Elapsed -= OnRenotifyServerTimerElapsed;
            renotifyServerTimer.Enabled = false;
            renotifyServerTimer?.Dispose();
            PrintManager?.Dispose();
        }

        private void Login()
        {
            TicketService.Login("zizhuji01", "zizhuji@01");
        }

        private void InitHome()
        {
            AttachOnlyView(RegionNames.HomeRegion, HomeView);

            renotifyServerTimer.Enabled = true;

            InitSound();

            idCardReaderUsage = IDCardReaderUsage.None;
            if (!CheckIDCardReader(out var msg))
            {
                IDCardReader.OnCardReceived -= OnIDCardReceived;
                GetTicketHomeView.IsSwipingIDEnabled = false;
                VisitBookingOrderView.IsSwipingIDEnabled = false;
            }
            else
            {
                GetTicketHomeView.IsSwipingIDEnabled = true;
                VisitBookingOrderView.IsSwipingIDEnabled = true;
            }

            bool needRaiseHardwareError = bool.Parse(ConfigurationManager.AppSettings.Get("HardwareDetection"));

            var checkPrinterResult = PrintManager.CheckPrinter();
            if (needRaiseHardwareError && !checkPrinterResult.IsOK)
            {
                ShowMessageWithoutCountDownView(Visibility.Collapsed,
                    ImagePath.SwipeIDPageTitle,
                    ImagePath.Error,
                    string.IsNullOrEmpty(checkPrinterResult.Message) ? "打印机异常，请联系管理员。" : $"打印机异常:{checkPrinterResult.Message}\n请联系管理员。",
                    Visibility.Collapsed);
            }
        }

        private void InitSound()
        {
            var printedPath = AppDomain.CurrentDomain.BaseDirectory + "Sounds/printed.wav";
            if (File.Exists(printedPath))
            {
                printedSound = new SoundPlayer(printedPath);
            }
        }

        private void StopSound()
        {
            try
            {
                if (printedSound != null)
                {
                    printedSound.Stop();
                }
            }
            catch
            {
            }
        }

        private bool CheckIDCardReader(out string message)
        {
            message = string.Empty;
            bool canOpen = IDCardReader.Open(out message);
            IDCardReader.OnCardReceived -= OnIDCardReceived;
            IDCardReader.OnCardReceived += OnIDCardReceived;

            return canOpen;
        }

        private void OnIDCardReceived(CVR_Reader.CardInfo card)
        {
            Logger.Information($"Read ID card. Current usage is {idCardReaderUsage}");
            IDCardReader.StopRead();
            if (idCardReaderUsage == IDCardReaderUsage.QueryTicket)
            {
                OnReadIDCardForQueryTicket(card);
            }
            else if (idCardReaderUsage == IDCardReaderUsage.VisitBookingIDCard)
            {
                OnReadIDCardForVisitBooking(card);
            }
            else if (idCardReaderUsage == IDCardReaderUsage.VisitBookingForeignIDCard)
            {
                OnReadForeignIDCardForVisitBooking(card);
            }
            else if (idCardReaderUsage == IDCardReaderUsage.ExhibitionIDCard)
            {
                OnReadIDCardForExhibition(card);
            }
            else if (idCardReaderUsage == IDCardReaderUsage.ExhibitionForeignIDCard)
            {
                OnReadForeignIDCardForExhibition(card);
            }
        }

        private void OnReadForeignIDCardForVisitBooking(CVR_Reader.CardInfo card)
        {
            Dispatcher.Invoke(() =>
            {
                FinishReadingIDCardForVisitBooking(card, CertTypes.ForeignIdcard);
            });
        }

        private void OnReadIDCardForVisitBooking(CVR_Reader.CardInfo card)
        {
            Dispatcher.Invoke(() =>
            {
                FinishReadingIDCardForVisitBooking(card, CertTypes.IDCard);
            });
        }

        private void FinishReadingIDCardForVisitBooking(CVR_Reader.CardInfo card, string type)
        {
            Logger.Information($"Processing {idCardReaderUsage} ID card reading.");
            var visitor = new VisitorModel();
            visitor.ID = card.GetCompatibleID();
            visitor.CertType = type;
            AttachOnlyView(RegionNames.HomeRegion, VisitBookingOrderView);
            VisitBookingOrderView.AddVisitor(visitor);
            MsgView.Reset();
        }

        private void OnReadForeignIDCardForExhibition(CVR_Reader.CardInfo card)
        {
            Dispatcher.Invoke(() =>
            {
                FinishReadingIDCardForExhibition(card, CertTypes.ForeignIdcard);
            });
        }

        private void OnReadIDCardForExhibition(CVR_Reader.CardInfo card)
        {
            Dispatcher.Invoke(() =>
            {
                FinishReadingIDCardForExhibition(card, CertTypes.IDCard);
            });
        }

        private void FinishReadingIDCardForExhibition(CVR_Reader.CardInfo card, string type)
        {
            Logger.Information($"Processing {idCardReaderUsage} ID card reading.");
            var visitor = new VisitorModel();
            visitor.ID = card.GetCompatibleID();
            visitor.CertType = type;
            AttachOnlyView(RegionNames.HomeRegion, ExhibitionOrderView);
            ExhibitionOrderView.AddVisitor(visitor);
            MsgView.Reset();
        }

        private void OnReadIDCardForQueryTicket(CVR_Reader.CardInfo card)
        {
            try
            {
                var cardId = card.GetCompatibleID();
                var queryTicketResponse = TicketService.QueryTickets(cardId);
                HandleQueryTicketResponse(queryTicketResponse);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => ShowMessageWithCountDownView(
                    Visibility.Collapsed,
                    ImagePath.SwipeIDPageTitle,
                    ImagePath.Error,
                    "从服务器查询门票发生错误，请重试！",
                    Visibility.Visible,
                    10));
                Logger.Log(LogLevel.Error, ex, "查询门票发生异常！", null);
            }
        }

        private void HandleQueryTicketResponse(ResponseModel<List<TicketResponseData>> queryTicketResponse)
        {
            if (queryTicketResponse != null && queryTicketResponse.success)
            {
                if (queryTicketResponse.data.Count > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        foreach (var ticketData in queryTicketResponse.data)
                        {
                            var ticket = new TicketItemViewModel(ticketData);
                            SelectTicketsView.Tickets.Add(ticket);
                        }
                        SelectTicketsView.CountDown = 30;
                        AttachOnlyView(RegionNames.CenterRegion, SelectTicketsView);
                        SelectTicketsView.IsTimerEnable = true;
                        OnSelectAllCommand(null);
                    });
                }
                else
                {
                    Dispatcher.Invoke(() => ShowMessageWithCountDownView(
                        Visibility.Collapsed,
                        ImagePath.SwipeIDPageTitle,
                        ImagePath.Error,
                        "未能从服务器查询到门票，请重试！",
                        Visibility.Visible,
                        10));
                }
            }
            else
            {
                Dispatcher.Invoke(() => ShowMessageWithCountDownView(
                    Visibility.Collapsed,
                    ImagePath.SwipeIDPageTitle,
                    ImagePath.Error,
                    queryTicketResponse == null ? "从服务器查询门票发生错误，请重试！" : queryTicketResponse.msg,
                    Visibility.Visible,
                    10));

                Logger.Information($"从服务器查询门票发生错误。服务器返回：{queryTicketResponse?.msg}");
            }
        }

        private void ShowMessageWithCountDownView(Visibility titleVisibility,
            string titleImageSource,
            string contentImageSource,
            string msg,
            Visibility backButtonVisibility,
            int countDown)
        {
            MsgView.SetValue(titleVisibility,
                titleImageSource,
                contentImageSource,
                msg,
                backButtonVisibility,
                countDown);
            AttachOnlyView(RegionNames.CenterRegion, MsgView);
            MsgView.IsTimerEnable = true;
        }

        public void ShowMessageWithoutCountDownView(Visibility titleVisibility,
            string titleImageSource,
            string contentImageSource,
            string msg,
            Visibility backButtonVisibility)
        {
            MsgWithoutCountDownView.SetValue(titleVisibility,
                titleImageSource,
                contentImageSource,
                msg,
                backButtonVisibility);
            AttachOnlyView(RegionNames.CenterRegion, MsgWithoutCountDownView);
        }

        private void ReNotifyServer()
        {
            try
            {
                var allNotNotifiedTickets = NotifyInfoDao.SelectAll();
                if (allNotNotifiedTickets != null && allNotNotifiedTickets.Count > 0)
                {
                    var renotifyOkList = new List<TicketNotifyInfoModel>();
                    foreach (var ticket in allNotNotifiedTickets)
                    {
                        var response = TicketService.NotifyTicketPrinted(ticket.TicketUUID);
                        if (response != null && response.success)
                        {
                            renotifyOkList.Add(ticket);
                            Logger.Information($"门票（{ticket.TicketUUID}）再次更新门票在服务器中的状态成功。");
                        }
                        else
                        {
                            Logger.Information($"门票（{ticket.TicketUUID}）再次更新门票在服务器中的状态失败。");
                        }
                    }

                    if (renotifyOkList.Count > 0)
                    {
                        bool isDeleted = NotifyInfoDao.Delete(renotifyOkList);
                        StringBuilder sb = new StringBuilder();
                        renotifyOkList.ForEach(t => sb.Append(t.ToString()));
                        var ticketString = sb.ToString();
                        if (isDeleted)
                        {
                            Logger.Information($"门票{ticketString},再次更新门票在服务器中的状态成功，并从本地数据库中删除。");
                        }
                        else
                        {
                            Logger.Information($"门票{ticketString},再次更新门票在服务器中的状态成功，但是从本地数据库中删除失败。");
                        }
                    }
                }
                else
                {
                    Logger.Information($"再次更新之前更新失败的门票在服务器中的状态，但是本地数据库没有之前更新失败的门票。");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "把数据库中所有未通知服务器的门票重新通知服务器失败");
            }
        }

        private void OnRenotifyServerTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ReNotifyServer();
        }

        private List<string> CheckVisitorsHasSameTicketTypeOrder(IList<VisitorModel> visitors, bool isForBooking)
        {
            var configuredScheduleIds = GetConfiguredScheduleIds("BookingScheduleId");
            var bookingScheduleId = configuredScheduleIds[0];

            var hasOrderIDs = new List<string>();
            foreach (var visitor in visitors)
            {
                if (visitor == null || string.IsNullOrEmpty(visitor.ID))
                    continue;

                var queryTicketsResponse = TicketService.QueryTickets(visitor.ID);
                if (queryTicketsResponse != null && queryTicketsResponse.success
                    && queryTicketsResponse.data != null && queryTicketsResponse.data.Count > 0)
                {
                    if(isForBooking)
                    {
                        // 预约参观，只检查有没有配置在文件中的预约参观的ID的票，如果没有该票或者已退票，就可以预约
                        if (queryTicketsResponse.data.Any(t => t.scheduleId == bookingScheduleId && t.status != "T"))
                        {
                            hasOrderIDs.Add(visitor.ID);
                        }
                    }
                    else
                    {
                        // 查询该用户的当日门票，剔除掉预约参观的票种，当日没有其他门票、或者所有门票都已退，则可以继续购买
                        var nonBookingTickets = queryTicketsResponse.data.Where(t => t.scheduleId != bookingScheduleId);
                        if (!(nonBookingTickets.Count() == 0 || nonBookingTickets.All(t => t.status == "T")))
                        {
                            hasOrderIDs.Add(visitor.ID);
                        }
                    }
                }
            }

            return hasOrderIDs;
        }

        private TimeSlotsModel GetVisitBookingTimeSlots()
        {
            var slotList = TicketService.GetSlotList("visitor", DateTime.Today.ToString("yyyy-MM-dd"), "N");
            if (slotList != null && slotList.data?.reservePeriodList?.Count > 0)
            {
                var slotDetailList = new List<ReserveSlotDetailResponse>();
                foreach (var period in slotList.data?.reservePeriodList)
                {
                    var slotDetailResponse = TicketService.GetSlotDetail(period.id);
                    if (slotDetailResponse != null)
                    {
                        slotDetailList.Add(slotDetailResponse);
                    }
                }

                if (slotDetailList.Any())
                {
                    var configuredScheduleIds = GetConfiguredScheduleIds("BookingScheduleId");
                    var scheduleId = (int)configuredScheduleIds[0]; // only one should be cnfigured.

                    var needToCheckTickStockString = ConfigurationManager.AppSettings["NeedToCheckVisitTickStock"];
                    if (!bool.TryParse(needToCheckTickStockString, out bool needToCheckTickStock))
                    {
                        Logger.Error("配置的NeedToCheckVisitTickStock错误。使用默认值false。");
                    }

                    var timeSlots = new TimeSlotsModel();
                    foreach (var slot in slotDetailList)
                    {
                        if (slot.data != null
                            && slot.data.scheduleIdList != null
                            && slot.data.scheduleIdList.Any()
                            && slot.data.scheduleIdList.Contains(scheduleId)
                            && slot.data.reservePeriod != null)
                        {
                            var slotItem = PopulateSlotItem(slot, scheduleId, "test1", needToCheckTickStock);
                            timeSlots.Slots.Add(slotItem);
                        }
                    }
                    if (!timeSlots.Slots.Any())
                    {
                        Logger.Information($"无时间段加入列表！");
                    }

                    return timeSlots;
                }
            }

            return null;
        }

        private TimeSlotsModel GetExhibitionTimeSlots()
        {
            var slotList = TicketService.GetSlotList("visitor", DateTime.Today.ToString("yyyy-MM-dd"), "Y");
            if (slotList != null && slotList.data?.reservePeriodList?.Count > 0)
            {
                var slotDetailList = new List<ReserveSlotDetailResponse>();
                foreach (var period in slotList.data?.reservePeriodList)
                {
                    var slotDetailResponse = TicketService.GetSlotDetail(period.id);
                    if (slotDetailResponse != null)
                    {
                        slotDetailList.Add(slotDetailResponse);
                    }
                }

                if (slotDetailList.Any())
                {
                    var needToCheckTickStockString = ConfigurationManager.AppSettings["NeedToCheckExhibitionTickStock"];
                    if (!bool.TryParse(needToCheckTickStockString, out bool needToCheckTickStock))
                    {
                        Logger.Error("配置的NeedToCheckVisitTickStock错误。使用默认值false。");
                    }

                    var timeSlots = new TimeSlotsModel();
                    foreach (var slot in slotDetailList)
                    {
                        if (slot.data != null
                            && slot.data.scheduleIdList != null
                            && slot.data.scheduleIdList.Any()
                            && slot.data.reservePeriod != null)
                        {
                            var slotItem = PopulateSlotItem(slot, null, "chinaUmsPay", needToCheckTickStock);
                            timeSlots.Slots.Add(slotItem);
                        }
                    }
                    if (!timeSlots.Slots.Any())
                    {
                        Logger.Information($"无时间段加入列表！");
                    }

                    return timeSlots;
                }
            }

            return null;
        }

        private SlotItem PopulateSlotItem(ReserveSlotDetailResponse slot, int? scheduleId, string payMethod, bool needToCheckTickStock)
        {
            var slotItem = new SlotItem();
            slotItem.PayMethod = payMethod;
            slotItem.ReservePeriodId = slot.data.reservePeriod.id;
            if (scheduleId.HasValue)
            {
                slotItem.ScheduleId = scheduleId.Value;
            }

            string slotTimeStartString = string.Empty;
            // "starttime": "2024-03-31 09:00:00"
            if (!string.IsNullOrEmpty(slot.data.reservePeriod.starttime))
            {
                var starttimeString = JsonHelper.ConvertToDateTime(slot.data.reservePeriod.starttime);
                Logger.Information($"转换时间：{slot.data.reservePeriod.starttime} to {starttimeString}");
                var startTimeArray = starttimeString.Split(' ');
                if (startTimeArray != null && startTimeArray.Length == 2)
                {
                    slotTimeStartString = startTimeArray[1];
                }
            }
            string slotTimeEndString = string.Empty;
            DateTime slotTimeEnd = DateTime.Now;
            // "endtime": "2024-03-31 10:00:00"
            if (!string.IsNullOrEmpty(slot.data.reservePeriod.endtime))
            {
                var endtimeString = JsonHelper.ConvertToDateTime(slot.data.reservePeriod.endtime);
                Logger.Information($"转换时间：{slot.data.reservePeriod.endtime} to {endtimeString}");
                // use to compare whether the time is passed.
                if (!DateTime.TryParse(endtimeString, out slotTimeEnd))
                {
                    Logger.Error($"转换预约时间段的结束时间错误。结束时间：{slot.data.reservePeriod.endtime}");
                }
                var endTimeArray = endtimeString.Split(' ');
                if (endTimeArray != null && endTimeArray.Length == 2)
                {
                    slotTimeEndString = endTimeArray[1];
                }
            }

            if (string.IsNullOrEmpty(slotTimeStartString) || string.IsNullOrEmpty(slotTimeEndString))
            {
                Logger.Error($"解析预约时间段错误。起始时间：{slot?.data?.reservePeriod?.starttime}，结束时间：{slot?.data?.reservePeriod?.endtime}");
            }

            var now = DateTime.Now;
            if (now >= slotTimeEnd)
            {
                slotItem.DisplayName = $"{slotTimeStartString}-{slotTimeEndString} (该时段已过)";
                slotItem.TimeRange = $"{slotTimeStartString}-{slotTimeEndString}";
                slotItem.IsAvailable = false;
                slotItem.TimeSlotImagePath = ImagePath.ItemRectangleDisabled;
            }
            else
            {
                var leftCount = slot.data.reservePeriod.visitorsnum - slot.data.reservePeriod.usedNum4Other;
                if (leftCount <= 0)
                {
                    if (needToCheckTickStock)
                    {
                        slotItem.DisplayName = $"{slotTimeStartString}-{slotTimeEndString} (已约满)";
                        slotItem.IsAvailable = false;
                        slotItem.TimeSlotImagePath = ImagePath.ItemRectangleDisabled;
                    }
                    else
                    {
                        slotItem.DisplayName = $"{slotTimeStartString}-{slotTimeEndString}";
                        slotItem.IsAvailable = true;
                        slotItem.TimeSlotImagePath = ImagePath.ItemRectangle;
                    }
                    slotItem.TimeRange = $"{slotTimeStartString}-{slotTimeEndString}";
                }
                else
                {
                    if (needToCheckTickStock)
                    {
                        slotItem.DisplayName = $"{slotTimeStartString}-{slotTimeEndString} (余票{leftCount}张)";
                    }
                    else
                    {
                        slotItem.DisplayName = $"{slotTimeStartString}-{slotTimeEndString}";
                    }

                    slotItem.TimeRange = $"{slotTimeStartString}-{slotTimeEndString}";
                    slotItem.IsAvailable = true;
                    slotItem.TimeSlotImagePath = ImagePath.ItemRectangle;
                }
            }
            return slotItem;
        }

        
        private ExhibitionTicketTypesModel GetTicketTypes(int reservePeriodId, string ticketTimeRange)
        {
            var ticketTypeList = TicketService.GetTicketTypes(reservePeriodId);
            if (ticketTypeList != null && ticketTypeList.data?.Count > 0)
            {
                var scheduleIds = GetConfiguredScheduleIds("ExhibitionScheduleId");
                var ticketTypes = new ExhibitionTicketTypesModel();
                foreach (var ticketTypeFromServer in ticketTypeList.data)
                {
                    if (scheduleIds.Contains(ticketTypeFromServer.id))
                    {
                        var ticketType = new ExhibitionTicketType();
                        ticketType.TicketType = ticketTypeFromServer.cnName;
                        ticketType.TicketPrice = ticketTypeFromServer.showPrice;
                        ticketType.TicketPriceString = $"{ticketTypeFromServer.showPrice}元";
                        ticketType.TicketTypeDescription = GetTicketTypeDescriptionWithLineBreak(ticketTypeFromServer.remark);
                        ticketType.ReservePeriodId = reservePeriodId;
                        ticketType.IsAvailable = ticketTypeFromServer.available == "Y" ? true : false;
                        ticketType.TimeRange = ticketTimeRange;
                        ticketType.ScheduleId = ticketTypeFromServer.id;
                        ticketTypes.ExhibitionTicketTypes.Add(ticketType);
                    }
                }

                return ticketTypes;
            }

            return null;
        }

        private IList<long> GetConfiguredScheduleIds(string configName)
        {
            var result = new List<long>();
            var tempString = ConfigurationManager.AppSettings[configName];
            var configuredScheduleIdStrings = tempString?.Split(',');
            bool hasBadConfiguredValue = false;
            foreach (var scheduleIdString in configuredScheduleIdStrings)
            {
                if (!string.IsNullOrWhiteSpace(scheduleIdString) && long.TryParse(scheduleIdString?.Trim(), out long scheduleId))
                {
                    result.Add(scheduleId);
                }
                else
                {
                    hasBadConfiguredValue = true;
                }
            }
            if (hasBadConfiguredValue)
            {
                Logger.Error($"配置的{configName}有错误。");
            }
            return result;
        }

        private void BuyExhibitionFreeTickets()
        {
            ExhibitionPaymentCountDownView.SetValue(Visibility.Hidden, string.Empty,
                        ExhibitionOrderView.TicketType.TicketType,
                        ExhibitionOrderView.VisitorCount,
                        ExhibitionOrderView.TotalPrice,
                        "test1",
                        Visibility.Visible, 120, paymentRequestGuid);
            AttachOnlyView(RegionNames.CenterRegion, ExhibitionPaymentCountDownView);
            ExhibitionPaymentCountDownView.ReadyToScan = false;
            ExhibitionPaymentCountDownView.IsTimerEnable = true;

            var request = CreateRequestVisitBookingModel(ExhibitionOrderView.TicketType.ReservePeriodId,
                    "test1", ExhibitionOrderView.TicketType.ScheduleId,
                    ExhibitionOrderView.VisitorCount, ExhibitionOrderView.Visitors);

            var response = TicketService.PlaceVistBookingOrder(request);
            if (response != null && response.success)
            {
                QueryAndPrintExhibitionTickets(response.data.tradeNo);
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MsgView.SetValue(Visibility.Hidden,
                    string.Empty,
                    ImagePath.Error,
                    "特展购票失败，请重试或者联系管理员！",
                    Visibility.Visible, 30,
                    ExhibitionOrderView);
                    AttachOnlyView(RegionNames.CenterRegion, MsgView);
                    ExhibitionPaymentCountDownView.Reset();
                    MsgView.IsTimerEnable = true;
                });
            }
        }

        private void BuyExhibitionNonFreeTickets()
        {
            paymentRequestGuid = Guid.NewGuid();
            ExhibitionPaymentCountDownView.SetValue(Visibility.Hidden, string.Empty,
            ExhibitionOrderView.TicketType.TicketType,
            ExhibitionOrderView.VisitorCount,
            ExhibitionOrderView.TotalPrice,
            PayMethods.GetPayMethodString(ExhibitionOrderView.PayMethod),
            Visibility.Visible, 120, paymentRequestGuid);
            AttachOnlyView(RegionNames.CenterRegion, ExhibitionPaymentCountDownView);
            ExhibitionPaymentCountDownView.ReadyToScan = true;
            ExhibitionPaymentCountDownView.IsTimerEnable = true;

            // waiting for scanning the payment barcode.
        }

        private void OnPaymentScanDataReceived(object sender, BarCodeScanDataEventArgs e)
        {
            if (e.RequestGuid == paymentRequestGuid)
            {
                ExhibitionPaymentCountDownView.ReadyToScan = false;
                PlaceBuyExhibitionTicketsOrder(e.ScannedData, "chinaUmsPay");
            }
        }

        private void PlaceBuyExhibitionTicketsOrder(string scannedData, string payMethod)
        {
            Logger.Information($"Payment scan data is received! The length is {scannedData.Length}.");

            var request = CreateRequestTicketPayment(scannedData, ExhibitionOrderView.TicketType.ReservePeriodId,
                payMethod, ExhibitionOrderView.TicketType.ScheduleId,
                ExhibitionOrderView.VisitorCount, ExhibitionOrderView.Visitors);

            var response = TicketService.PlaceBuyExhibitionTicketOrder(request);
            if (response != null && response.success && response.data != null)
            {
                var tradeNo = response.data.tradeNo;
                var serialNo = response.data.serialNo;
                var isPaymentSuccess = QueryExhibitionTicketPaymentStatus(tradeNo, serialNo);
                if (isPaymentSuccess)
                {
                    QueryAndPrintExhibitionTickets(tradeNo);
                    return;
                }
            }
            var msg = !string.IsNullOrEmpty(response.msg) ? $"特展购票下单失败: \n{response.msg}\n请重试或者联系管理员！" : $"特展购票下单失败\n请重试或者联系管理员！";
            Dispatcher.Invoke(() =>
            {
                MsgView.SetValue(Visibility.Hidden,
                string.Empty,
                ImagePath.Error,
                msg,
                Visibility.Visible, 30,
                ExhibitionOrderView);

                AttachOnlyView(RegionNames.CenterRegion, MsgView);
                ExhibitionPaymentCountDownView.Reset();
                MsgView.IsTimerEnable = true;
            });
        }

        private bool QueryExhibitionTicketPaymentStatus(string tradeNo, string serialNo)
        {
            bool isPaymentOk = false;
            for (int i = 0; i < 10; ++i)
            {
                System.Threading.Thread.Sleep(2000);

                var paymentResponse = TicketService.QueryExhibitionPaymentStatus(tradeNo, serialNo);
                if (paymentResponse != null && paymentResponse.success)
                {
                    isPaymentOk = true;
                    break;
                }
                Logger.Information($"Checking payment status {i} time(s).");
            }

            if (!isPaymentOk)
            {
                Dispatcher.Invoke(() =>
                {
                    MsgView.SetValue(Visibility.Hidden,
                    string.Empty,
                    ImagePath.Error,
                    $"特展购票支付失败。\n订单号码：{tradeNo}。\n请重试或联系管理员！",
                    Visibility.Visible, 60,
                    ExhibitionOrderView);

                    AttachOnlyView(RegionNames.CenterRegion, MsgView);
                    ExhibitionPaymentCountDownView.Reset();
                    MsgView.IsTimerEnable = true;
                });
            }

            return isPaymentOk;
        }

        private void QueryAndPrintExhibitionTickets(string tradeNo)
        {
            var queryTicketsResponse = TicketService.QueryTickets(ExhibitionOrderView.Visitors[0]?.ID);
            if (queryTicketsResponse != null && queryTicketsResponse.success && queryTicketsResponse.data.Count > 0)
            {
                var ticketsToPrint = new List<TicketItemViewModel>();
                foreach (var ticketData in queryTicketsResponse.data)
                {
                    if (string.Compare(ticketData.voucherNo, tradeNo, true) == 0)
                    {
                        var ticket = new TicketItemViewModel(ticketData);
                        ticketsToPrint.Add(ticket);
                    }
                    else
                    {
                        Logger.Information($"Ticket {ticketData.uuid} is not printed as its order number {ticketData.voucherNo} is different from {tradeNo}");
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    AttachOnlyView(RegionNames.HomeRegion, ExhibitionPrintTicketView);
                    ExhibitionPaymentCountDownView.Reset();
                    ExhibitionPrintTicketView.WaitingForPrintMessage = $"正在打印{ticketsToPrint.Count}张门票。\n请稍等...";
                    ExhibitionPrintTicketView.PrintTickets(PrintManager, ticketsToPrint, tradeNo);
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MsgView.SetValue(Visibility.Hidden,
                    string.Empty,
                    ImagePath.Error,
                    $"特展购票已经成功，但是获取票务信息失败。\n订单号码：{tradeNo}。\n请联系管理员！",
                    Visibility.Visible,
                    60,
                    ExhibitionOrderView);

                    AttachOnlyView(RegionNames.CenterRegion, MsgView);
                    ExhibitionPaymentCountDownView.Reset();
                    MsgView.IsTimerEnable = true;
                });
            }
        }

        private RequestVisitBookingModel CreateRequestVisitBookingModel(int reservePeriodId, string payMethod,
            int scheduleId, int quantity, IEnumerable<VisitorModel> visitors)
        {
            var request = new RequestVisitBookingModel();
            request.payMethod = payMethod;
            request.reservePeriodId = reservePeriodId;
            request.scheduleReqs = new List<ScheduleRequest>();
            var scheduleReq = new ScheduleRequest();
            scheduleReq.scheduleId = scheduleId;
            scheduleReq.quantity = quantity;
            scheduleReq.certificateReqs = new List<CertificateRequest>();
            foreach (var visitor in visitors)
            {
                var certReq = new CertificateRequest();
                certReq.certificateNo = visitor.ID;
                certReq.certificateType = visitor.CertType;

                scheduleReq.certificateReqs.Add(certReq);
            }
            request.scheduleReqs.Add(scheduleReq);

            return request;
        }

        private RequestTicketPayment CreateRequestTicketPayment(string scannedData, int reservePeriodId, string payMethod,
            int scheduleId, int quantity, IEnumerable<VisitorModel> visitors)
        {
            var request = new RequestTicketPayment();
            request.authCode = scannedData;
            request.payMethod = payMethod;
            request.reservePeriodId = reservePeriodId;
            request.scheduleReqs = new List<ScheduleRequest>();
            var scheduleReq = new ScheduleRequest();
            scheduleReq.scheduleId = scheduleId;
            scheduleReq.quantity = quantity;
            scheduleReq.certificateReqs = new List<CertificateRequest>();
            foreach (var visitor in visitors)
            {
                var certReq = new CertificateRequest();
                certReq.certificateNo = visitor.ID;
                certReq.certificateType = visitor.CertType;

                scheduleReq.certificateReqs.Add(certReq);
            }
            request.scheduleReqs.Add(scheduleReq);

            return request;
        }

        private string GetTicketTypeDescriptionWithLineBreak(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                var sb = new StringBuilder(description);

                int times = description.Length / maxCharNumberInOneLine;
                for (int i = 0; i < times; ++i)
                {
                    sb.Insert(maxCharNumberInOneLine * (i + 1), "\r\n");
                }

                return sb.ToString();
            }

            return description;
        }

        #endregion
    }
}