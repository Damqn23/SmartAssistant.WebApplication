using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartAssistant.WebApplication.Utilities.Constants;


namespace SmartAssistant.WebApp.Data.Entities
{
    public class Task
    {
        public int Id { get; set; } 
        [Required(ErrorMessage = "Task description is required")]
        [MaxLength(DescriptionMaxLength, ErrorMessage = "Task description is too long")]
        public string Description { get; set; } 
        [DefaultValue(DefaultCompletionStatus)]
        public bool IsCompleted { get; set; } 
        [Required]
        public DateTime DueDate { get; set; } 
        [Range(1, 1440, ErrorMessage = "Time must be between 1 minute and 24 hours.")]
        public int EstimatedTimeToComplete { get; set; } 
        [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5.")]
        public int Priority { get; set; } 
        [ForeignKey("User")]
        public string UserId { get; set; } 
        public User User { get; set; } 
        [ForeignKey("Team")]
        public int? TeamId { get; set; } 
        public Team Team { get; set; }     }

}
