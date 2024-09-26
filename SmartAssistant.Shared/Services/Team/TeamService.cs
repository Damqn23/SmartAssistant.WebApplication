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
            return await _teamRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TeamModel>> GetAllTeamsAsync()
        {
            return await _teamRepository.GetAllAsync();
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
    }
}
