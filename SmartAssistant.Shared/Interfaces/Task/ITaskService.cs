using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Task
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskModel>> GetAllTasksAsync();
        Task<TaskModel> GetTaskByIdAsync(int id);
        System.Threading.Tasks.Task AddTaskAsync(TaskCreateModel task, string userId);
        System.Threading.Tasks.Task UpdateTaskAsync(TaskEditModel task);

        System.Threading.Tasks.Task DeleteTaskAsync(int id);

        Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);

        System.Threading.Tasks.Task RemoveExpiredTasksAsync();

        Task<List<TaskModel>> GetTasksByTeamIdAsync(int teamId);

        System.Threading.Tasks.Task AddTeamTaskAsync(TeamTaskCreateModel model, string userId);

        Task<List<TaskModel>> GetTasksBySearchQueryAsync(string searchQuery);

    }
}
