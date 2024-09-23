using SmartAssistant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces
{
    public interface IReminderRepository : IRepository<ReminderModel>
    {
        Task<List<ReminderModel>> GetRemindersByUserIdAsync(string userId);

        Task UpdateReminderStatusAsync(int reminderId, bool status);

        Task<List<ReminderModel>> GetRemindersDueSoonAsync();
    }
}
