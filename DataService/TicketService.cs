using DataService.Helpers;
using Genesis;
using Genesis.DynamicProxy;
using Genesis.Logging;
using Genesis.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace DataService
{
    public interface ITicketService : IService
    {
        LoginResponse Login(string username, string pwd);

        ResponseModel<List<TicketResponseData>> QueryTickets(string cellphoneNumber, string orderId);

        ResponseModel<List<TicketResponseData>> QueryTickets(string ownerId);

        ResponseModel<TicketTemplateResponseData> QueryTicketPrintTemplate(string scheduleId);

        ResponseModel<object> NotifyTicketPrinted(string uuid);

        VisitBookingResponse PlaceVistBookingOrder(RequestVisitBookingModel ticketModel);

        ReserveSlotListResponse GetSlotList(string reserveType, string reserveDate, string isForExhitition);

        ReserveSlotDetailResponse GetSlotDetail(int reservePeriodId);

        TicketTypeResponse GetTicketTypes(int reservePeriodId);

        ExhibitionTicketOrderResponse PlaceBuyExhibitionTicketOrder(RequestTicketPayment paymentRequest);

        ExhibitionPaymentStatusResponse QueryExhibitionPaymentStatus(string tradeNo, string serialNo);

    }

    public class TicketService : ITicketService
    {
        private string baseUrl = ConfigurationManager.AppSettings["ServiceBaseUrl"];

        private string loginUrl = "/ticket/common/asynchLogin.xhtml";

        private string queryTicketUrl = "/ticket/machine/query.xhtml";

        private string queryTicketPrintTemplateUrl = "/ticket/machine/getTicketLayout.xhtml";

        private string notifyTicketPrintedUrl = "/ticket/machine/notify.xhtml";

        private string visitBookingUrl = "/ticket/home/ticket/sell/drawAddVoucherMachine.xhtml";

        private string getReservablePeriodListUrl = "/ticket/home/reserve/getReservePeriodListByType.xhtml";

        private string getReservablePeriodDetailUrl = "/ticket/home/reserve/getReservePeriod.xhtml";

        private string getTicketTypeUrl = "/ticket/home/reserve/getScheduleListByReservePeriodId.xhtml";

        private string exhibitionBuyTicketUrl = "/ticket/home/ticket/sell/drawAddVoucherChinaUmsPay.xhtml";

        private string paymentQueryUrl = "/ticket/home/order/getChinaUmsOrderStatus.xhtml";

        public ILogger Logger { get; set; }

        public TicketService(ILogger logger)
        {
            Logger = logger;
        }

        public ResponseModel<List<TicketResponseData>> QueryTickets(string cellphoneNumber, string orderId)
        {
            ResponseModel<List<TicketResponseData>> tickets = null;

            var parameters = RequesSystemParamHelper.GenerateSystemParameter();
            parameters.Add("mobile", cellphoneNumber);
            parameters.Add("serialNo", orderId);
            parameters.Add("deviceId", Environment.MachineName);
            var signValue = SignHelper.SignWithMD5(parameters, ConfigurationManager.AppSettings["SecretKey"]);
            parameters.Add("sign", signValue);

            var parameterString = SignHelper.ConvertToParameterString(parameters, null, true);

            var result = HttpClientHelper.Get(baseUrl + queryTicketUrl, parameterString);

            if (!string.IsNullOrEmpty(result))
            {
                tickets = JsonHelper.JsonDeserialize<ResponseModel<List<TicketResponseData>>>(result);
            }

            return tickets;
        }

        public ResponseModel<List<TicketResponseData>> QueryTickets(string ownerId)
        {
            ResponseModel<List<TicketResponseData>> tickets = null;

            var parameters = RequesSystemParamHelper.GenerateSystemParameter();
            parameters.Add("uuid", ownerId);
            parameters.Add("deviceId", Environment.MachineName);
            var signValue = SignHelper.SignWithMD5(parameters, ConfigurationManager.AppSettings["SecretKey"]);
            parameters.Add("sign", signValue);

            var parameterString = SignHelper.ConvertToParameterString(parameters, null, true);

            var result = HttpClientHelper.Get(baseUrl + queryTicketUrl, parameterString);

            if (!string.IsNullOrEmpty(result))
            {
                tickets = JsonHelper.JsonDeserialize<ResponseModel<List<TicketResponseData>>>(result);
                Logger.Information(result);
            }

            return tickets;
        }

        public ResponseModel<TicketTemplateResponseData> QueryTicketPrintTemplate(string scheduleId)
        {
            ResponseModel<TicketTemplateResponseData> template = null;

            var parameters = RequesSystemParamHelper.GenerateSystemParameter();
            parameters.Add("scheduleId", scheduleId);
            parameters.Add("deviceId", Environment.MachineName);
            var signValue = SignHelper.SignWithMD5(parameters, ConfigurationManager.AppSettings["SecretKey"]);
            parameters.Add("sign", signValue);

            var parameterString = SignHelper.ConvertToParameterString(parameters, null, true);

            var result = HttpClientHelper.Get(baseUrl + queryTicketPrintTemplateUrl, parameterString);

            if (!string.IsNullOrEmpty(result))
            {
                template = JsonHelper.JsonDeserialize<ResponseModel<TicketTemplateResponseData>>(result);
            }

            return template;
        }

        public ResponseModel<object> NotifyTicketPrinted(string uuid)
        {
            ResponseModel<object> response = null;

            var parameters = RequesSystemParamHelper.GenerateSystemParameter();
            parameters.Add("uuid", uuid);
            parameters.Add("deviceId", Environment.MachineName);
            var signValue = SignHelper.SignWithMD5(parameters, ConfigurationManager.AppSettings["SecretKey"]);
            parameters.Add("sign", signValue);

            var parameterString = SignHelper.ConvertToParameterString(parameters, null, true);

            var result = HttpClientHelper.Get(baseUrl + notifyTicketPrintedUrl, parameterString);

            if (!string.IsNullOrEmpty(result))
            {
                response = JsonHelper.JsonDeserialize<ResponseModel<object>>(result);
            }

            return response;
        }

        public LoginResponse Login(string username, string pwd)
        {
            LoginResponse loginResponse = null;

            var parameters = RequesSystemParamHelper.GenerateSystemParameter();
            parameters.Add("username", username);
            parameters.Add("password", pwd);
            var signValue = SignHelper.SignWithMD5(parameters, ConfigurationManager.AppSettings["SecretKey"]);
            parameters.Add("sign", signValue);

            var parameterString = SignHelper.ConvertToParameterString(parameters, null, true);
            var result = HttpClientHelper.Post(baseUrl + loginUrl, parameterString);

            if (!string.IsNullOrEmpty(result))
            {
                loginResponse = JsonHelper.JsonDeserialize<LoginResponse>(result);
                Logger.Information(result);
            }

            return loginResponse;
        }

        public VisitBookingResponse PlaceVistBookingOrder(RequestVisitBookingModel ticketModel)
        {
            VisitBookingResponse response = null;

            var command = JsonHelper.JsonSerializer<RequestVisitBookingModel>(ticketModel);
            var encodedParamString = HttpUtility.UrlEncode(command, Encoding.UTF8);
            var encodedRequestString = $"command={encodedParamString}";
            var result = HttpClientHelper.Post(baseUrl + visitBookingUrl, encodedRequestString);

            if (!string.IsNullOrEmpty(result))
            {
                response = JsonHelper.JsonDeserialize<VisitBookingResponse>(result);
                Logger.Information(result);
            }

            return response;
        }

        public ReserveSlotListResponse GetSlotList(string reserveType, string reserveDate, string isForExhitition)
        {
            ReserveSlotListResponse response = null;
            string status = "Y";

            var requestString = $"reserveType={reserveType}&reservedate={reserveDate}&isTemp={isForExhitition}&status={status}";

            var result = HttpClientHelper.Post(baseUrl + getReservablePeriodListUrl, requestString);

            if (!string.IsNullOrEmpty(result))
            {
                Logger.Information(result);
                response = JsonHelper.JsonDeserialize<ReserveSlotListResponse>(result);
            }
            return response;
        }

        public ReserveSlotDetailResponse GetSlotDetail(int reservePeriodId)
        {
            ReserveSlotDetailResponse response = null;

            var requestString = $"id={reservePeriodId}";
            var result = HttpClientHelper.Post(baseUrl + getReservablePeriodDetailUrl, requestString);

            if (!string.IsNullOrEmpty(result))
            {
                response = JsonHelper.JsonDeserialize<ReserveSlotDetailResponse>(result);
                Logger.Information(result);
            }

            return response;
        }

        public TicketTypeResponse GetTicketTypes(int reservePeriodId)
        {
            TicketTypeResponse response = null;

            var requestString = $"reservePeriodId={reservePeriodId}";
            var result = HttpClientHelper.Post(baseUrl + getTicketTypeUrl, requestString);

            if (!string.IsNullOrEmpty(result))
            {
                response = JsonHelper.JsonDeserialize<TicketTypeResponse>(result);
                Logger.Information(result);
            }

            return response;
        }

        public ExhibitionTicketOrderResponse PlaceBuyExhibitionTicketOrder(RequestTicketPayment paymentRequest)
        {
            ExhibitionTicketOrderResponse response = null;

            var command = JsonHelper.JsonSerializer<RequestTicketPayment>(paymentRequest);
            var encodedParamString = HttpUtility.UrlEncode(command, Encoding.UTF8);
            var encodedRequestString = $"command={encodedParamString}";
            var result = HttpClientHelper.Post(baseUrl + exhibitionBuyTicketUrl, encodedRequestString);

            if (!string.IsNullOrEmpty(result))
            {
                response = JsonHelper.JsonDeserialize<ExhibitionTicketOrderResponse>(result);
                Logger.Information(result);
            }

            return response;
        }

        public ExhibitionPaymentStatusResponse QueryExhibitionPaymentStatus(string tradeNo, string serialNo)
        {
            ExhibitionPaymentStatusResponse response = null;

            var requestString = $"tradeNo={tradeNo}&serialNo={serialNo}";
            var result = HttpClientHelper.Post(baseUrl + paymentQueryUrl, requestString);

            if (!string.IsNullOrEmpty(result))
            {
                response = JsonHelper.JsonDeserialize<ExhibitionPaymentStatusResponse>(result);
                Logger.Information(result);
            }

            return response;
        }

    }
}