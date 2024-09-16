using SmartAssistant.WebApp.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace SmartAssistant.WebApplication.Data.Entities
{
    public class UserTeam
    {
        [Required]
        public int UserId { get; set; } // User foreign key

        [Required]
        public int TeamId { get; set; } // Team foreign key

        public User User { get; set; }  // Navigation property
        public Team Team { get; set; }  // Navigation property
    }
}
