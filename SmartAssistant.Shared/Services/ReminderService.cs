using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services
{
	public class ReminderService : IReminderService
	{
		private readonly IReminderRepository reminderRepository;

		public ReminderService(IReminderRepository _reminderRepository)
		{
			reminderRepository = _reminderRepository;
		}
		public async Task AddReminderAsync(ReminderModel reminder)
		{
			await reminderRepository.AddAsync(reminder);
		}

        public Task AddReminderAsync(ReminderCreateModel reminder)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteReminderAsync(int id)
		{
			var reminder = await reminderRepository.GetByIdAsync(id);
			if(reminder != null)
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
	}
}
