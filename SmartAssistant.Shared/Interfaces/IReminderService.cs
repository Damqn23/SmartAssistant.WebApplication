using SmartAssistant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces
{
	public interface IReminderService
	{
		Task<IEnumerable<ReminderModel>> GetAllRemindersAsync();
		Task<ReminderModel> GetReminderByIdAsync(int id);
        Task<ReminderModel> AddReminderAsync(ReminderCreateModel reminder, string userId);
        Task UpdateReminderAsync(ReminderModel reminder);
		Task DeleteReminderAsync(int id);
		Task UpdateReminderStatusAsync(int reminderId, bool status);
		Task<List<ReminderModel>> GetRemindersByUserIdAsync(string userId);
        Task<List<ReminderModel>> GetUpcomingRemindersAsync();

    }
}
