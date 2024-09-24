using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Task;
using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;
        private readonly IReminderService reminderService;
        IHubContext<NotificationHub> hubContext;

        public TaskService(ITaskRepository _taskRepository, IReminderService _reminderService, IHubContext<NotificationHub> _hubContext)
        {
            taskRepository = _taskRepository;
            reminderService = _reminderService;
            hubContext = _hubContext;
        }
        public async Task AddTaskAsync(TaskCreateModel taskCreateModel, string userId)
        {
            var task = new TaskModel
            {
                Description = taskCreateModel.Description,
                DueDate = taskCreateModel.DueDate,
                EstimatedTimeToComplete = taskCreateModel.EstimatedTimeToComplete,
                Priority = (int)taskCreateModel.Priority,
                IsCompleted = false,
                UserId = userId
            };

            await taskRepository.AddAsync(task);

            var reminderCreateModel = new ReminderCreateModel
            {
                ReminderMessage = $"Reminder for task: {task.Description}",
                ReminderDate = task.DueDate.AddHours(-1),
                 };

            await reminderService.AddReminderAsync(reminderCreateModel,userId);

            await hubContext.Clients.User(userId).SendAsync("ReceiveReminderNotification", $"Task '{task.Description}' is created, and a reminder is set for {reminderCreateModel.ReminderDate}");
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await taskRepository.GetByIdAsync(id);
            if(task != null)
            {
                await taskRepository.DeleteAsync(task);
            }
        }

        public async Task<IEnumerable<TaskModel>> GetAllTasksAsync()
        {
            return await taskRepository.GetAllAsync();
        }

        public async Task<TaskModel> GetTaskByIdAsync(int id)
        {
            return await taskRepository.GetByIdAsync(id);
        }

        public async Task<List<TaskModel>> GetTasksByUserIdAsync(string userId)
        {
            return await taskRepository.GetTasksByUserIdAsync(userId);
        }

        public async Task UpdateTaskAsync(TaskEditModel taskEditModel)
        {
            // Fetch the original task from the repository to preserve UserId
            var existingTask = await taskRepository.GetByIdAsync(taskEditModel.Id);

            if (existingTask == null)
            {
                throw new Exception("Task not found");
            }

            // Map the changes from TaskEditModel to TaskModel
            var task = new TaskModel
            {
                Id = taskEditModel.Id,
                Description = taskEditModel.Description,
                DueDate = taskEditModel.DueDate,
                EstimatedTimeToComplete = taskEditModel.EstimatedTimeToComplete,
                Priority = (int)taskEditModel.Priority,
                IsCompleted = taskEditModel.IsCompleted,
                UserId = existingTask.UserId // Preserve the original UserId
            };

            await taskRepository.UpdateAsync(task);
        }

    }
}
