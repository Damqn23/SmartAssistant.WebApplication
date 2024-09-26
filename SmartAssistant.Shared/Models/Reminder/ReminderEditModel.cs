using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Reminder
{
    public class ReminderEditModel
    {
        public int Id { get; set; } // Ensure this is populated for editing
        [Required]
        public string ReminderMessage { get; set; }
        [Required]
        public DateTime ReminderDate { get; set; }
    }

}
