using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;

namespace SmartAssistant.Shared.Models
{
    public class UserModel
	{
        public string Id { get; set; }          
        public string UserName { get; set; }          
        public string Email { get; set; }  
        public List<TeamModel> Teams { get; set; } = new List<TeamModel>();          
        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();          
        public List<ReminderModel> Reminders { get; set; } = new List<ReminderModel>();      }
}