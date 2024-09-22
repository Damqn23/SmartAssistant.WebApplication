using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApp.Data.Entities;
using SmartAssistant.WebApplication.Data;

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


        public async System.Threading.Tasks.Task<IEnumerable<ReminderModel>> GetAllAsync()
		{
			var reminder = await context.Reminders.ToListAsync();
			return mapper.Map<List<ReminderModel>>(reminder);
		}

        public async System.Threading.Tasks.Task<ReminderModel> GetByIdAsync(int id)
        {
            var reminder = await context.Reminders.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            return mapper.Map<ReminderModel>(reminder);
        }


        public async System.Threading.Tasks.Task<List<ReminderModel>> GetRemindersByUserIdAsync(string userId)
		{
			var reminder = await context.Reminders.Where(x => x.UserId == userId).ToListAsync();
			return mapper.Map<List<ReminderModel>>(reminder);
		}

		public async System.Threading.Tasks.Task UpdateAsync(ReminderModel entity)
		{
			var reminder = mapper.Map<Reminder>(entity);
			context.Reminders.Update(reminder);
			await context.SaveChangesAsync();
		}

		public async System.Threading.Tasks.Task UpdateReminderStatusAsync(int reminderId, bool status)
		{
			var reminder = await context.Reminders.FindAsync(reminderId);
			if(reminder != null)
			{
				reminder.ReminderDate = status ? DateTime.Now : reminder.ReminderDate;
				await context.SaveChangesAsync();
			}
		}
	}
}
