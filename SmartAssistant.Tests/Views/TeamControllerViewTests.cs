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
            // Arrange
            var teams = new List<TeamModel>
    {
        new TeamModel { Id = 1, TeamName = "Team A", OwnerUserName = "Owner A" },
        new TeamModel { Id = 2, TeamName = "Team B", OwnerUserName = "Owner B" }
    };

            _mockTeamService.Setup(s => s.GetTeamsByUserIdAsync("123")).ReturnsAsync(teams);

            // Simulate an authenticated user
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

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TeamModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_Get_ShouldReturnView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ShouldRedirectToIndexOnSuccess()
        {
            // Arrange
            var createModel = new TeamCreateModel { TeamName = "New Team" };

            _mockTeamService
                .Setup(s => s.CreateTeamAsync(createModel, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Simulate an authenticated user
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

            // Act
            var result = await _controller.Create(createModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ShouldReturnViewIfModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("TeamName", "Required");

            var createModel = new TeamCreateModel();

            // Act
            var result = await _controller.Create(createModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(createModel, viewResult.Model);
        }

        [Fact]
        public async Task Details_ShouldReturnViewWithTeamModel()
        {
            // Arrange
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

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TeamModel>(viewResult.Model);
            Assert.Equal(team.TeamName, model.TeamName);
        }

        [Fact]
        public async Task Delete_Get_ShouldReturnViewWithTeamModel()
        {
            // Arrange
            var team = new TeamModel { Id = 1, TeamName = "Team A" };

            _mockTeamService.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync(team);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TeamModel>(viewResult.Model);
            Assert.Equal(team.TeamName, model.TeamName);
        }

        [Fact]
        public async Task Delete_Post_ShouldRedirectToIndexOnSuccess()
        {
            // Arrange
            _mockTeamService.Setup(s => s.DeleteTeamAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
