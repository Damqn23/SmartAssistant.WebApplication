using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Attachment
    {
        public int Id { get; set; } // Unique identifier


        [Comment("File name")]
        public string FileName { get; set; } // File name


        [Comment("File data")]
        public byte[] FileData { get; set; } // File data

        [ForeignKey("Task")]
        [Comment("Task foreign key")]
        public int TaskId { get; set; } // Task foreign key

        public Task Task { get; set; }  // Navigation property
    }
}
