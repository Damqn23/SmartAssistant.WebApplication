using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Preference
    {
        public int Id { get; set; } // Unique identifier


        [Comment("Preference key")]
        public string PreferenceKey { get; set; } // Preference key


        [Comment("Preference value")]
        public string PreferenceValue { get; set; } // Preference value

        [ForeignKey("User")]
        [Comment("User foreign key")]
        public int UserId { get; set; } // User foreign key

        public User User { get; set; }  // Navigation property
    }
}
