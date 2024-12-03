using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Speech;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Services.Speech;
using SmartAssistant.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Views
{
    public class EventControllerViewTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IGoogleSpeechService> _mockGoogleSpeechService;
        private readonly Mock<SpeechTextExtractionService> _mockExtractionService;
        private readonly EventController _controller;

        public EventControllerViewTests()
        {
            _mockEventService = new Mock<IEventService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockGoogleSpeechService = new Mock<IGoogleSpeechService>();
            _mockExtractionService = new Mock<SpeechTextExtractionService>();
            _mockMapper = new Mock<IMapper>(); // Initialize the mock mapper

            _controller = new EventController(
                _mockEventService.Object,
                _mockMapper.Object, // Pass the mock mapper to the controller
                _mockTaskService.Object,
                _mockGoogleSpeechService.Object,
                _mockExtractionService.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                    }))
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithEvents()
        {
            // Arrange
            var events = new List<EventModel>
    {
        new EventModel { Id = 1, EventTitle = "Test Event 1", EventDate = DateTime.Now },
        new EventModel { Id = 2, EventTitle = "Test Event 2", EventDate = DateTime.Now.AddDays(1) }
    };

            _mockEventService.Setup(s => s.GetEventsByUserIdAsync(It.IsAny<string>())).ReturnsAsync(events);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<EventModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_Get_ShouldReturnView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task Create_Post_ShouldRedirectToIndexOnSuccess()
        {
            // Arrange
            var createModel = new EventCreateModel
            {
                EventTitle = "New Event",
                EventDate = DateTime.Now
            };

            _mockEventService.Setup(s => s.AddEventAsync(It.IsAny<EventCreateModel>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(createModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ShouldReturnViewOnInvalidModelState()
        {
            // Arrange
            var createModel = new EventCreateModel
            {
                EventTitle = "",
                EventDate = DateTime.Now
            };
            _controller.ModelState.AddModelError("EventTitle", "Required");

            // Act
            var result = await _controller.Create(createModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(createModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_ShouldReturnViewWithEditModel()
        {
            // Arrange
            var eventModel = new EventModel
            {
                Id = 1,
                EventTitle = "Test Event",
                EventDate = DateTime.Now
            };

            _mockEventService.Setup(s => s.GetEventByIdAsync(It.IsAny<int>())).ReturnsAsync(eventModel);
            _mockMapper.Setup(m => m.Map<EventEditModel>(It.IsAny<EventModel>())).Returns(new EventEditModel
            {
                Id = eventModel.Id,
                EventTitle = eventModel.EventTitle,
                EventDate = eventModel.EventDate
            });

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EventEditModel>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task Edit_Post_ShouldRedirectToIndexOnSuccess()
        {
            // Arrange
            var editModel = new EventEditModel
            {
                Id = 1,
                EventTitle = "Updated Event",
                EventDate = DateTime.Now
            };
            _mockEventService.Setup(s => s.UpdateEventAsync(editModel)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(editModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ShouldReturnViewOnInvalidModelState()
        {
            // Arrange
            var editModel = new EventEditModel
            {
                Id = 1,
                EventTitle = "",
                EventDate = DateTime.Now
            };
            _controller.ModelState.AddModelError("EventTitle", "Required");

            // Act
            var result = await _controller.Edit(editModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editModel, viewResult.Model);
        }

        [Fact]
        public async Task Delete_Get_ShouldReturnViewWithDeleteModel()
        {
            // Arrange
            var eventModel = new EventModel
            {
                Id = 1,
                EventTitle = "Test Event",
                EventDate = DateTime.Now
            };
            _mockEventService.Setup(s => s.GetEventByIdAsync(1)).ReturnsAsync(eventModel);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EventModel>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task Delete_Post_ShouldRedirectToIndexOnSuccess()
        {
            // Arrange
            _mockEventService.Setup(s => s.DeleteEventAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
