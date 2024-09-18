namespace SmartAssistant.Shared.Models
{
	public class UserModel
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public DateTime RegistrationDate { get; set; }

		public ICollection<ReminderModel> Reminders { get; set; }
	}
}