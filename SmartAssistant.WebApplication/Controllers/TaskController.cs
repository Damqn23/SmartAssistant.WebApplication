﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Services.Speech;
using System.Security.Claims;
using Chronic;
using SmartAssistant.Shared.Interfaces.Speech;

namespace SmartAssistant.WebApplication.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IMapper mapper;
        private readonly IGoogleSpeechService googleSpeechService;
        private readonly SpeechTextExtractionService extractionService;

        public TaskController(ITaskService _taskService, IMapper _mapper, IGoogleSpeechService _googleSpeechService, SpeechTextExtractionService _extractionService)
        {
            taskService = _taskService;
            mapper = _mapper;
            googleSpeechService = _googleSpeechService;
            extractionService = _extractionService;
        }
        public async Task<IActionResult> Index(string sortOrder)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetTasksByUserIdAsync(userId);

            switch (sortOrder)
            {
                case "priority_asc":
                    tasks = tasks.OrderBy(t => t.Priority).ToList();
                    break;
                case "priority_desc":
                    tasks = tasks.OrderByDescending(t => t.Priority).ToList();
                    break;
                default:
                    tasks = tasks.OrderBy(t => t.DueDate).ToList(); 
                    break;
            }

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
        public async Task<IActionResult> VoiceInput(IFormFile audioFile)
        {
            try
            {
                if (audioFile == null || audioFile.Length == 0)
                {
                    return Json(new { error = "Audio file is missing. Please record again." });
                }

                byte[] audioBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await audioFile.CopyToAsync(memoryStream);
                    audioBytes = memoryStream.ToArray();
                }

                var recognizedText = googleSpeechService.RecognizeSpeech(audioBytes);

                if (string.IsNullOrEmpty(recognizedText))
                {
                    return Json(new { error = "Speech recognition failed. Please try again." });
                }

                string title = extractionService.ExtractTitle(recognizedText); 
                DateTime? dueDate = extractionService.ExtractDate(recognizedText);
                int estimatedTime = (int)extractionService.ExtractEstimatedTime(recognizedText);
                PriorityLevel priority = extractionService.ExtractPriority(recognizedText);

                if (string.IsNullOrEmpty(title) || !dueDate.HasValue)
                {
                    return Json(new { error = "Failed to recognize necessary fields. Please try again." });
                }

                var createModel = new TaskCreateModel
                {
                    Description = title,
                    DueDate = dueDate.Value,
                    EstimatedTimeToComplete = estimatedTime,
                    Priority = priority
                };

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await taskService.AddTaskAsync(createModel, userId);

                return Json(new { success = true, message = "Task created successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
        }


    }
}