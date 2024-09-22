using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models;
using System.Security.Claims;

namespace SmartAssistant.WebApplication.Controllers
{
	[Authorize]
	public class ReminderController : Controller
	{
		private readonly IReminderService reminderService;
        private readonly IMapper mapper;

		public ReminderController(IReminderService _reminderService, IMapper _mapper)
		{
			reminderService = _reminderService;
			mapper = _mapper;
		}
		public async Task<IActionResult> Index()
		{
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
			var reminders = await reminderService.GetRemindersByUserIdAsync(userId);
			return View(reminders);
		}

		public async Task<IActionResult> Details(int id)
		{
			var reminder = await reminderService.GetReminderByIdAsync(id);
			if (reminder == null)
			{
				return NotFound();
			}
			return View(reminder);
		}
		// GET: Reminder/Create
		public IActionResult Create()
		{
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReminderCreateModel reminder)
        {
            if (ModelState.IsValid)
            {
                // Map ReminderCreateModel to ReminderModel
                var reminderModel = mapper.Map<ReminderModel>(reminder);

                // Set the UserId from the logged-in user
                reminderModel.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                await reminderService.AddReminderAsync(reminderModel);
                return RedirectToAction(nameof(Index));
            }

            return View(reminder);
        }


        // GET: Reminder/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var reminder = await reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }

            // Map the ReminderModel to ReminderEditModel
            var editModel = mapper.Map<ReminderEditModel>(reminder);

            return View(editModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReminderEditModel editModel)
        {
            if (id != editModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var reminder = mapper.Map<ReminderModel>(editModel);

                // Retrieve the original reminder from the database to preserve the UserId
                var existingReminder = await reminderService.GetReminderByIdAsync(id);
                if (existingReminder == null)
                {
                    return NotFound();
                }

                // Ensure the UserId is preserved
                reminder.UserId = existingReminder.UserId;

                await reminderService.UpdateReminderAsync(reminder);
                return RedirectToAction(nameof(Index));
            }

            return View(editModel);
        }


        // GET: Reminder/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var reminder = await reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }

            // Map the ReminderModel to ReminderDeleteModel
            var deleteModel = mapper.Map<ReminderDeleteModel>(reminder);

            return View(deleteModel);
        }


        // POST: Reminder/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await reminderService.DeleteReminderAsync(id);
            return RedirectToAction(nameof(Index));
        }


        // POST: UpdateReminderStatus
        [HttpPost]
		public async Task<IActionResult> UpdateReminderStatus(int reminderId, bool status)
		{
			await reminderService.UpdateReminderStatusAsync(reminderId, status);
			return RedirectToAction(nameof(Index));
		}
	}
}
