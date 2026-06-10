using System.Collections.Generic;

namespace DataService
{
    public class RequestVisitBookingModel
    {
        public string payMethod { get; set; }

        public int reservePeriodId { get; set; }

        public List<ScheduleRequest> scheduleReqs { get; set; }
    }

    public class ScheduleRequest
    {
        public int scheduleId { get; set; }
        public int quantity { get; set; }
        public List<CertificateRequest> certificateReqs { get; set; }
    }

    public class CertificateRequest
    {
        public CertificateRequest()
        {
            realname = "现场预约";
            adultRealname = "";
            adultCertificateType = "";
            adultCertificateNo = "";
            extraCertType = "appoint";
        }

        public string realname { get; set; }
        public string certificateType { get; set; }
        public string certificateNo { get; set; }
        public string adultRealname { get; set; }
        public string adultCertificateType { get;set; }
        public string adultCertificateNo { get;set; }
        public string extraCertType { get; set; }
    }
}