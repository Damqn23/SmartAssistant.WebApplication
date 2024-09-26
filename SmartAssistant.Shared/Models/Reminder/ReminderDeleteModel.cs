using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Reminder
{
    public class ReminderDeleteModel
    {
        public int Id { get; set; } // Reminder ID

        public string ReminderMessage { get; set; } // Reminder message

        public DateTime ReminderDate { get; set; } // Add this property to hold the reminder date
    }


}
