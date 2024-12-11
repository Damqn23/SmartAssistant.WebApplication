using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Reminder
    {
		public int Id { get; set; } 
		[Required(ErrorMessage = "Reminder message is required")]
		[Comment("Reminder message")]
        [Column("ReminderMessage")]
        public string ReminderMessage { get; set; } 
		[Required(ErrorMessage = "Reminder date is required")]
		[Comment("Reminder date")]
		[Column("ReminderDate")]
		public DateTime ReminderDate { get; set; } 
		[ForeignKey("User")]
		[Comment("User foreign key")]
		public string UserId { get; set; } 
		public User User { get; set; } 
		
		
	}
}
