using SmartAssistant.WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartAssistant.WebApplication.Utilities.Constants;

namespace SmartAssistant.Shared.Data.Entities
{
    public class TeamDocument
    {
        // Constants for maximum lengths (if needed)

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(FileNameMaxLength)]  // Limit the file name length to 255 characters (for example)
        public string FileName { get; set; }

        [Required]
        public string DocumentPath { get; set; }

        [Required]
        [ForeignKey("Team")]
        public int TeamId { get; set; }  // Teams

        [Required]
        [ForeignKey("User")]
        public string UploadedBy { get; set; }  // AspNetUsers

        [Required]
        public DateTime UploadedAt { get; set; }

        // Navigation properties
        public virtual Team Team { get; set; }
        public virtual User User { get; set; }
    }
}
