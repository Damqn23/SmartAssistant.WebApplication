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
            // Arrange
            var taskCreateModel = new TaskCreateModel
            {
                Description = "Complete the project",
                DueDate = DateTime.Now.AddDays(1),
                EstimatedTimeToComplete = 3,
                Priority = PriorityLevel.High
            };

            var userId = "user123";

            // Act
            await _taskService.AddTaskAsync(taskCreateModel, userId);

            // Assert
            _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<TaskModel>()), Times.Once);
            _mockReminderService.Verify(r => r.AddReminderAsync(It.IsAny<ReminderCreateModel>(), userId), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldDeleteTask_WhenTaskExists()
        {
            // Arrange
            var taskId = 1;
            var task = new TaskModel { Id = taskId };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            await _taskService.DeleteTaskAsync(taskId);

            // Assert
            _mockTaskRepository.Verify(r => r.DeleteAsync(task), Times.Once);
        }

        [Fact]
        public async Task GetAllTasksAsync_ShouldReturnAllTasks()
        {
            // Arrange
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Description = "Task 1" },
                new TaskModel { Id = 2, Description = "Task 2" }
            };

            _mockTaskRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksAsync();

            // Assert
            result.Should().BeEquivalentTo(tasks);
            _mockTaskRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateTask_WhenTaskExists()
        {
            // Arrange
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

            // Act
            await _taskService.UpdateTaskAsync(taskEditModel);

            // Assert
            _mockTaskRepository.Verify(r => r.UpdateAsync(It.Is<TaskModel>(
                t => t.Id == taskEditModel.Id &&
                     t.Description == taskEditModel.Description &&
                     t.DueDate == taskEditModel.DueDate &&
                     t.UserId == existingTask.UserId)), Times.Once);
        }

        [Fact]
        public async Task RemoveExpiredTasksAsync_ShouldDeleteExpiredTasks()
        {
            // Arrange
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, DueDate = DateTime.Now.AddDays(-1) },
                new TaskModel { Id = 2, DueDate = DateTime.Now.AddDays(1) }
            };

            _mockTaskRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            await _taskService.RemoveExpiredTasksAsync();

            // Assert
            _mockTaskRepository.Verify(r => r.DeleteAsync(It.Is<TaskModel>(t => t.Id == 1)), Times.Once);
            _mockTaskRepository.Verify(r => r.DeleteAsync(It.Is<TaskModel>(t => t.Id == 2)), Times.Never);
        }

        [Fact]
        public async Task GetTasksBySearchQueryAsync_ShouldReturnMatchingTasks()
        {
            // Arrange
            var searchQuery = "project";
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Description = "Complete the project" },
                new TaskModel { Id = 2, Description = "Prepare for meeting" }
            };

            _mockTaskRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetTasksBySearchQueryAsync(searchQuery);

            // Assert
            result.Should().HaveCount(1);
            result[0].Description.Should().Contain(searchQuery, StringComparison.OrdinalIgnoreCase.ToString());
        }
    }
}
