using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApplication.Data;

namespace SmartAssistant.Shared.Repositories
{
	public class ReminderRepository : IReminderRepository
	{
		public Task AddAsync(ReminderModel entity)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(ReminderModel entity)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ReminderModel>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<ReminderModel> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<List<ReminderModel>> GetRemindersByUserIdAsync(string userId)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(ReminderModel entity)
		{
			throw new NotImplementedException();
		}

		public Task UpdateReminderStatusAsync(int reminderId, bool status)
		{
			throw new NotImplementedException();
		}
	}
}
