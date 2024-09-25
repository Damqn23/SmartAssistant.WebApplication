using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces;
using System.Security.Claims;
using SmartAssistant.Shared.Models.Calendar;

namespace SmartAssistant.WebApplication.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IEventService eventService;

        public CalendarController(ITaskService _taskService, IEventService _eventService)
        {
            taskService = _taskService;
            eventService = _eventService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetTasksByUserIdAsync(userId);
            var events = await eventService.GetEventsByUserIdAsync(userId);

            var currentMonth = DateTime.Now; // Or allow the user to select the month

            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month))
                .Select(day => new DateTime(currentMonth.Year, currentMonth.Month, day))
                .ToList();

            var days = daysInMonth.Select(date => new DayViewModel
            {
                Date = date,
                Tasks = tasks.Where(t => t.DueDate.Date == date.Date).ToList(),
                Events = events.Where(e => e.EventDate.Date == date.Date).ToList()
            }).ToList();

            var viewModel = new CalendarViewModel
            {
                CurrentMonth = currentMonth,
                Days = days
            };

            return View(viewModel);
        }

    }


}

