using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Calendar
{
    public class CalendarViewModel
    {
        public List<DayViewModel> Days { get; set; }
        public DateTime CurrentMonth { get; set; }
        public string TeamOwnerUserName { get; set; } // Add this property to hold the team owner's username
        public int TeamId { get; set; } // Add this property

    }
}
