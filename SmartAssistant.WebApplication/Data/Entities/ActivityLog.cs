using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class ActivityLog
    {
		public string Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Action description is required")]
		[Comment("Action description")]
		public string ActionDescription { get; set; } // Action description

		[Comment("Action date")]
		public DateTime ActionDate { get; set; } // Action date

		[ForeignKey("User")]
		[Comment("User foreign key")]
		public string UserId { get; set; } // User foreign key

		public User User { get; set; } // Navigation property
	}
}
