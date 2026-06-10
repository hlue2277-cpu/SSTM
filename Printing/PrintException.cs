using System;

namespace Printing
{
    public class PrintException : Exception
    {
        public PrintException(int total, int printed, string tradeNO = null, string message =""): base(message)
        {
            TotalTicketNum = total;
            PrintedTicketNum = printed;
            TradeNO = tradeNO;
        }

        public int TotalTicketNum { get; set; }
        public int PrintedTicketNum { get; set; }
        public string TradeNO { get; set; }
    }
}
