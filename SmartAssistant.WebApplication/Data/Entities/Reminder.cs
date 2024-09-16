using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Reminder
    {
        public int Id { get; set; } // Unique identifier


        [Comment("Reminder message")]
        public string ReminderMessage { get; set; } // Reminder message


        [Comment("Reminder date")]
        public DateTime ReminderDate { get; set; } // Reminder date

        [ForeignKey("User")]
        [Comment("User foreign key")]
        public int UserId { get; set; } // User foreign key

        public User User { get; set; }  // Navigation property
    }
}
