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

namespace SmartAssistant.Tests.Views
{
    public class CalendarControllerViewTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly CalendarController _controller;

        public CalendarControllerViewTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockEventService = new Mock<IEventService>();
            _mockTeamService = new Mock<ITeamService>();

            _controller = new CalendarController(
                _mockTaskService.Object,
                _mockEventService.Object,
                _mockTeamService.Object,
                null // Mapper is not being used in these tests
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                        new Claim(ClaimTypes.Name, "TestUser")
                    }))
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithCalendarViewModel()
        {
            _mockTaskService.Setup(s => s.GetTasksByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<TaskModel>
                {
                    new TaskModel { Description = "Task 1", DueDate = DateTime.Now }
                });

            _mockEventService.Setup(s => s.GetEventsByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<EventModel>
                {
                    new EventModel { EventTitle = "Event 1", EventDate = DateTime.Now }
                });

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CalendarViewModel>(viewResult.Model);
            Assert.NotNull(model);
            Assert.NotEmpty(model.Days);
        }

        [Fact]
        public async Task TeamIndex_ShouldReturnViewWithCalendarViewModel()
        {
            _mockTeamService.Setup(s => s.GetTeamByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new TeamModel { Id = 1, OwnerUserName = "TestUser" });

            _mockTaskService.Setup(s => s.GetTasksByTeamIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<TaskModel>
                {
                    new TaskModel { Description = "Team Task 1", DueDate = DateTime.Now }
                });

            _mockEventService.Setup(s => s.GetEventsByTeamIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<EventModel>
                {
                    new EventModel { EventTitle = "Team Event 1", EventDate = DateTime.Now }
                });

            var result = await _controller.TeamIndex(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CalendarViewModel>(viewResult.Model);
            Assert.NotNull(model);
            Assert.NotEmpty(model.Days);
        }

        [Fact]
        public async Task AddTeamTask_Get_ShouldReturnViewWithTeamTaskCreateModel()
        {
            _mockTeamService.Setup(s => s.GetTeamByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new TeamModel { Id = 1, OwnerUserName = "TestUser" });

            _mockTeamService.Setup(s => s.GetTeamMembersByTeamIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<UserModel>
                {
                    new UserModel { Id = "user1", UserName = "Member 1" },
                    new UserModel { Id = "user2", UserName = "Member 2" }
                });

            var result = await _controller.AddTeamTask(1, DateTime.Now);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TeamTaskCreateModel>(viewResult.Model);
            Assert.NotNull(model.TeamMembers);
        }
        [Fact]
        public async Task AddTeamEvent_Get_ShouldReturnViewWithTeamEventCreateModel()
        {
            _mockTeamService.Setup(s => s.GetTeamByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new TeamModel { Id = 1, OwnerUserName = "TestUser" });

            _mockTeamService.Setup(s => s.GetTeamMembersByTeamIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<UserModel>
                {
                    new UserModel { Id = "user1", UserName = "Member 1" },
                    new UserModel { Id = "user2", UserName = "Member 2" }
                });

            var result = await _controller.AddTeamEvent(1, DateTime.Now);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TeamEventCreateModel>(viewResult.Model);
            Assert.NotNull(model.TeamMembers);
        }

        
    }
}
