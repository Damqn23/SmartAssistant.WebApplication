using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Services.Speech;
using System.Security.Claims;
using Chronic;

namespace SmartAssistant.WebApplication.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IMapper mapper;
        private readonly GoogleSpeechService googleSpeechService;

        public TaskController(ITaskService _taskService, IMapper _mapper, GoogleSpeechService _googleSpeechService)
        {
            taskService = _taskService;
            mapper = _mapper;
            googleSpeechService = _googleSpeechService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetTasksByUserIdAsync(userId);
            return View(tasks);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateModel taskCreateModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await taskService.AddTaskAsync(taskCreateModel, userId);

                return RedirectToAction(nameof(Index));
            }

            return View(taskCreateModel);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var task = await taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var editModel = mapper.Map<TaskEditModel>(task);

            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskEditModel editModel)
        {
            if (ModelState.IsValid)
            {
                await taskService.UpdateTaskAsync(editModel);
                return RedirectToAction(nameof(Index));
            }
            return View(editModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var task = await taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var deleteModel = mapper.Map<TaskDeleteModel>(task);

            return View(deleteModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await taskService.DeleteTaskAsync(id);
            return RedirectToAction(nameof(Index));
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
                    case "description":
                        TempData["Description"] = recognizedText;
                        return Json(new { nextStep = "duedate", message = "Please say the due date" });

                    case "duedate":
                        string normalizedText = recognizedText.ToLower().Trim();

                        DateTime dueDate;
                        var parser = new Parser();
                        var parsedDate = parser.Parse(normalizedText);

                        if (parsedDate != null && parsedDate.Start != null)
                        {
                            dueDate = parsedDate.Start.Value;
                        }
                        else
                        {
                            return Json(new { error = "Invalid date format. Please try again with a valid date." });
                        }

                        TempData["DueDate"] = dueDate;
                        return Json(new { nextStep = "estimatedtime", message = "Please say the estimated time to complete" });

                    case "estimatedtime":
                        if (int.TryParse(recognizedText, out int estimatedTime))
                        {
                            TempData["EstimatedTime"] = estimatedTime;
                            return Json(new { nextStep = "priority", message = "Please say the priority level (Low, Medium, High)" });
                        }
                        else
                        {
                            return Json(new { error = "Invalid estimated time. Please provide a number in hours." });
                        }

                    case "priority":
                        normalizedText = recognizedText.ToLower().Trim();

                        // Handle variants of "High", "Medium", and "Low"
                        if (normalizedText == "high" || normalizedText.Contains("hi"))
                        {
                            TempData["Priority"] = PriorityLevel.High;
                        }
                        else if (normalizedText == "medium" || normalizedText.Contains("med"))
                        {
                            TempData["Priority"] = PriorityLevel.Medium;
                        }
                        else if (normalizedText == "low" || normalizedText.Contains("lo"))
                        {
                            TempData["Priority"] = PriorityLevel.Low;
                        }
                        else
                        {
                            return Json(new { error = "Invalid priority level. Please say either Low, Medium, or High." });
                        }

                        // Now create the task
                        var taskCreateModel = new TaskCreateModel
                        {
                            Description = TempData["Description"] as string,
                            DueDate = (DateTime)TempData["DueDate"],
                            EstimatedTimeToComplete = (int)TempData["EstimatedTime"],
                            Priority = (PriorityLevel)TempData["Priority"]
                        };

                        await taskService.AddTaskAsync(taskCreateModel, userId);

                        return Json(new { success = true, message = "Task created successfully." });

                    default:
                        return Json(new { error = "Unknown step. Please try again." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
        }

    }
}