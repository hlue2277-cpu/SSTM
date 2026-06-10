namespace DataService
{
    public class ExhibitionTicketOrderResponse
    {
        public bool success { get; set; }
        public string errorcode { get; set; }
        public string msg { get; set; }
        public ExhibitionTicketOrderResponseData data { get; set; }
    }

    public class ExhibitionTicketOrderResponseData
    {
        public string tradeNo { get; set; }
        public string serialNo { get; set; }
        public string cmd { get; set; }
        public string token { get; set; }

    }
}