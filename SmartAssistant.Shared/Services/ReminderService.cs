using Microsoft.AspNetCore.SignalR;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository reminderRepository;
        private readonly IHubContext<NotificationHub> hubContext;

        public ReminderService(IReminderRepository _reminderRepository, IHubContext<NotificationHub> _hubContext)
        {
            reminderRepository = _reminderRepository;
            hubContext = _hubContext;
        }

        public async Task<ReminderModel> AddReminderAsync(ReminderCreateModel model, string userId)
        {
            var reminder = new ReminderModel
            {
                ReminderMessage = model.ReminderMessage,
                ReminderDate = model.ReminderDate,
                UserId = userId
            };

            await reminderRepository.AddAsync(reminder);

            // Send notification
            await hubContext.Clients.User(reminder.UserId).SendAsync("ReceiveReminderNotification",
                $"Reminder: {reminder.ReminderMessage} is set for {reminder.ReminderDate}");

            return reminder;
        }

        public async Task DeleteReminderAsync(int id)
        {
            var reminder = await reminderRepository.GetByIdAsync(id);
            if (reminder != null)
            {
                await reminderRepository.DeleteAsync(reminder);
            }
        }

        public async Task<IEnumerable<ReminderModel>> GetAllRemindersAsync()
        {
            return await reminderRepository.GetAllAsync();
        }

        public async Task<ReminderModel> GetReminderByIdAsync(int id)
        {
            return await reminderRepository.GetByIdAsync(id);
        }

        public async Task<List<ReminderModel>> GetRemindersByUserIdAsync(string userId)
        {
            return await reminderRepository.GetRemindersByUserIdAsync(userId);
        }

        public async Task UpdateReminderAsync(ReminderModel reminder)
        {
            await reminderRepository.UpdateAsync(reminder);
        }

        public async Task UpdateReminderStatusAsync(int reminderId, bool status)
        {
            await reminderRepository.UpdateReminderStatusAsync(reminderId, status);
        }

        

        public async Task<List<ReminderModel>> GetRemindersDueSoonAsync(int minutes)
        {
            var currentTime = DateTime.Now;
            var upcomingReminders = await reminderRepository.GetRemindersDueSoonAsync(minutes);

            foreach (var reminder in upcomingReminders)
            {
                await hubContext.Clients.User(reminder.UserId).SendAsync("ReceiveReminderNotification", $"Reminder: {reminder.ReminderMessage} is coming up at {reminder.ReminderDate}");
            }

            return upcomingReminders;
        }



    }
}
