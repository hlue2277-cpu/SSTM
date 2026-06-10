using System.Collections.Generic;

namespace DataService
{
    public class TicketResponseData
    {
        public string uuid { get; set; }
        public string seatid { get; set; }
        public string voucherNo { get; set; }
        public string voucherId { get; set; }
        public string ticketType { get; set; }
        public string scheduleCnName { get; set; }
        public string stadiumId { get; set; }
        public string stadiumCnName { get; set; }
        public string programId { get; set; }
        public string programCnName { get; set; }
        public string programStartTime { get; set; }
        public string programEndTime { get; set; }
        public double ticketPrice { get; set; }
        public double discountAmount { get; set; }
        public string serialNum { get; set; }
        public long scheduleId { get; set; }
        public string payMethod { get; set; }
        public double discount { get; set; }
        public string realname { get; set; }
        public string certificateType { get; set; }
        public string certificateNo { get; set; }
        public string certificateMd5 { get; set; }
        public string showPrice { get; set; }
        public string seatLabel { get; set; }
        public string operatorName { get; set; }
        public string printNum { get; set; }
        public string printTime { get; set; }
        public string validTime { get; set; }
        public string status { get; set; }
        public string certificateTypeText { get; set; }
        public string playDate { get; set; }
        public string playTime { get; set; }
        public string lineno { get; set; }
        public string rankno { get; set; }
        public string programEnName { get; set; }
        public string userName { get; set; }
        public string scheduleEnName { get; set; }
        public string scheduleRemark { get; set; }
        public string TwoBarcodeInfo { get => uuid; }
        public string oneCode { get; set; }
        public string twoCode { get; set; }
        public string tempPrientInfo { get; set; }
        public string priceDescription { get; set; }
        public string priceRemark { get; set; }
        public string areaCnName { get; set; }
        public string areaEnName { get; set; }
        public string areaDescription { get; set; }
        public string areaIcon { get; set; }
        public string venueCnName { get; set; }
        public string venueEnName { get; set; }
        public string stadiumEnName { get; set; }
        public string cnAddress { get; set; }
        public string enAddress { get; set; }
        public string packInfo { get; set; }
        public string packInfo2 { get; set; }
        public string packIntro { get; set; }
        public string greetings { get; set; }
        public string customerName { get; set; }
        public string userAbbrev { get; set; }
        public string scheduleCode { get; set; }
        public string idnumber { get; set; }
        public string venueAreaId { get; set; }
        public bool printable { get; set; }
        public string statusText { get; set; }
    }
}