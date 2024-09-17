using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Comment
    {
		public int Id { get; set; } // Unique identifier

		[Required(ErrorMessage = "Comment text is required")]
		[Comment("Comment text")]
		public string Text { get; set; } // Comment text

		[Comment("Comment date")]
		public DateTime CommentDate { get; set; } // Comment date

		[ForeignKey("Task")]
		[Comment("Task foreign key")]
		public int TaskId { get; set; } // Task foreign key

		public Task Task { get; set; } // Navigation property
	}
}
