using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Team
{
    public interface ITeamRepository : IRepository<TeamModel>
    {
        System.Threading.Tasks.Task AddUserToTeamAsync(string userId, int teamId);
        Task<IEnumerable<TeamModel>> GetTeamsByUserIdAsync(string userId);

        System.Threading.Tasks.Task RemoveUserFromTeamAsync(int teamId, string userId);

        Task<IEnumerable<TeamModel>> GetTeamsByOwnerIdAsync(string ownerId); // Add this line

        Task<IEnumerable<UserModel>> GetTeamMembersByTeamIdAsync(int teamId);

        public Task<IEnumerable<TaskModel>> GetTasksByTeamIdAsync(int teamId);
        public Task<IEnumerable<EventModel>> GetEventsByTeamIdAsync(int teamId);
    }
}
