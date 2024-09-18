using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Note
    {
		public int Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Note content is required")]
		[Comment("Note content")]
		public string NoteContent { get; set; } // Note content

		[Required(ErrorMessage = "Creation date is required")]
		[Comment("Creation date")]
		public DateTime CreatedDate { get; set; } // Creation date

		[ForeignKey("User")]
		[Required(ErrorMessage = "User foreign key is required")]
		[Comment("User foreign key")]
		public string UserId { get; set; } // User foreign key

		public User User { get; set; } // Navigation property
	}
}
