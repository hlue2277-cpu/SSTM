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
    public class ExhibitionsModel
    {
        public ExhibitionsModel()
        {
            Exhibitions = new ObservableCollection<ExhibitionItem>();
        }

        public IList<ExhibitionItem> Exhibitions {  get; set; }
    }

    public class ExhibitionItem
    {
        public ExhibitionItem()
        {
            // It's just use this image as background, not indicating it's disabled.
            ExhibitionImagePath = ImagePath.ItemRectangleDisabled;
        }
        public string DisplayName { get; set;}
        public string TimeRange { get; set;}
        // false if no available tickets or time passed
        public bool IsAvailable { get; set; }
        public string ExhibitionImagePath { get; set; }
        public int ScheduleId { get; set; }
        public string ReservePeriodId { get; set; }
        public string PayMethod { get; set; }
    }
}
