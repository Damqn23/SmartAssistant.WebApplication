using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models
{
    public class ReminderEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Reminder message is required")]
        public string ReminderMessage { get; set; }

        [Required(ErrorMessage = "Reminder date is required")]
        [DataType(DataType.DateTime)]
        public DateTime ReminderDate { get; set; }
    }
}
