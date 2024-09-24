using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Interfaces;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.WebApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public TaskRepository(ApplicationDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async Task AddAsync(TaskModel entity)
        {
            var task = mapper.Map<WebApp.Data.Entities.Task>(entity);
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskModel entity)
        {
            var task = await context.Tasks.FindAsync(entity.Id);
            if (task != null)
            {
                context.Tasks.Remove(task);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TaskModel>> GetAllAsync()
        {
            var tasks = await context.Tasks.ToListAsync();
            return mapper.Map<List<TaskModel>>(tasks);
        }

        public async Task<TaskModel> GetByIdAsync(int id)
        {
            var task = await context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            return mapper.Map<TaskModel>(task);
        }

        public async Task<List<TaskModel>> GetTasksByUserIdAsync(string userId)
        {
            var tasks = await context.Tasks.Where(t => t.UserId == userId).ToListAsync();
            return mapper.Map<List<TaskModel>>(tasks);
        }

        public async Task UpdateAsync(TaskModel taskModel)
        {
            var task = await context.Tasks.FindAsync(taskModel.Id);

            if (task != null)
            {
                task.Description = taskModel.Description;
                task.DueDate = taskModel.DueDate;
                task.EstimatedTimeToComplete = taskModel.EstimatedTimeToComplete;
                task.Priority = taskModel.Priority;
                task.IsCompleted = taskModel.IsCompleted;

                // Preserve UserId from the original task
                task.UserId = taskModel.UserId;

                await context.SaveChangesAsync();
            }
        }
    }
}
