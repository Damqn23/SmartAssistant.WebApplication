using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Data.Entities;
using SmartAssistant.WebApplication.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartAssistant.WebApplication.Utilities.Constants;


namespace SmartAssistant.WebApp.Data.Entities
{
    public class Team
    {
        public int Id { get; set; } // Unique identifier

        [Required(ErrorMessage = "Team name is required")]
        [MaxLength(TeamNameMaxLength)]  // Adjust length based on your constant
        public string TeamName { get; set; } // Team name

        [ForeignKey("User")]
        public string OwnerId { get; set; } // The creator/owner of the team
        public User Owner { get; set; } // Navigation property for owner

        // Use UserTeam for many-to-many relationship
        public ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<TeamDocument> TeamDocuments { get; set; }
    }
}
