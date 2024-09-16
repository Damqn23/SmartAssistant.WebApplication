using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class ActivityLog
    {
        public int Id { get; set; } // Unique identifier


        [Comment("Action description")]
        public string ActionDescription { get; set; } // Action description


        [Comment("Action date")]
        public DateTime ActionDate { get; set; } // Action date

        [ForeignKey("User")]
        [Comment("User foreign key")]
        public int UserId { get; set; } // User foreign key

        public User User { get; set; }  // Navigation property
    }
}
