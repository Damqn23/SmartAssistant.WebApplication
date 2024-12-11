using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartAssistant.WebApplication.Utilities.Constants;


namespace SmartAssistant.WebApp.Data.Entities
{
    public class Task
    {
        public int Id { get; set; } // Unique identifier

        [Required(ErrorMessage = "Task description is required")]
        [MaxLength(DescriptionMaxLength, ErrorMessage = "Task description is too long")]
        public string Description { get; set; } // Task description

        [DefaultValue(DefaultCompletionStatus)]
        public bool IsCompleted { get; set; } // Flag to indicate task completion

        [Required]
        public DateTime DueDate { get; set; } // Due date for the task

        [Range(1, 1440, ErrorMessage = "Time must be between 1 minute and 24 hours.")]
        public int EstimatedTimeToComplete { get; set; } // Time required in minutes

        [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5.")]
        public int Priority { get; set; } // Task priority (1 = Low, 5 = High)

        [ForeignKey("User")]
        public string UserId { get; set; } // Foreign key to the User

        public User User { get; set; } // Navigation property to the User

        [ForeignKey("Team")]
        public int? TeamId { get; set; } // Foreign key to the Team

        public Team Team { get; set; } // Navigation property to the Team
    }

}
