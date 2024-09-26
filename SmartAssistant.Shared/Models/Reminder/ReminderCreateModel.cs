using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Reminder
{
    public class ReminderCreateModel
    {
        [Required(ErrorMessage = "Reminder message is required")]
        public string ReminderMessage { get; set; }

        [Required(ErrorMessage = "Reminder date is required")]
        public DateTime ReminderDate { get; set; }
    }
}
