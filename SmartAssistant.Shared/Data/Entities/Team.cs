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
        public int Id { get; set; } 
        [Required(ErrorMessage = "Team name is required")]
        [MaxLength(TeamNameMaxLength)]          public string TeamName { get; set; } 
        [ForeignKey("User")]
        public string OwnerId { get; set; }         public User Owner { get; set; } 
        public ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();

        public virtual ICollection<Message> Messages { get; set; }

    }
}
