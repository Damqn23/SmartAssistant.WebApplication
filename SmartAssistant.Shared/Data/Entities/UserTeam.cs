using SmartAssistant.WebApp.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApplication.Data.Entities
{
    public class UserTeam
    {
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }  // User foreign key
        public User User { get; set; }

        [Required]
        [ForeignKey("Team")]
        public int TeamId { get; set; }  // Team foreign key
        public Team Team { get; set; }
    }
}
