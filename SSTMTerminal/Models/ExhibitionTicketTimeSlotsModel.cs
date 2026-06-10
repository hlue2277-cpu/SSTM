using Genesis;
using SSTMTerminal.Images;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal.Models
{
    public class ExhibitionTicketTimeSlotsModel
    {
        public ExhibitionTicketTimeSlotsModel()
        {
            ExhibitionTicketTimeSlots = new ObservableCollection<ExhibitionTicketTimeSlot>();
        }

        public string ExhibitionAndTicketTypeName { get; set; }

        public IList<ExhibitionTicketTimeSlot> ExhibitionTicketTimeSlots { get; set; }
    }

    public class ExhibitionTicketTimeSlot
    {
        public ExhibitionTicketTimeSlot()
        {
            ExhibitionTicketTimeSlotImagePath = ImagePath.ItemRectangleDisabled;
        }
        public string TicketType { get; set;}
        public float TicketPrice { get; set;}
        // false if no available tickets or time passed
        public bool IsAvailable { get; set; }
        public string ExhibitionTicketTimeSlotImagePath { get; set; }
        public int ScheduleId { get; set; }
        public string ReservePeriodId { get; set; }
        // 09:00-09:45 (余票8张) or 09:00-09:45 (已约满) or 09:00-09:45 (该时段已过)
        public string DisplayName { get; set; }
        public string TimeRange { get; set; }
        // false if no available tickets or time passed
        public string PayMethod { get; set; }
    }
}
