using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Category
    {
        public int Id { get; set; } // Unique identifier


        [Comment("Category name")]
        public string CategoryName { get; set; } // Category name

        // Navigation property
        public ICollection<Task> Tasks { get; set; } = new List<Task>(); // Category tasks
    }
}
