using System.Collections.Generic;

namespace DataService
{
    public class ReserveSlotDetailResponse
    {
        public bool success { get; set; }
        public string errcode { get; set; }
        public ReserveSlotDetailData data { get; set; }
    }

    public class ReserveSlotDetailData
    {
        public List<int> scheduleIdList { get; set; }
        public ReserveSlotDetail reservePeriod {  get; set; }
    }

    public class ReserveSlotDetail
    {
        public int id { get; set; }
        public int? sourceId { get; set; }
        public int stadiumId { get; set; }
        public string addtime { get; set; }
        public string updatetime { get; set; }
        public string reservedate { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public int week { get; set; }
        public int visitorsnum { get; set; }
        public int teamsnum { get; set; }
        public int teamtermnum { get; set; }
        public int otasnum { get; set; }
        public int membershipsnum { get; set; }
        public string status { get; set; }
        public string onlineSupport { get; set; }
        public int addUserId { get; set; }
        public int userGroupId { get; set; }
        public int usedNum4Team { get; set; }
        public int usedNum4Ota { get; set; }
        public int usedNum4Other { get; set; }
        public int usedNum4Teamterm { get; set; }
        public int usedNum4Membership { get; set; }

        //2026.6.25
        public string sessionName { get; set; }
    }
}