using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.WebApplication.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartAssistant.WebApplication.Utilities.Constants;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class User : IdentityUser
    {      

        // Navigation properties
        public ICollection<Task> Tasks { get; set; } = new List<Task>(); // User tasks
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>(); // User reminders

        public ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
    }
}
