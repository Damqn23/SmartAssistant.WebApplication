using AutoMapper;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.Shared.Repositories.Team;
using SmartAssistant.WebApplication.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Repositories
{
    public class TeamRepositoryTests : RepositoryTestBase
    {
        private readonly IMapper _mapper;

        public TeamRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TeamModel, SmartAssistant.WebApp.Data.Entities.Team>().ReverseMap();
                cfg.CreateMap<UserModel, SmartAssistant.WebApp.Data.Entities.User>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task AddAsync_ShouldAddTeamToDatabase()
        {
            using var context = CreateDbContext();
            var repository = new TeamRepository(context, _mapper, null);
            var team = new TeamModel { TeamName = "Test Team", OwnerId = "owner123" };

            await repository.AddAsync(team);

            var addedTeam = context.Teams.FirstOrDefault(t => t.TeamName == "Test Team");
            Assert.NotNull(addedTeam);
            Assert.Equal("Test Team", addedTeam.TeamName);
        }

        [Fact]
        public async Task AddUserToTeamAsync_ShouldAddUserToTeam()
        {
            using var context = CreateDbContext();
            var repository = new TeamRepository(context, _mapper, null);

            var team = new SmartAssistant.WebApp.Data.Entities.Team { TeamName = "Test Team", OwnerId = "owner123" };
            context.Teams.Add(team);
            await context.SaveChangesAsync();

            await repository.AddUserToTeamAsync("user123", team.Id);

            var userTeam = context.UserTeams.FirstOrDefault(ut => ut.TeamId == team.Id && ut.UserId == "user123");
            Assert.NotNull(userTeam);
            Assert.Equal(team.Id, userTeam.TeamId);
            Assert.Equal("user123", userTeam.UserId);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTeamFromDatabase()
        {
            using var context = CreateDbContext();
            var repository = new TeamRepository(context, _mapper, null);

            var team = new SmartAssistant.WebApp.Data.Entities.Team { TeamName = "Test Team", OwnerId = "owner123" };
            context.Teams.Add(team);
            await context.SaveChangesAsync();

            var teamModel = _mapper.Map<TeamModel>(team);

            await repository.DeleteAsync(teamModel);

            var deletedTeam = context.Teams.FirstOrDefault(t => t.Id == team.Id);
            Assert.Null(deletedTeam);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectTeam()
        {
            using var context = CreateDbContext();
            var repository = new TeamRepository(context, _mapper, null);

            var owner = new SmartAssistant.WebApp.Data.Entities.User { Id = "owner123", UserName = "Team Owner" };
            var team = new SmartAssistant.WebApp.Data.Entities.Team { TeamName = "Test Team", OwnerId = owner.Id };

            context.Users.Add(owner);
            context.Teams.Add(team);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(team.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Team", result.TeamName);
            Assert.Equal("owner123", result.OwnerId);
        }


        
        [Fact]
        public async Task UpdateAsync_ShouldUpdateTeam()
        {
            using var context = CreateDbContext();
            var repository = new TeamRepository(context, _mapper, null);

            var team = new SmartAssistant.WebApp.Data.Entities.Team { TeamName = "Old Team Name", OwnerId = "owner123" };
            context.Teams.Add(team);
            await context.SaveChangesAsync();

            var updatedTeamModel = _mapper.Map<TeamModel>(team);
            updatedTeamModel.TeamName = "Updated Team Name";

            await repository.UpdateAsync(updatedTeamModel);

            var updatedTeam = context.Teams.FirstOrDefault(t => t.Id == team.Id);
            Assert.NotNull(updatedTeam);
            Assert.Equal("Updated Team Name", updatedTeam.TeamName);
        }

        [Fact]
        public async Task GetTeamMembersByTeamIdAsync_ShouldReturnTeamMembers()
        {
            using var context = CreateDbContext();
            var repository = new TeamRepository(context, _mapper, null);

            var team = new SmartAssistant.WebApp.Data.Entities.Team { TeamName = "Team With Members", OwnerId = "owner123" };
            var user1 = new SmartAssistant.WebApp.Data.Entities.User { Id = "user1", UserName = "User 1" };
            var user2 = new SmartAssistant.WebApp.Data.Entities.User { Id = "user2", UserName = "User 2" };

            context.Teams.Add(team);
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            context.UserTeams.AddRange(
                new UserTeam { UserId = "user1", TeamId = team.Id },
                new UserTeam { UserId = "user2", TeamId = team.Id }
            );
            await context.SaveChangesAsync();

            var result = await repository.GetTeamMembersByTeamIdAsync(team.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.UserName == "User 1");
            Assert.Contains(result, u => u.UserName == "User 2");
        }
    }
}
