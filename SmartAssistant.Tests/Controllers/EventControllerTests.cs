using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Speech;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Event;
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
    public class EventControllerTests
    {
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IGoogleSpeechService> _mockGoogleSpeechService;
        private readonly Mock<SpeechTextExtractionService> _mockExtractionService;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _mockEventService = new Mock<IEventService>();
            _mockMapper = new Mock<IMapper>();
            _mockTaskService = new Mock<ITaskService>();
            _mockGoogleSpeechService = new Mock<IGoogleSpeechService>();
            _mockExtractionService = new Mock<SpeechTextExtractionService>();

            var userContext = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        }));

            _controller = new EventController(
                _mockEventService.Object,
                _mockMapper.Object,
                _mockTaskService.Object,
                _mockGoogleSpeechService.Object,
                _mockExtractionService.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = userContext }
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithEvents()
        {
            // Arrange
            var events = new List<EventModel>
            {
                new EventModel { Id = 1, EventTitle = "Event 1" }
            };
            _mockEventService.Setup(s => s.GetEventsByUserIdAsync("test-user-id"))
                .ReturnsAsync(events);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<EventModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Create_ShouldRedirectToIndexOnValidModel()
        {
            // Arrange
            var eventCreateModel = new EventCreateModel { EventTitle = "New Event" };

            // Act
            var result = await _controller.Create(eventCreateModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_ShouldReturnViewOnInvalidModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("EventTitle", "Required");
            var eventCreateModel = new EventCreateModel();

            // Act
            var result = await _controller.Create(eventCreateModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(eventCreateModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ShouldReturnViewWithEditModel()
        {
            // Arrange
            var eventModel = new EventModel { Id = 1, EventTitle = "Test Event" };
            var editModel = new EventEditModel { Id = 1, EventTitle = "Test Event" };

            _mockEventService.Setup(s => s.GetEventByIdAsync(1)).ReturnsAsync(eventModel);
            _mockMapper.Setup(m => m.Map<EventEditModel>(eventModel)).Returns(editModel);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ShouldReturnNotFoundWhenEventDoesNotExist()
        {
            // Arrange
            _mockEventService.Setup(s => s.GetEventByIdAsync(1)).ReturnsAsync((EventModel)null);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldRedirectToIndex()
        {
            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task GetCalendarEvents_ShouldReturnJsonWithEventsAndTasks()
        {
            // Arrange
            var events = new List<EventModel>
            {
                new EventModel { EventTitle = "Event 1", EventDate = DateTime.Now }
            };
            var tasks = new List<TaskModel>
            {
                new TaskModel { Description = "Task 1", DueDate = DateTime.Now }
            };

            _mockEventService.Setup(s => s.GetEventsByUserIdAsync("test-user-id")).ReturnsAsync(events);
            _mockTaskService.Setup(s => s.GetTasksByUserIdAsync("test-user-id")).ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetCalendarEvents();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = jsonResult.Value as IEnumerable<object>;
            Assert.NotNull(data);
            Assert.Equal(2, data.Count()); // One event, one task
        }
    }
}
