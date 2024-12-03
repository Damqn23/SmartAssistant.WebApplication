using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Interfaces.User;
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
    public class TeamControllerTests
    {
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly TeamController _controller;

        public TeamControllerTests()
        {
            _mockTeamService = new Mock<ITeamService>();
            _mockUserRepository = new Mock<IUserRepository>();

            var userContext = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }));

            _controller = new TeamController(_mockTeamService.Object, _mockUserRepository.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = userContext }
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithTeams()
        {
            // Arrange
            var teams = new List<TeamModel>
            {
                new TeamModel { Id = 1, TeamName = "Team 1" },
                new TeamModel { Id = 2, TeamName = "Team 2" }
            };

            _mockTeamService.Setup(s => s.GetTeamsByUserIdAsync("test-user-id"))
                .ReturnsAsync(teams);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TeamModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ShouldReturnViewWhenTeamExists()
        {
            // Arrange
            var team = new TeamModel { Id = 1, TeamName = "Team 1" };
            _mockTeamService.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync(team);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(team, viewResult.Model);
        }

        [Fact]
        public async Task Details_ShouldReturnNotFoundWhenTeamDoesNotExist()
        {
            // Arrange
            _mockTeamService.Setup(s => s.GetTeamByIdAsync(1)).ReturnsAsync((TeamModel)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldRedirectToIndexOnValidModel()
        {
            // Arrange
            var model = new TeamCreateModel { TeamName = "New Team" };

            // Act
            var result = await _controller.Create(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_ShouldReturnViewOnInvalidModelState()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            var model = new TeamCreateModel();

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task AddUserToTeam_ShouldRedirectToDetailsOnSuccess()
        {
            // Arrange
            var user = new UserModel { Id = "user1", UserName = "TestUser" };
            _mockUserRepository.Setup(r => r.GetUserByUserNameAsync("TestUser")).ReturnsAsync(user);

            // Act
            var result = await _controller.AddUserToTeam(1, "TestUser");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task AddUserToTeam_ShouldReturnErrorWhenUserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetUserByUserNameAsync("NonExistentUser"))
                .ReturnsAsync((UserModel)null);

            // Act
            var result = await _controller.AddUserToTeam(1, "NonExistentUser");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task RemoveUserFromTeam_ShouldRedirectToDetailsOnSuccess()
        {
            // Arrange
            var user = new UserModel { Id = "user1", UserName = "TestUser" };
            _mockUserRepository.Setup(r => r.GetUserByUserNameAsync("TestUser")).ReturnsAsync(user);

            // Act
            var result = await _controller.RemoveUserFromTeam(1, "TestUser");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task RemoveUserFromTeam_ShouldReturnErrorWhenUserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetUserByUserNameAsync("NonExistentUser"))
                .ReturnsAsync((UserModel)null);

            // Act
            var result = await _controller.RemoveUserFromTeam(1, "NonExistentUser");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldRedirectToIndexOnSuccess()
        {
            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Chat_ShouldReturnViewWithTeams()
        {
            // Arrange
            var teams = new List<TeamModel>
            {
                new TeamModel { Id = 1, TeamName = "Team 1" },
                new TeamModel { Id = 2, TeamName = "Team 2" }
            };

            _mockTeamService.Setup(s => s.GetTeamsByUserIdAsync("test-user-id"))
                .ReturnsAsync(teams);

            // Act
            var result = await _controller.Chat(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TeamModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void TeamChat_ShouldReturnViewWithChatModel()
        {
            // Act
            var result = _controller.TeamChat(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ChatViewModel>(viewResult.Model);
            Assert.Equal(1, model.TeamId);
            Assert.Equal("test-user-id", model.UserId);
        }
    }
}
