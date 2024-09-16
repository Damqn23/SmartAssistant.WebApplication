using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Note
    {
        public int Id { get; set; } // Unique identifier


        [Comment("Note content")]
        public string NoteContent { get; set; } // Note content


        [Comment("Creation date")]
        public DateTime CreatedDate { get; set; } // Creation date

        [ForeignKey("User")]
        [Comment("User foreign key")]
        public int UserId { get; set; } // User foreign key

        public User User { get; set; }  // Navigation property
    }
}
