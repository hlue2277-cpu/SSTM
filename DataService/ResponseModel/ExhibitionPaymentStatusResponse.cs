using System.Collections.Generic;

namespace DataService
{
    public class ExhibitionPaymentStatusResponse
    {
        public bool success { get; set; }
        public string errcode { get; set; }
        public string mst { get; set; }
        public string data { get; set; }
    }
}