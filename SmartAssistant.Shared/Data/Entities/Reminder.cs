using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Reminder
    {
		public int Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Reminder message is required")]
		[Comment("Reminder message")]
        [Column("ReminderMessage")]
        public string ReminderMessage { get; set; } // Reminder message

		[Required(ErrorMessage = "Reminder date is required")]
		[Comment("Reminder date")]
		[Column("ReminderDate")]
		public DateTime ReminderDate { get; set; } // Reminder date

		[ForeignKey("User")]
		[Comment("User foreign key")]
		public string UserId { get; set; } // User foreign key

		public User User { get; set; } // Navigation property

		
		
	}
}
