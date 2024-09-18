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
        Task<List<ReminderModel>> GetRemindersByUserId(string userId);

        Task UpdateReminderStatus(int reminderId, bool status);
    }
}
