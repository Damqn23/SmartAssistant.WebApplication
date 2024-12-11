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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IGoogleSpeechService> _mockGoogleSpeechService;
        private readonly SpeechTextExtractionService _realExtractionService;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockMapper = new Mock<IMapper>();
            _mockGoogleSpeechService = new Mock<IGoogleSpeechService>();

            _realExtractionService = new SpeechTextExtractionService();

            _controller = new TaskController(
                _mockTaskService.Object,
                _mockMapper.Object,
                _mockGoogleSpeechService.Object,
                _realExtractionService
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithTasksSortedByPriorityAscending()
        {
            var tasks = new List<TaskModel>
            {
                new TaskModel { Priority = 2, DueDate = DateTime.Now },
                new TaskModel { Priority = 1, DueDate = DateTime.Now.AddDays(1) }
            };
            _mockTaskService.Setup(s => s.GetTasksByUserIdAsync("test-user-id"))
                .ReturnsAsync(tasks);

            var result = await _controller.Index("priority_asc");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<TaskModel>>(viewResult.Model);
            Assert.Equal(1, model.First().Priority); // Sorted by priority ascending
        }

        [Fact]
        public async Task Create_ShouldRedirectToIndexOnSuccess()
        {
            var taskCreateModel = new TaskCreateModel { Description = "Test Task" };

            var result = await _controller.Create(taskCreateModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_ShouldReturnViewOnInvalidModelState()
        {
            _controller.ModelState.AddModelError("Description", "Required");
            var taskCreateModel = new TaskCreateModel();

            var result = await _controller.Create(taskCreateModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(taskCreateModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_ShouldReturnViewWithEditModel()
        {
            var task = new TaskModel { Id = 1, Description = "Test Task" };
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(task);

            var editModel = new TaskEditModel { Id = 1, Description = "Test Task" };
            _mockMapper.Setup(m => m.Map<TaskEditModel>(task)).Returns(editModel);

            var result = await _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Post_ShouldRedirectToIndexOnSuccess()
        {
            var editModel = new TaskEditModel { Id = 1, Description = "Updated Task" };

            var result = await _controller.Edit(editModel);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Delete_Get_ShouldReturnViewWithDeleteModel()
        {
            var task = new TaskModel { Id = 1, Description = "Test Task" };
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(task);

            var deleteModel = new TaskDeleteModel { Id = 1, Description = "Test Task" };
            _mockMapper.Setup(m => m.Map<TaskDeleteModel>(task)).Returns(deleteModel);

            var result = await _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(deleteModel, viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldRedirectToIndexOnSuccess()
        {
            var result = await _controller.DeleteConfirmed(1);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }


    }
}
