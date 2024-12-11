using Microsoft.AspNetCore.SignalR;
using Moq;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Services.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace SmartAssistant.Tests.Services
{
    public class TeamServiceTests
    {
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IHubContext<NotificationHub>> _mockHubContext;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly TeamService _teamService;

        public TeamServiceTests()
        {
            _mockTeamRepository = new Mock<ITeamRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
            _mockClientProxy = new Mock<IClientProxy>();

            var mockClients = new Mock<IHubClients>();
            _mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.All).Returns(_mockClientProxy.Object);

            
        }

        [Fact]
        public async Task CreateTeamAsync_ShouldCreateTeam()
        {
            var teamCreateModel = new TeamCreateModel
            {
                TeamName = "Test Team"
            };
            var ownerId = "user123";

            await _teamService.CreateTeamAsync(teamCreateModel, ownerId);

            _mockTeamRepository.Verify(r => r.AddAsync(It.Is<TeamModel>(
                t => t.TeamName == teamCreateModel.TeamName && t.OwnerId == ownerId)), Times.Once);
        }

        [Fact]
        public async Task AddUserToTeamAsync_ShouldAddUser_WhenAuthorized()
        {
            var teamId = 1;
            var userId = "user123";
            var currentUserId = "owner123";
            var team = new TeamModel { Id = teamId, OwnerId = currentUserId };
            var user = new UserModel { Id = userId, UserName = "John Doe" };

            _mockTeamRepository.Setup(r => r.GetByIdAsync(teamId)).ReturnsAsync(team);
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            _mockClientProxy
                .Setup(client => client.SendCoreAsync(
                    It.Is<string>(method => method == "UserAddedToTeam"),
                    It.Is<object[]>(args => args.Length == 1 && args[0].ToString().Contains("John Doe")),
                    default))
                .Returns(Task.CompletedTask);

            await _teamService.AddUserToTeamAsync(teamId, userId, currentUserId);

            _mockTeamRepository.Verify(r => r.AddUserToTeamAsync(userId, teamId), Times.Once);
            _mockClientProxy.Verify(client => client.SendCoreAsync(
                It.Is<string>(method => method == "UserAddedToTeam"),
                It.IsAny<object[]>(),
                default), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromTeamAsync_ShouldRemoveUser_WhenAuthorized()
        {
            var teamId = 1;
            var userId = "user123";
            var currentUserId = "owner123";
            var team = new TeamModel { Id = teamId, OwnerId = currentUserId };
            var user = new UserModel { Id = userId, UserName = "John Doe" };

            _mockTeamRepository.Setup(r => r.GetByIdAsync(teamId)).ReturnsAsync(team);
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            _mockClientProxy
                .Setup(client => client.SendCoreAsync(
                    It.Is<string>(method => method == "UserRemovedFromTeam"),
                    It.Is<object[]>(args => args.Length == 1 && args[0].ToString().Contains("John Doe")),
                    default))
                .Returns(Task.CompletedTask);

            await _teamService.RemoveUserFromTeamAsync(teamId, userId, currentUserId);

            _mockTeamRepository.Verify(r => r.RemoveUserFromTeamAsync(teamId, userId), Times.Once);
            _mockClientProxy.Verify(client => client.SendCoreAsync(
                It.Is<string>(method => method == "UserRemovedFromTeam"),
                It.IsAny<object[]>(),
                default), Times.Once);
        }

        [Fact]
        public async Task GetTeamByIdAsync_ShouldReturnTeam_WhenExists()
        {
            var teamId = 1;
            var team = new TeamModel { Id = teamId, OwnerId = "owner123" };
            var owner = new UserModel { Id = "owner123", UserName = "John Doe" };

            _mockTeamRepository.Setup(r => r.GetByIdAsync(teamId)).ReturnsAsync(team);
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(team.OwnerId)).ReturnsAsync(owner);

            var result = await _teamService.GetTeamByIdAsync(teamId);

            result.Should().BeEquivalentTo(team);
            result.OwnerUserName.Should().Be(owner.UserName);
        }

        [Fact]
        public async Task GetAllTeamsAsync_ShouldReturnTeamsForUser()
        {
            var userId = "user123";
            var teams = new List<TeamModel>
            {
                new TeamModel { Id = 1, TeamName = "Team 1", OwnerId = "owner123" },
                new TeamModel { Id = 2, TeamName = "Team 2", OwnerId = "owner456" }
            };
            var owners = new List<UserModel>
            {
                new UserModel { Id = "owner123", UserName = "John Doe" },
                new UserModel { Id = "owner456", UserName = "Jane Doe" }
            };

            _mockTeamRepository.Setup(r => r.GetTeamsByUserIdAsync(userId)).ReturnsAsync(teams);
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync((string ownerId) =>
                owners.FirstOrDefault(o => o.Id == ownerId));

            var result = await _teamService.GetAllTeamsAsync(userId);

            result.Should().HaveCount(2);
            result.First().OwnerUserName.Should().Be("John Doe");
            result.Last().OwnerUserName.Should().Be("Jane Doe");
        }
    }
}
