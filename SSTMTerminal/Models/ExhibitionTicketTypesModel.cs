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
    public class ExhibitionTicketTypesModel
    {
        public ExhibitionTicketTypesModel()
        {
            ExhibitionTicketTypes = new ObservableCollection<ExhibitionTicketType>();
        }

        public string ExhibitionName { get; set; }

        public IList<ExhibitionTicketType> ExhibitionTicketTypes { get; set; }
    }

    public class ExhibitionTicketType
    {
        public ExhibitionTicketType()
        {
            ExhibitionTicketTypeImagePath = ImagePath.ItemRectangleDisabled;
        }
        public string TicketType { get; set;}
        public string TicketPriceString { get; set;}
        public float TicketPrice { get; set; }
        public string TicketTypeDescription { get; set; }
        // false if no available tickets or time passed
        public bool IsAvailable { get; set; }
        public string Active { get; set; }
        public string ExhibitionTicketTypeImagePath { get; set; }
        public int ScheduleId { get; set; }
        public int ReservePeriodId { get; set; }
        public string TimeRange { get; set; }
    }
}
