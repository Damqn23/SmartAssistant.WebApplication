using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
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

        public async Task AddTeamTaskAsync(TeamTaskCreateModel entity)
        {
            var task = new WebApp.Data.Entities.Task
            {
                TeamId = entity.TeamId,
                Description = entity.Description,
                DueDate = entity.DueDate,
                EstimatedTimeToComplete = entity.EstimatedTimeToComplete,
                Priority = entity.Priority,
                UserId = entity.AssignedUserId             };
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskModel entity)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == entity.Id);
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
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            return mapper.Map<TaskModel>(task);
        }

        public async Task<List<TaskModel>> GetTasksByUserIdAsync(string userId)
        {
            var tasks = await context.Tasks.Where(t => t.UserId == userId).ToListAsync();
            return mapper.Map<List<TaskModel>>(tasks);
        }

        public async Task UpdateAsync(TaskModel taskModel)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == taskModel.Id);

            if (task != null)
            {
                task.Description = taskModel.Description;
                task.DueDate = taskModel.DueDate;
                task.EstimatedTimeToComplete = taskModel.EstimatedTimeToComplete;
                task.Priority = taskModel.Priority;
                task.IsCompleted = taskModel.IsCompleted;

                task.UserId = taskModel.UserId;

                context.Entry(task).State = EntityState.Modified;

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<TaskModel>> GetTasksByTeamIdAsync(int teamId)
        {
            var tasks = await context.Tasks
                .Include(t => t.User)                 .Where(t => t.TeamId == teamId)
                .ToListAsync();
            return mapper.Map<List<TaskModel>>(tasks);
        }

    }
}
