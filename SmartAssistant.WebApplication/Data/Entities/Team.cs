using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Team
    {
        public int Id { get; set; } // Unique identifier


        [Comment("Team name")]
        public string TeamName { get; set; } // Team name

        // Navigation property
        public ICollection<User> Members { get; set; } = new List<User>(); // Team members
    }
}
