using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartAssistant.WebApplication.Utilities.Constants;


namespace SmartAssistant.WebApp.Data.Entities
{
    public class Event
    {
		public int Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Event title is required")]
		[MaxLength(EventTitleMaxLength, ErrorMessage = "Event title is too long")]
		[Comment("Event title")]
		public string EventTitle { get; set; } // Event title

		[Comment("Event date")]
		public DateTime EventDate { get; set; } // Event date

		[ForeignKey("User")]
		[Comment("User foreign key")]
		public string UserId { get; set; } // User foreign key

		public User User { get; set; } // Navigation property
	}
}
