using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Services;
using SmartAssistant.Shared.Services.Speech;
using System.Security.Claims;
using Chronic;
using SmartAssistant.Shared.Interfaces.Speech;

namespace SmartAssistant.WebApplication.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService eventService;
        private readonly IMapper mapper;
        private readonly ITaskService taskService;
        private readonly IGoogleSpeechService googleSpeechService;
        private readonly SpeechTextExtractionService extractionService;

        public EventController(IEventService _eventService, IMapper _mapper, ITaskService _taskService, IGoogleSpeechService _googleSpeechService, SpeechTextExtractionService _extractionService)
        {
            eventService = _eventService;
            taskService = _taskService;
            mapper = _mapper;
            googleSpeechService = _googleSpeechService;
            extractionService = _extractionService;
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
        public async Task<IActionResult> VoiceInput(IFormFile audioFile)
        {
            try
            {
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

                // Use the extraction service to get event details
                string eventTitle = extractionService.ExtractTitle(recognizedText); // Extract the event title
                DateTime? eventDate = extractionService.ExtractDate(recognizedText); // Extract the event date and time

                // Handle missing fields
                if (string.IsNullOrEmpty(eventTitle) || !eventDate.HasValue)
                {
                    return Json(new { error = "Failed to recognize necessary fields. Please try again." });
                }

                // Create the event
                var eventCreateModel = new EventCreateModel
                {
                    EventTitle = eventTitle,
                    EventDate = eventDate.Value
                };

                await eventService.AddEventAsync(eventCreateModel, userId);

                return Json(new { success = true, message = "Event created successfully." });
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