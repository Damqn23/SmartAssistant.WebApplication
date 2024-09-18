using SmartAssistant.WebApp.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApplication.Data.Entities
{
    public class UserTeam
    {
		[Required]
        [ForeignKey("User")]
		public string UserForeignKey { get; set; } // User foreign key as string

		[Required]
		[ForeignKey("Team")]
		public int TeamForeignKey { get; set; } // Team foreign key

		public User User { get; set; } // Navigation property to User entity
		public Team Team { get; set; } // Navigation property to Team entity
	}
}
