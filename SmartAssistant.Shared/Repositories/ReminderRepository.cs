using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApplication.Data;

namespace SmartAssistant.Shared.Repositories
{
	public class ReminderRepository : IReminderRepository
	{
		public readonly ApplicationDbContext _context;
		public ReminderRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public void Add(ReminderModel entity)
		{
			_context.Reminders.Add(entity);
		}

		public void Delete(ReminderModel entity)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ReminderModel> GetAll()
		{
			throw new NotImplementedException();
		}

		public ReminderModel GetById(int id)
		{
			throw new NotImplementedException();
		}

		public Task<List<ReminderModel>> GetRemindersByUserId(string userId)
		{
			throw new NotImplementedException();
		}

		public void Update(ReminderModel entity)
		{
			throw new NotImplementedException();
		}

		public Task UpdateReminderStatus(int reminderId, bool status)
		{
			throw new NotImplementedException();
		}
	}
}
