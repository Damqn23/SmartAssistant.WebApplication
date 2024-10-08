using Microsoft.AspNetCore.SignalR;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services.Teams
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository teamRepository;
        private readonly IUserRepository userRepository;
        private readonly IHubContext<NotificationHub> hubContext;

        public TeamService(ITeamRepository _teamRepository, IUserRepository _userRepository, IHubContext<NotificationHub> _hubContext)
        {
            teamRepository = _teamRepository;
            userRepository = _userRepository;
            hubContext = _hubContext;
        }

        public async Task<TeamModel> GetTeamByIdAsync(int id)
        {
            var team = await teamRepository.GetByIdAsync(id);

            if (team == null)
            {
                throw new Exception($"Team with ID {id} was not found.");
            }

            if (string.IsNullOrEmpty(team.OwnerId))
            {
                // Log or handle the case where the OwnerId is missing or null
                throw new Exception($"Team with ID {id} has no valid owner.");
            }

            // Fetch the owner's details
            var owner = await userRepository.GetUserByIdAsync(team.OwnerId);

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


        public async Task<IEnumerable<TeamModel>> GetAllTeamsAsync(string userId)
        {
            var teams = await teamRepository.GetTeamsByUserIdAsync(userId);

            foreach (var team in teams)
            {
                var owner = await userRepository.GetUserByIdAsync(team.OwnerId);  // Fetch owner details

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

        public async System.Threading.Tasks.Task CreateTeamAsync(TeamCreateModel teamCreateModel, string ownerId)
        {
            var team = new TeamModel
            {
                TeamName = teamCreateModel.TeamName,
                OwnerId = ownerId
            };
            await teamRepository.AddAsync(team);
        }

        public async System.Threading.Tasks.Task RemoveUserFromTeamAsync(int teamId, string userId)
        {
            await teamRepository.RemoveUserFromTeamAsync(teamId, userId);
        }

        public async System.Threading.Tasks.Task DeleteTeamAsync(int id)
        {
            var team = await teamRepository.GetByIdAsync(id);
            if (team != null)
            {
                await teamRepository.DeleteAsync(team);
            }
        }

        public async System.Threading.Tasks.Task AddUserToTeamAsync(int teamId, string userId, string currentUserId)
        {
            var team = await teamRepository.GetByIdAsync(teamId);
            var user = await userRepository.GetUserByIdAsync(userId);
            if (team.OwnerId != currentUserId)
            {
                throw new UnauthorizedAccessException("Only the team creator can add members.");
            }

            await teamRepository.AddUserToTeamAsync(userId, teamId);

            if(user != null && team != null)
            {
                string message = $"User {user.UserName} was added to the team {team.TeamName}";
                await hubContext.Clients.All.SendAsync("UserAddedToTeam", message);
            }
        }

        public async System.Threading.Tasks.Task RemoveUserFromTeamAsync(int teamId, string userId, string currentUserId)
        {
            var team = await teamRepository.GetByIdAsync(teamId);
            var user = await userRepository.GetUserByIdAsync(userId);
            if (team.OwnerId != currentUserId)
            {
                throw new UnauthorizedAccessException("Only the team creator can remove members.");
            }

            await teamRepository.RemoveUserFromTeamAsync(teamId, userId);

            if (user != null && team != null)
            {
                string message = $"User {user.UserName} was removed from the team {team.TeamName}";
                await hubContext.Clients.All.SendAsync("UserRemovedFromTeam", message);
            }
        }

        public async Task<IEnumerable<TeamModel>> GetTeamsByUserIdAsync(string userId)
        {
            
            var memberTeams = await teamRepository.GetTeamsByUserIdAsync(userId);
            var ownedTeams = await teamRepository.GetTeamsByOwnerIdAsync(userId);
            var allTeams = memberTeams.Concat(ownedTeams)
                                      .GroupBy(t => t.Id)
                                      .Select(g => g.First())
                                      .ToList();
            return allTeams;
        }

        public async Task<IEnumerable<UserModel>> GetTeamMembersByTeamIdAsync(int teamId)
        {
            var members = await teamRepository.GetTeamMembersByTeamIdAsync(teamId);
            return members ?? new List<UserModel>(); // Return empty list if null
        }
    }
}
