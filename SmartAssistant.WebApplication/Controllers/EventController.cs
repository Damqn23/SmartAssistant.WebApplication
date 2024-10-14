using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Services;
using SmartAssistant.Shared.Services.Speech;
using System.Security.Claims;
using Chronic;

namespace SmartAssistant.WebApplication.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService eventService;
        private readonly IMapper mapper;
        private readonly ITaskService taskService;
        private readonly GoogleSpeechService googleSpeechService;

        public EventController(IEventService _eventService, IMapper _mapper, ITaskService _taskService, GoogleSpeechService _googleSpeechService)
        {
            eventService = _eventService;
            taskService = _taskService;
            mapper = _mapper;
            googleSpeechService = _googleSpeechService;
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

        [HttpPost]
        public async Task<IActionResult> VoiceInput(IFormFile audioFile, string field)
        {
            try
            {
                // Validate field value
                if (string.IsNullOrEmpty(field))
                {
                    return Json(new { error = "Field value is missing. Please try again." });
                }

                // Validate audio file
                if (audioFile == null || audioFile.Length == 0)
                {
                    return Json(new { error = "Audio file is missing. Please record again." });
                }

                // Convert audio file to byte array
                byte[] audioBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await audioFile.CopyToAsync(memoryStream);
                    audioBytes = memoryStream.ToArray();
                }

                // Send audio to Google Speech API
                var recognizedText = googleSpeechService.RecognizeSpeech(audioBytes);

                // Validate recognized text
                if (string.IsNullOrEmpty(recognizedText))
                {
                    return Json(new { error = "Speech recognition failed. Please try again." });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { error = "User not found. Please log in and try again." });
                }

                // Handle the recognized text based on the current step
                switch (field.ToLower())
                {
                    case "eventtitle":
                        TempData["EventTitle"] = recognizedText;
                        return Json(new { nextStep = "eventdate", message = "Please say the event date and time" });

                    case "eventdate":
                        string normalizedText = recognizedText.ToLower().Trim();

                        DateTime eventDate;
                        var parser = new Parser();
                        var parsedDate = parser.Parse(normalizedText);

                        if (parsedDate != null && parsedDate.Start != null)
                        {
                            eventDate = parsedDate.Start.Value;
                        }
                        else
                        {
                            return Json(new { error = "Invalid date format. Please try again with a valid date." });
                        }

                        TempData["EventDate"] = eventDate;

                        // Now create the event
                        var eventCreateModel = new EventCreateModel
                        {
                            EventTitle = TempData["EventTitle"] as string,
                            EventDate = (DateTime)TempData["EventDate"]
                        };

                        await eventService.AddEventAsync(eventCreateModel, userId);

                        return Json(new { success = true, message = "Event created successfully." });

                    default:
                        return Json(new { error = "Unknown step. Please try again." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
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

            var events = await eventService.GetEventsByUserIdAsync(userId);
            var tasks = await taskService.GetTasksByUserIdAsync(userId);

            var calendarEvents = new List<object>();

            foreach (var evt in events)
            {
                calendarEvents.Add(new
                {
                    title = evt.EventTitle,
                    start = evt.EventDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    color = "blue"
                });
            }

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