using SmartAssistant.Shared.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces
{
    public interface ITaskRepository : IRepository<TaskModel>
    {
        Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);
    }
}
