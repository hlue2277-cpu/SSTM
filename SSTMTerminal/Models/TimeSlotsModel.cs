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
    public class TimeSlotsModel
    {
        public TimeSlotsModel()
        {
            Slots = new ObservableCollection<SlotItem>();
        }

        public IList<SlotItem> Slots {  get; set; }
    }

    public class SlotItem
    {
        public SlotItem()
        {
            TimeSlotImagePath = ImagePath.ItemRectangle;
        }
        // 09:00-09:45 (余票8张) or 09:00-09:45 (已约满) or 09:00-09:45 (该时段已过)
        public string DisplayName { get; set;}
        public string TimeRange { get; set;}
        // false if no available tickets or time passed
        public bool IsAvailable { get; set; }
        public string TimeSlotImagePath { get; set; }
        public int ScheduleId { get; set; }
        public int ReservePeriodId { get; set; }
        public string PayMethod { get; set; }
    }
}
