using System.Collections.Generic;

namespace DataService
{
    public class TicketItemDto
    {
        public long resourceid { get; set; }

        public int quantity { get; set; }

        public string ticketname { get; set; }

        public string ownername { get; set; }

        public TicketItemDto()
        {
        }
    }
}