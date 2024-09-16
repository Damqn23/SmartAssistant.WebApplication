using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Task
    {
		public int Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Task description is required")]
		public string Description { get; set; } // Task description

		public bool IsCompleted { get; set; } // Flag to indicate task completion

		[Required]
		public DateTime DueDate { get; set; } // Due date for the task

		// Constants
		public const string DefaultDescription = "New Task";
		public const bool DefaultCompletionStatus = false;
	}
}
