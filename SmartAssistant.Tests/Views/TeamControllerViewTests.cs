using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SmartAssistant.Tests.Views
{
    public class TeamControllerViewTests
    {
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly TeamController _controller;

        public TeamControllerViewTests()
        {
            _mockTeamService = new Mock<ITeamService>();

            _controller = new TeamController(_mockTeamService.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithTeams()
        {
            var teams = new List<TeamModel>
    {
        new TeamModel { Id = 1, TeamName = "Team A", OwnerUserName = "Owner A" },
        new TeamModel { Id = 2, TeamName = "Team B", OwnerUserName = "Owner B" }
    };

            _mockTeamService.Setup(s => s.GetTeamsByUserIdAsync("123")).ReturnsAsync(teams);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.NameIdentifier, "123")
                    }))
                }
            };

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TeamModel>>(viewResult.Model);
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
            var createModel = new TeamCreateModel { TeamName = "New Team" };

            _mockTeamService
                .Setup(s => s.CreateTeamAsync(createModel, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.NameIdentifier, "123") // Mock authenticated user ID
                    }))
                }
            };

            var result = await _controller.Create(createModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ShouldReturnViewIfModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("TeamName", "Required");

            var createModel = new TeamCreateModel();

            var result = await _controller.Create(createModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(createModel, viewResult.Model);
        }

        [Fact]
        public async Task Details_ShouldReturnViewWithTeamModel()
        {
            var team = new TeamModel
            {
                Id = 1,
                TeamName = "Team A",
                OwnerUserName = "Owner A",
                Members = new List<UserModel>
                {
                    new UserModel { Id = "1", UserName = "User A" },
                    new UserModel { Id = "2", UserName = "User B" }
                }
            };

            _mockTeamService.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync(team);

            var result = await _controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TeamModel>(viewResult.Model);
            Assert.Equal(team.TeamName, model.TeamName);
        }

        [Fact]
        public async Task Delete_Get_ShouldReturnViewWithTeamModel()
        {
            var team = new TeamModel { Id = 1, TeamName = "Team A" };

            _mockTeamService.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync(team);

            var result = await _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TeamModel>(viewResult.Model);
            Assert.Equal(team.TeamName, model.TeamName);
        }

        [Fact]
        public async Task Delete_Post_ShouldRedirectToIndexOnSuccess()
        {
            _mockTeamService.Setup(s => s.DeleteTeamAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
