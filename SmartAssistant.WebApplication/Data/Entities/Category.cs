using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static SmartAssistant.WebApplication.Utilities.Constants;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Category
    {
		public int Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Category name is required")]
		[Comment("Category name")]
		[MaxLength(CategoryNameMaxLength)]
		public string CategoryName { get; set; } // Category name

		// Navigation property
		public ICollection<Task> Tasks { get; set; } = new List<Task>(); // Category tasks
	}
}

