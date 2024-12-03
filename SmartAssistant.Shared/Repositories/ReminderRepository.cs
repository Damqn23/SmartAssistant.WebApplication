using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Interfaces.Reminder;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.WebApp.Data.Entities;
using SmartAssistant.WebApplication.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ReminderRepository(ApplicationDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async System.Threading.Tasks.Task AddAsync(ReminderModel entity)
        {
            var reminder = mapper.Map<Reminder>(entity);
            context.Reminders.Add(reminder);
            await context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteAsync(ReminderModel entity)
        {
            var reminder = await context.Reminders.FindAsync(entity.Id);
            if (reminder != null)
            {
                context.Reminders.Remove(reminder);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ReminderModel>> GetAllAsync()
        {
            var reminders = await context.Reminders.ToListAsync();
            return mapper.Map<List<ReminderModel>>(reminders);
        }

        public async Task<ReminderModel> GetByIdAsync(int id)
        {
            var reminder = await context.Reminders.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            return mapper.Map<ReminderModel>(reminder);
        }

        public async Task<List<ReminderModel>> GetRemindersByUserIdAsync(string userId)
        {
            var reminders = await context.Reminders.Where(x => x.UserId == userId).ToListAsync();
            return mapper.Map<List<ReminderModel>>(reminders);
        }

        public async System.Threading.Tasks.Task UpdateAsync(ReminderModel entity)
        {
            var reminder = await context.Reminders.FirstOrDefaultAsync(r => r.Id == entity.Id);
            if (reminder != null)
            {
                // Update properties manually
                reminder.ReminderMessage = entity.ReminderMessage;
                reminder.ReminderDate = entity.ReminderDate;
                reminder.UserId = entity.UserId;

                // Save changes
                await context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task UpdateReminderStatusAsync(int reminderId, bool status)
        {
            var reminder = await context.Reminders.FindAsync(reminderId);
            if (reminder != null)
            {
                reminder.ReminderDate = status ? DateTime.Now : reminder.ReminderDate;
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ReminderModel>> GetRemindersDueSoonAsync(int minutes)
        {
            var currentTime = DateTime.Now;
            var upcomingReminders = await context.Reminders
                .Where(r => r.ReminderDate >= currentTime && r.ReminderDate <= currentTime.AddMinutes(minutes))
                .ToListAsync();

            return mapper.Map<List<ReminderModel>>(upcomingReminders);
        }
    }
}
