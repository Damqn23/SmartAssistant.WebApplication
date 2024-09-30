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
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ContentMaxLength)]  // Limit the content length to 1000 characters (for example)
        public string Content { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }  // AspNetUsers

        [Required]
        [ForeignKey("Team")]
        public int TeamId { get; set; }  // Teams

        [Required]
        public DateTime SentAt { get; set; }

        
        public virtual Team Team { get; set; }
        public virtual User User { get; set; }
    }
}
