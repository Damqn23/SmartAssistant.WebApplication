using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Event
{
    public class EventModel
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public string UserId { get; set; }
    }
}
