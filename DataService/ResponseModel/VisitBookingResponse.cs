namespace DataService
{
    public class VisitBookingResponse
    {
        public bool success { get; set; }
        public string errorcode { get; set; }
        public VisitBookingResponseData data { get; set; }
    }

    public class VisitBookingResponseData
    {
        public string tradeNo { get; set; }
        public string cmd { get; set; }
        public string token { get; set; }

    }
}