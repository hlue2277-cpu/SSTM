using System.Collections.Generic;

namespace DataService
{
    public class RequestTicketPayment
    {
        public string payMethod { get; set; }
        public string authCode { get; set; }
        public int reservePeriodId { get; set; }
        public List<ScheduleRequest> scheduleReqs { get; set; }
    }
}