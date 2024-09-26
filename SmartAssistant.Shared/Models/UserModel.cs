using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;

namespace SmartAssistant.Shared.Models
{
    public class UserModel
	{
        public string Id { get; set; }  // User ID (Primary Key)
        public string UserName { get; set; }  // User's username
        public string Email { get; set; }  // User's email address

        // Additional fields as needed
        public List<TeamModel> Teams { get; set; } = new List<TeamModel>();  // Associated Teams
        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();  // Associated Tasks
        public List<ReminderModel> Reminders { get; set; } = new List<ReminderModel>();  // Associated Reminders
    }
}