using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Calendar
{
    public class DayViewModel
    {
        public DateTime Date { get; set; }
        public List<TaskModel>? Tasks { get; set; }
        public List<EventModel>? Events { get; set; }
    }
}
