using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Task
{
    public interface ITaskRepository : IRepository<TaskModel>
    {
        Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);
        Task<List<TaskModel>> GetTasksByTeamIdAsync(int teamId);
        System.Threading.Tasks.Task AddTeamTaskAsync(TeamTaskCreateModel model);

    }
}
