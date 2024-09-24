using SmartAssistant.Shared.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskModel>> GetAllTasksAsync();
        Task<TaskModel> GetTaskByIdAsync(int id);
        Task AddTaskAsync(TaskCreateModel task, string userId);
        Task UpdateTaskAsync(TaskEditModel task);

        Task DeleteTaskAsync(int id);

        Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);
    }
}
