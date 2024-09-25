using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartAssistant.WebApplication.Controllers
{
    [Authorize]
    public class ReminderController : Controller
    {
        private readonly IReminderService reminderService;
        private readonly IMapper mapper;
        private readonly IHubContext<NotificationHub> hubContext;

        public ReminderController(IReminderService _reminderService, IMapper _mapper, IHubContext<NotificationHub> _hubContext)
        {
            reminderService = _reminderService;
            mapper = _mapper;
            hubContext = _hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var reminderModel = await reminderService.AddReminderAsync(reminder, userId);

                await hubContext.Clients.User(userId).SendAsync("ReceiveReminderNotification",
                    $"Reminder: {reminderModel.ReminderMessage} is set for {reminderModel.ReminderDate}");

                return RedirectToAction(nameof(Index));
            }

            return View(reminder);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var reminder = await reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }

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
                var existingReminder = await reminderService.GetReminderByIdAsync(id);
                if (existingReminder == null)
                {
                    return NotFound();
                }

                reminder.UserId = existingReminder.UserId;
                await reminderService.UpdateReminderAsync(reminder);
                return RedirectToAction(nameof(Index));
            }

            return View(editModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var reminder = await reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }

            var deleteModel = mapper.Map<ReminderDeleteModel>(reminder);
            return View(deleteModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await reminderService.DeleteReminderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReminderStatus(int reminderId, bool status)
        {
            await reminderService.UpdateReminderStatusAsync(reminderId, status);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetUpcomingReminders()
        {
            await reminderService.GetRemindersDueSoonAsync(1); 
            return Ok(); 
        }




    }
}
