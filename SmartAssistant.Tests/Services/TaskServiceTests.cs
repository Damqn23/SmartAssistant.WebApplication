using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces.Reminder;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IReminderService> _mockReminderService;
        private readonly Mock<IHubContext<NotificationHub>> _mockHubContext;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockReminderService = new Mock<IReminderService>();
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();

            _taskService = new TaskService(_mockTaskRepository.Object, _mockReminderService.Object, _mockHubContext.Object);
        }

        [Fact]
        public async Task AddTaskAsync_ShouldAddTaskAndCreateReminder()
        {
            var taskCreateModel = new TaskCreateModel
            {
                Description = "Complete the project",
                DueDate = DateTime.Now.AddDays(1),
                EstimatedTimeToComplete = 3,
                Priority = PriorityLevel.High
            };

            var userId = "user123";

            await _taskService.AddTaskAsync(taskCreateModel, userId);

            _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<TaskModel>()), Times.Once);
            _mockReminderService.Verify(r => r.AddReminderAsync(It.IsAny<ReminderCreateModel>(), userId), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldDeleteTask_WhenTaskExists()
        {
            var taskId = 1;
            var task = new TaskModel { Id = taskId };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

            await _taskService.DeleteTaskAsync(taskId);

            _mockTaskRepository.Verify(r => r.DeleteAsync(task), Times.Once);
        }

        [Fact]
        public async Task GetAllTasksAsync_ShouldReturnAllTasks()
        {
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Description = "Task 1" },
                new TaskModel { Id = 2, Description = "Task 2" }
            };

            _mockTaskRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            var result = await _taskService.GetAllTasksAsync();

            result.Should().BeEquivalentTo(tasks);
            _mockTaskRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateTask_WhenTaskExists()
        {
            var taskEditModel = new TaskEditModel
            {
                Id = 1,
                Description = "Updated Task",
                DueDate = DateTime.Now.AddDays(2),
                EstimatedTimeToComplete = 4,
                Priority = PriorityLevel.Medium,
                IsCompleted = false
            };

            var existingTask = new TaskModel { Id = 1, UserId = "user123" };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskEditModel.Id)).ReturnsAsync(existingTask);

            await _taskService.UpdateTaskAsync(taskEditModel);

            _mockTaskRepository.Verify(r => r.UpdateAsync(It.Is<TaskModel>(
                t => t.Id == taskEditModel.Id &&
                     t.Description == taskEditModel.Description &&
                     t.DueDate == taskEditModel.DueDate &&
                     t.UserId == existingTask.UserId)), Times.Once);
        }

        [Fact]
        public async Task RemoveExpiredTasksAsync_ShouldDeleteExpiredTasks()
        {
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, DueDate = DateTime.Now.AddDays(-1) },
                new TaskModel { Id = 2, DueDate = DateTime.Now.AddDays(1) }
            };

            _mockTaskRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            await _taskService.RemoveExpiredTasksAsync();

            _mockTaskRepository.Verify(r => r.DeleteAsync(It.Is<TaskModel>(t => t.Id == 1)), Times.Once);
            _mockTaskRepository.Verify(r => r.DeleteAsync(It.Is<TaskModel>(t => t.Id == 2)), Times.Never);
        }

        [Fact]
        public async Task GetTasksBySearchQueryAsync_ShouldReturnMatchingTasks()
        {
            var searchQuery = "project";
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Description = "Complete the project" },
                new TaskModel { Id = 2, Description = "Prepare for meeting" }
            };

            _mockTaskRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            var result = await _taskService.GetTasksBySearchQueryAsync(searchQuery);

            result.Should().HaveCount(1);
            result[0].Description.Should().Contain(searchQuery, StringComparison.OrdinalIgnoreCase.ToString());
        }
    }
}
