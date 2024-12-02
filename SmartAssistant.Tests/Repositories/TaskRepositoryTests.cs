using AutoMapper;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Repositories
{
    public class TaskRepositoryTests : RepositoryTestBase
    {
        private readonly IMapper _mapper;

        public TaskRepositoryTests()
        {
            // Initialize AutoMapper (mock or real configuration depending on setup)
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TaskModel, SmartAssistant.WebApp.Data.Entities.Task>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task AddAsync_ShouldAddTaskToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new TaskRepository(context, _mapper);
            var task = new TaskModel
            {
                Description = "Test Task",
                DueDate = DateTime.Now.AddDays(1),
                UserId = "user123",
                Priority = 1
            };

            // Act
            await repository.AddAsync(task);

            // Assert
            var addedTask = context.Tasks.FirstOrDefault(t => t.Description == "Test Task");
            Assert.NotNull(addedTask);
            Assert.Equal("Test Task", addedTask.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectTask()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new TaskRepository(context, _mapper);
            var taskEntity = new SmartAssistant.WebApp.Data.Entities.Task
            {
                Description = "Test Task",
                DueDate = DateTime.Now.AddDays(1),
                UserId = "user123",
                Priority = 1
            };
            context.Tasks.Add(taskEntity);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(taskEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskEntity.Description, result.Description);
        }


        [Fact]
        public async Task DeleteAsync_ShouldRemoveTaskFromDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new TaskRepository(context, _mapper);
            var taskEntity = new SmartAssistant.WebApp.Data.Entities.Task
            {
                Description = "Task to Delete",
                DueDate = DateTime.Now.AddDays(1),
                UserId = "user123",
                Priority = 1
            };
            context.Tasks.Add(taskEntity);
            await context.SaveChangesAsync();

            // Map the entity back to TaskModel for deletion
            var taskModel = _mapper.Map<TaskModel>(taskEntity);

            // Act
            await repository.DeleteAsync(taskModel);

            // Assert
            var deletedTask = context.Tasks.FirstOrDefault(t => t.Id == taskEntity.Id);
            Assert.Null(deletedTask);
        }


        [Fact]
        public async Task GetTasksByUserIdAsync_ShouldReturnTasksForUser()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new TaskRepository(context, _mapper);
            var userTasks = new List<TaskModel>
        {
            new TaskModel { Description = "User Task 1", UserId = "user1", DueDate = DateTime.Now, Priority = 1 },
            new TaskModel { Description = "User Task 2", UserId = "user1", DueDate = DateTime.Now, Priority = 2 },
            new TaskModel { Description = "Other Task", UserId = "user2", DueDate = DateTime.Now, Priority = 1 }
        };

            foreach (var task in userTasks)
            {
                context.Tasks.Add(_mapper.Map<SmartAssistant.WebApp.Data.Entities.Task>(task));
            }
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTasksByUserIdAsync("user1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal("user1", t.UserId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new TaskRepository(context, _mapper);

            var taskEntity = new SmartAssistant.WebApp.Data.Entities.Task
            {
                Description = "Old Task Description",
                DueDate = DateTime.Now.AddDays(1),
                UserId = "user123",
                Priority = 1
            };
            context.Tasks.Add(taskEntity);
            await context.SaveChangesAsync();

            // Create a TaskModel with the same ID to update the task
            var updatedTaskModel = new TaskModel
            {
                Id = taskEntity.Id, // Use the same ID as the saved task
                Description = "Updated Task Description",
                DueDate = taskEntity.DueDate,
                UserId = taskEntity.UserId,
                Priority = taskEntity.Priority,
                IsCompleted = taskEntity.IsCompleted
            };

            // Act
            await repository.UpdateAsync(updatedTaskModel);

            // Assert
            var updatedTask = context.Tasks.FirstOrDefault(t => t.Id == taskEntity.Id);
            Assert.NotNull(updatedTask);
            Assert.Equal("Updated Task Description", updatedTask.Description);
        }

    }
}
