using SmartAssistant.Shared.Models.Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Reminder
{
    public interface IReminderRepository : IRepository<ReminderModel>
    {
        Task<List<ReminderModel>> GetRemindersByUserIdAsync(string userId);

        System.Threading.Tasks.Task UpdateReminderStatusAsync(int reminderId, bool status);

        Task<List<ReminderModel>> GetRemindersDueSoonAsync(int minutes);


    }
}
