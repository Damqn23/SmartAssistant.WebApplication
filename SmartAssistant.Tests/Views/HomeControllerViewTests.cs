using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SmartAssistant.Tests.Views
{
    public class HomeControllerViewTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly HomeController _controller;

        public HomeControllerViewTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockEventService = new Mock<IEventService>();
            _mockTeamService = new Mock<ITeamService>();

            _controller = new HomeController(null, _mockTaskService.Object, _mockEventService.Object, _mockTeamService.Object);
        }

        [Fact]
        public void Index_ShouldReturnView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public void Privacy_ShouldReturnView()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task GlobalSearch_ShouldReturnViewWithSearchResults()
        {
            // Arrange
            var searchQuery = "test";
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = 1, Description = "Test Task 1" },
                new TaskModel { Id = 2, Description = "Test Task 2" }
            };
            var events = new List<EventModel>
            {
                new EventModel { Id = 1, EventTitle = "Test Event 1" },
                new EventModel { Id = 2, EventTitle = "Test Event 2" }
            };
            var teams = new List<TeamModel>
            {
                new TeamModel { Id = 1, TeamName = "Test Team 1" },
                new TeamModel { Id = 2, TeamName = "Test Team 2" }
            };

            _mockTaskService.Setup(s => s.GetTasksBySearchQueryAsync(searchQuery)).ReturnsAsync(tasks);
            _mockEventService.Setup(s => s.GetEventsBySearchQueryAsync(searchQuery)).ReturnsAsync(events);
            _mockTeamService.Setup(s => s.GetTeamsBySearchQueryAsync(searchQuery)).ReturnsAsync(teams);

            // Act
            var result = await _controller.GlobalSearch(searchQuery);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GlobalSearchViewModel>(viewResult.Model);

            Assert.Equal(tasks, model.Tasks);
            Assert.Equal(events, model.Events);
            Assert.Equal(teams, model.Teams);
        }

        [Fact]
        public async Task GlobalSearch_ShouldReturnIndexViewWhenQueryIsEmpty()
        {
            // Arrange
            var searchQuery = string.Empty;

            // Act
            var result = await _controller.GlobalSearch(searchQuery);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
        }
    }
}
