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
		public DateTime DueDate { get; set; } // Due date for the tas
	}
}
