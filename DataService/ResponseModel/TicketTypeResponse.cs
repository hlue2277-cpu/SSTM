using System.Collections.Generic;

namespace DataService
{
    public class TicketTypeResponse
    {
        public bool success { get; set; }
        public string errcode { get; set; }
        public List<TicketType> data { get; set; }
    }

    public class TicketType
    {
        public int id { get; set; }
        public int stadiumId { get; set; }
        public string addtime { get; set; }
        public string updatetime { get; set; }
        public string code { get; set; }
        public string cnName { get; set; }
        public string active { get; set; }
        public string available { get; set; }
        public int venueId { get; set; }
        public string internalname { get; set; }
        public string tickettype { get; set; }
        public float showPrice { get; set; }
        public int templateId { get; set; }
        public int totalNum { get; set; }
        public string remark { get; set; }
        public string checkcard { get; set; }
        public string discountType { get; set; }
        public string supportRefund { get; set; }
        public int maxChangeNum { get; set; }
        public string category { get; set; }
        public int sortNum { get; set; }
        public string description { get; set; }
        public string showText { get; set; }
        public string supportChange { get; set; }
        public int maxChangeTimes { get; set; }
        public string rule { get; set; }
    }
}