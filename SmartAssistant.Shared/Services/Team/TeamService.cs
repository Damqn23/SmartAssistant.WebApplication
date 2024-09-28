using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services.Teams
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
        }

        public async Task<TeamModel> GetTeamByIdAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            var owner = await _userRepository.GetUserByIdAsync(team.OwnerId);  // Fetch owner details

            if (owner != null)
            {
                team.OwnerUserName = owner.UserName;  // Set owner's username
            }
            else
            {
                team.OwnerUserName = "Unknown"; // Fallback in case owner not found
            }

            return team;
        }

        public async Task<IEnumerable<TeamModel>> GetAllTeamsAsync()
        {
            var teams = await _teamRepository.GetAllAsync();

            foreach (var team in teams)
            {
                var owner = await _userRepository.GetUserByIdAsync(team.OwnerId);  // Fetch owner details

                if (owner != null)
                {
                    team.OwnerUserName = owner.UserName;  // Set owner's username
                }
                else
                {
                    team.OwnerUserName = "Unknown"; // Fallback in case owner not found
                }
            }

            return teams;
        }

        public async Task CreateTeamAsync(TeamCreateModel teamCreateModel, string ownerId)
        {
            var team = new TeamModel
            {
                TeamName = teamCreateModel.TeamName,
                OwnerId = ownerId
            };
            await _teamRepository.AddAsync(team);
        }

        public async Task RemoveUserFromTeamAsync(int teamId, string userId)
        {
            await _teamRepository.RemoveUserFromTeamAsync(teamId, userId);
        }

        public async Task DeleteTeamAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team != null)
            {
                await _teamRepository.DeleteAsync(team);
            }
        }

        public async Task AddUserToTeamAsync(int teamId, string userId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team != null)
            {
                await _teamRepository.AddUserToTeamAsync(userId, teamId);
            }
        }

        public async Task AddUserToTeamAsync(int teamId, string userId, string currentUserId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team.OwnerId != currentUserId)
            {
                throw new UnauthorizedAccessException("Only the team creator can add members.");
            }

            await _teamRepository.AddUserToTeamAsync(userId, teamId);
        }

        public async Task RemoveUserFromTeamAsync(int teamId, string userId, string currentUserId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team.OwnerId != currentUserId)
            {
                throw new UnauthorizedAccessException("Only the team creator can remove members.");
            }

            await _teamRepository.RemoveUserFromTeamAsync(teamId, userId);
        }

        public async Task<IEnumerable<TeamModel>> GetTeamsByUserIdAsync(string userId)
        {
            // Fetch teams where the user is a member
            var memberTeams = await _teamRepository.GetTeamsByUserIdAsync(userId);

            // Fetch teams where the user is the owner
            var ownedTeams = await _teamRepository.GetTeamsByOwnerIdAsync(userId); // Separate method for owned teams

            // Combine both sets of teams, avoiding duplication by team ID
            var allTeams = memberTeams.Concat(ownedTeams)
                                      .GroupBy(t => t.Id)
                                      .Select(g => g.First())
                                      .ToList();

            return allTeams;
        }


    }
}
