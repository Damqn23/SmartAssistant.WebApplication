using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAssistant.Shared.Models.Reminder
{
    public class ReminderModel
    {
        public int Id { get; set; } // Make nullable

        [Required(ErrorMessage = "Reminder message is required")]
        public string ReminderMessage { get; set; }

        [Required(ErrorMessage = "Reminder date is required")]
        public DateTime ReminderDate { get; set; }

        public string UserId { get; set; }

        public UserModel User { get; set; }
    }
}
