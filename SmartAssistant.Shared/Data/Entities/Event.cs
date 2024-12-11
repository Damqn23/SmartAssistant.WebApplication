using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SmartAssistant.WebApplication.Utilities.Constants;


namespace SmartAssistant.WebApp.Data.Entities
{
    public class Event
    {
		public int Id { get; set; } 
		[Required(ErrorMessage = "Event title is required")]
		[MaxLength(EventTitleMaxLength, ErrorMessage = "Event title is too long")]
		[Comment("Event title")]
		public string EventTitle { get; set; } 
		[Comment("Event date")]
		public DateTime EventDate { get; set; } 
		[ForeignKey("User")]
		[Comment("User foreign key")]
		public string UserId { get; set; } 
		public User User { get; set; } 
        [ForeignKey("Team")]
        public int? TeamId { get; set; } 
        public Team Team { get; set; }     }
}
