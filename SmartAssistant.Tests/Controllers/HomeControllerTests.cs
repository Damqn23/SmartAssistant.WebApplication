using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApplication.Controllers;
using SmartAssistant.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartAssistant.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockTaskService = new Mock<ITaskService>();
            _mockEventService = new Mock<IEventService>();
            _mockTeamService = new Mock<ITeamService>();

            _controller = new HomeController(
                _mockLogger.Object,
                _mockTaskService.Object,
                _mockEventService.Object,
                _mockTeamService.Object
            );
        }

        [Fact]
        public void Index_ShouldReturnView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ShouldReturnView()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ShouldReturnViewWithErrorViewModel()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id";
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("test-trace-id", model.RequestId);
        }

        [Fact]
        public async Task GlobalSearch_ShouldReturnViewWithSearchResults()
        {
            // Arrange
            var searchQuery = "Test";
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Description = "Test Task" }
            };
            var events = new List<EventModel>
            {
                new EventModel { Id = 1, EventTitle = "Test Event" }
            };
            var teams = new List<TeamModel>
            {
                new TeamModel { Id = 1, TeamName = "Test Team" }
            };

            _mockTaskService.Setup(s => s.GetTasksBySearchQueryAsync(searchQuery)).ReturnsAsync(tasks);
            _mockEventService.Setup(s => s.GetEventsBySearchQueryAsync(searchQuery)).ReturnsAsync(events);
            _mockTeamService.Setup(s => s.GetTeamsBySearchQueryAsync(searchQuery)).ReturnsAsync(teams);

            // Act
            var result = await _controller.GlobalSearch(searchQuery);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("GlobalSearchResults", viewResult.ViewName);
            var model = Assert.IsType<GlobalSearchViewModel>(viewResult.Model);
            Assert.Single(model.Tasks);
            Assert.Single(model.Events);
            Assert.Single(model.Teams);
        }

        [Fact]
        public async Task GlobalSearch_ShouldReturnIndexViewWhenQueryIsEmpty()
        {
            // Act
            var result = await _controller.GlobalSearch("");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
        } 
    }
}
