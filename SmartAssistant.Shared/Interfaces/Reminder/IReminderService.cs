using SmartAssistant.Shared.Models.Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Reminder
{
    public interface IReminderService
    {
        Task<IEnumerable<ReminderModel>> GetAllRemindersAsync();
        Task<ReminderModel> GetReminderByIdAsync(int id);
        Task<ReminderModel> AddReminderAsync(ReminderCreateModel reminder, string userId);
        System.Threading.Tasks.Task UpdateReminderAsync(ReminderModel reminder);
        System.Threading.Tasks.Task DeleteReminderAsync(int id);
        System.Threading.Tasks.Task UpdateReminderStatusAsync(int reminderId, bool status);
        Task<List<ReminderModel>> GetRemindersByUserIdAsync(string userId);

        Task<List<ReminderModel>> GetRemindersDueSoonAsync(int minutes);

        System.Threading.Tasks.Task RemoveExpiredRemindersAsync();

    }
}
