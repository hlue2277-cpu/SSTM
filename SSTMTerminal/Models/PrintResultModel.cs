using System;

namespace SSTMTerminal.Models
{
    public class PrintResultModel
    {
        public bool hasprinted { get; set; }
        public string resourceid { get; set; }
        public string resourcename { get; set; }
        public DateTime? printedtime { get; set; }
    }
}