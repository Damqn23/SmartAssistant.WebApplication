using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Preference
    {
		public int Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Preference key is required")]
		[Comment("Preference key")]
		public string PreferenceKey { get; set; } // Preference key

		[Required(ErrorMessage = "Preference value is required")]
		[Comment("Preference value")]
		public string PreferenceValue { get; set; } // Preference value

		[ForeignKey("User")]
		[Required(ErrorMessage = "User foreign key is required")]
		[Comment("User foreign key")]
		public string UserId { get; set; } // User foreign key

		public User User { get; set; } // Navigation property
	}
}
