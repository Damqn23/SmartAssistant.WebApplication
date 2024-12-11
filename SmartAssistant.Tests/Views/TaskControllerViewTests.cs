using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartAssistant.Shared.Interfaces.Speech;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Services.Speech;
using SmartAssistant.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Views
{
    public class TaskControllerViewTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IGoogleSpeechService> _mockGoogleSpeechService;
        private readonly Mock<SpeechTextExtractionService> _mockExtractionService;
        private readonly TaskController _controller;

        public TaskControllerViewTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockMapper = new Mock<IMapper>();
            _mockGoogleSpeechService = new Mock<IGoogleSpeechService>();
            _mockExtractionService = new Mock<SpeechTextExtractionService>();

            _controller = new TaskController(
                _mockTaskService.Object,
                _mockMapper.Object,
                _mockGoogleSpeechService.Object,
                _mockExtractionService.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithTasks()
        {
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Description = "Task 1", DueDate = DateTime.Today, IsCompleted = false, Priority = 1 },
                new TaskModel { Id = 2, Description = "Task 2", DueDate = DateTime.Today, IsCompleted = true, Priority = 2 }
            };

            _mockTaskService.Setup(s => s.GetTasksByUserIdAsync(It.IsAny<string>())).ReturnsAsync(tasks);

            var result = await _controller.Index(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TaskModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_Get_ShouldReturnView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ShouldRedirectToIndexOnSuccess()
        {
            var taskModel = new TaskCreateModel
            {
                Description = "New Task",
                DueDate = DateTime.Today,
                EstimatedTimeToComplete = 2,
                Priority = PriorityLevel.High
            };

            _mockTaskService.Setup(s => s.AddTaskAsync(taskModel, It.IsAny<string>())).Returns(Task.CompletedTask);

            var result = await _controller.Create(taskModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ShouldReturnViewIfModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Description", "Required");

            var taskModel = new TaskCreateModel();

            var result = await _controller.Create(taskModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(taskModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_ShouldReturnViewWithEditModel()
        {
            var task = new TaskModel { Id = 1, Description = "Task 1", DueDate = DateTime.Today, Priority = (int)PriorityLevel.Medium };
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(task);

            var editModel = new TaskEditModel
            {
                Id = 1,
                Description = "Task 1",
                DueDate = DateTime.Today,
                Priority = PriorityLevel.Medium
            };

            _mockMapper.Setup(m => m.Map<TaskEditModel>(task)).Returns(editModel);

            var result = await _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TaskEditModel>(viewResult.Model);
            Assert.Equal(editModel.Id, model.Id);
        }

        [Fact]
        public async Task Edit_Post_ShouldRedirectToIndexOnSuccess()
        {
            var editModel = new TaskEditModel
            {
                Id = 1,
                Description = "Updated Task",
                DueDate = DateTime.Today,
                Priority = PriorityLevel.Low
            };

            _mockTaskService.Setup(s => s.UpdateTaskAsync(editModel)).Returns(Task.CompletedTask);

            var result = await _controller.Edit(editModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ShouldReturnViewIfModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Description", "Required");

            var editModel = new TaskEditModel();

            var result = await _controller.Edit(editModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editModel, viewResult.Model);
        }

        [Fact]
        public async Task Delete_Get_ShouldReturnViewWithDeleteModel()
        {
            var task = new TaskModel { Id = 1, Description = "Task 1", DueDate = DateTime.Today };
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(task);

            var deleteModel = new TaskDeleteModel
            {
                Id = 1,
                Description = "Task 1",
                DueDate = DateTime.Today
            };

            _mockMapper.Setup(m => m.Map<TaskDeleteModel>(task)).Returns(deleteModel);

            var result = await _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TaskDeleteModel>(viewResult.Model);
            Assert.Equal(deleteModel.Id, model.Id);
        }

        [Fact]
        public async Task Delete_Post_ShouldRedirectToIndexOnSuccess()
        {
            _mockTaskService.Setup(s => s.DeleteTaskAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
