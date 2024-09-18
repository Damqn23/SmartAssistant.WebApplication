using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartAssistant.WebApplication.Utilities.Constants;


namespace SmartAssistant.WebApp.Data.Entities
{
    public class Team
    {
		public int Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Team name is required")]
		[MaxLength(MaxTeamNameLength)]
		public string TeamName { get; set; } // Team name

		// Navigation property
		public ICollection<User> Members { get; set; } = new List<User>(); // Team members
	}
}
