using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Services;
using System.Security.Claims;

namespace SmartAssistant.WebApplication.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService eventService;
        private readonly IMapper mapper;
        private readonly ITaskService taskService;

        public EventController(IEventService _eventService, IMapper _mapper, ITaskService _taskService)
        {
            eventService = _eventService;
            taskService = _taskService;
            mapper = _mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var events = await eventService.GetEventsByUserIdAsync(userId);
            return View(events);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateModel eventCreateModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await eventService.AddEventAsync(eventCreateModel, userId);
                return RedirectToAction(nameof(Index));
            }

            return View(eventCreateModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var eventModel = await eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            var editModel = mapper.Map<EventEditModel>(eventModel);
            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventEditModel editModel)
        {
            if (ModelState.IsValid)
            {
                await eventService.UpdateEventAsync(editModel);
                return RedirectToAction(nameof(Index));
            }

            return View(editModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var eventModel = await eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await eventService.DeleteEventAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Get all events and tasks for the logged-in user
            var events = await eventService.GetEventsByUserIdAsync(userId);
            var tasks = await taskService.GetTasksByUserIdAsync(userId);

            // Prepare the data for FullCalendar
            var calendarEvents = new List<object>();

            // Add events to calendar data
            foreach (var evt in events)
            {
                calendarEvents.Add(new
                {
                    title = evt.EventTitle,
                    start = evt.EventDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    color = "blue"
                });
            }

            // Add tasks to calendar data
            foreach (var task in tasks)
            {
                calendarEvents.Add(new
                {
                    title = task.Description,
                    start = task.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    color = "green"
                });
            }

            return Json(calendarEvents);
        }

    }
}
