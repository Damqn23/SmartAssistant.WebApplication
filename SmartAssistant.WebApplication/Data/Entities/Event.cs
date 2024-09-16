using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Event
    {
        public int Id { get; set; } // Unique identifier


        [Comment("Event title")]
        public string EventTitle { get; set; } // Event title


        [Comment("Event date")]
        public DateTime EventDate { get; set; } // Event date

        [ForeignKey("User")]
        [Comment("User foreign key")]
        public int UserId { get; set; } // User foreign key

        public User User { get; set; }  // Navigation property
    }
}
