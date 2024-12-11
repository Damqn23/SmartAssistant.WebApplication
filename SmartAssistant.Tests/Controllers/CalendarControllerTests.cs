using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Models.Calendar;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Controllers
{
    public class CalendarControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CalendarController _controller;

        public CalendarControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockEventService = new Mock<IEventService>();
            _mockTeamService = new Mock<ITeamService>();
            _mockMapper = new Mock<IMapper>();

            var userContext = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "test-username")
            }));

            _controller = new CalendarController(
                _mockTaskService.Object,
                _mockEventService.Object,
                _mockTeamService.Object,
                _mockMapper.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = userContext }
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithCalendarViewModel()
        {
            var tasks = new List<TaskModel>
            {
                new TaskModel { Description = "Task 1", DueDate = DateTime.Today }
            };

            var events = new List<EventModel>
            {
                new EventModel { EventTitle = "Event 1", EventDate = DateTime.Today }
            };

            _mockTaskService.Setup(s => s.GetTasksByUserIdAsync("test-user-id")).ReturnsAsync(tasks);
            _mockEventService.Setup(s => s.GetEventsByUserIdAsync("test-user-id")).ReturnsAsync(events);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CalendarViewModel>(viewResult.Model);
            Assert.NotEmpty(model.Days);
        }

        [Fact]
        public async Task TeamIndex_ShouldReturnViewWithTeamCalendar()
        {
            var team = new TeamModel { Id = 1, TeamName = "Test Team", OwnerUserName = "test-username" };

            _mockTeamService.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync(team);
            _mockTaskService.Setup(s => s.GetTasksByTeamIdAsync(1)).ReturnsAsync(new List<TaskModel>());
            _mockEventService.Setup(s => s.GetEventsByTeamIdAsync(1)).ReturnsAsync(new List<EventModel>());

            var result = await _controller.TeamIndex(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CalendarViewModel>(viewResult.Model);
            Assert.Equal("test-username", model.TeamOwnerUserName);
        }

        [Fact]
        public async Task AddTeamTask_Get_ShouldReturnViewWithModel()
        {
            var team = new TeamModel { Id = 1, TeamName = "Test Team", OwnerUserName = "test-username", OwnerId = "test-owner-id" };
            var teamMembers = new List<UserModel>
            {
                new UserModel { Id = "member-1", UserName = "Member 1" }
            };

            _mockTeamService.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync(team);
            _mockTeamService.Setup(s => s.GetTeamMembersByTeamIdAsync(1)).ReturnsAsync(teamMembers);

            var result = await _controller.AddTeamTask(1, DateTime.Today);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TeamTaskCreateModel>(viewResult.Model);
            Assert.Equal(1, model.TeamId);
            Assert.NotEmpty(model.TeamMembers);
        }

       

        [Fact]
        public async Task AddTeamEvent_Get_ShouldReturnViewWithModel()
        {
            var team = new TeamModel { Id = 1, TeamName = "Test Team", OwnerUserName = "test-username", OwnerId = "test-owner-id" };
            var teamMembers = new List<UserModel>
            {
                new UserModel { Id = "member-1", UserName = "Member 1" }
            };

            _mockTeamService.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync(team);
            _mockTeamService.Setup(s => s.GetTeamMembersByTeamIdAsync(1)).ReturnsAsync(teamMembers);

            var result = await _controller.AddTeamEvent(1, DateTime.Today);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TeamEventCreateModel>(viewResult.Model);
            Assert.Equal(1, model.TeamId);
            Assert.NotEmpty(model.TeamMembers);
        }

        
    }
}
