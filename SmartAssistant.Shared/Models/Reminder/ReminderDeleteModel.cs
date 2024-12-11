using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Reminder
{
    public class ReminderDeleteModel
    {
        public int Id { get; set; } 
        public string ReminderMessage { get; set; } 
        public DateTime ReminderDate { get; set; }     }


}
