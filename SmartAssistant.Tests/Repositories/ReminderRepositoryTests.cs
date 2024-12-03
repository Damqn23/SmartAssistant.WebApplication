using AutoMapper;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Repositories
{
    public class ReminderRepositoryTests : RepositoryTestBase
    {
        private readonly IMapper _mapper;

        public ReminderRepositoryTests()
        {
            // Initialize AutoMapper (mock or real configuration depending on setup)
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReminderModel, SmartAssistant.WebApp.Data.Entities.Reminder>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task AddAsync_ShouldAddReminderToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new ReminderRepository(context, _mapper);
            var reminder = new ReminderModel
            {
                ReminderMessage = "Test Reminder",
                ReminderDate = DateTime.Now.AddMinutes(10),
                UserId = "user123"
            };

            // Act
            await repository.AddAsync(reminder);

            // Assert
            var addedReminder = context.Reminders.FirstOrDefault(r => r.ReminderMessage == "Test Reminder");
            Assert.NotNull(addedReminder);
            Assert.Equal("Test Reminder", addedReminder.ReminderMessage);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectReminder()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new ReminderRepository(context, _mapper);
            var reminderEntity = new SmartAssistant.WebApp.Data.Entities.Reminder
            {
                ReminderMessage = "Test Reminder",
                ReminderDate = DateTime.Now.AddMinutes(10),
                UserId = "user123"
            };
            context.Reminders.Add(reminderEntity);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(reminderEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reminderEntity.ReminderMessage, result.ReminderMessage);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveReminderFromDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new ReminderRepository(context, _mapper);
            var reminderEntity = new SmartAssistant.WebApp.Data.Entities.Reminder
            {
                ReminderMessage = "Reminder to Delete",
                ReminderDate = DateTime.Now.AddMinutes(10),
                UserId = "user123"
            };
            context.Reminders.Add(reminderEntity);
            await context.SaveChangesAsync();

            var reminderModel = _mapper.Map<ReminderModel>(reminderEntity);

            // Act
            await repository.DeleteAsync(reminderModel);

            // Assert
            var deletedReminder = context.Reminders.FirstOrDefault(r => r.Id == reminderEntity.Id);
            Assert.Null(deletedReminder);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateReminder()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new ReminderRepository(context, _mapper);

            var reminderEntity = new SmartAssistant.WebApp.Data.Entities.Reminder
            {
                ReminderMessage = "Old Reminder Message",
                ReminderDate = DateTime.Now.AddMinutes(10),
                UserId = "user123"
            };
            context.Reminders.Add(reminderEntity);
            await context.SaveChangesAsync();

            var updatedReminderModel = _mapper.Map<ReminderModel>(reminderEntity);
            updatedReminderModel.ReminderMessage = "Updated Reminder Message";

            // Act
            await repository.UpdateAsync(updatedReminderModel);

            // Assert
            var updatedReminder = context.Reminders.FirstOrDefault(r => r.Id == reminderEntity.Id);
            Assert.NotNull(updatedReminder);
            Assert.Equal("Updated Reminder Message", updatedReminder.ReminderMessage);
        }

        [Fact]
        public async Task GetRemindersByUserIdAsync_ShouldReturnUserReminders()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new ReminderRepository(context, _mapper);
            var reminders = new List<SmartAssistant.WebApp.Data.Entities.Reminder>
        {
            new SmartAssistant.WebApp.Data.Entities.Reminder { ReminderMessage = "User Reminder 1", UserId = "user1", ReminderDate = DateTime.Now },
            new SmartAssistant.WebApp.Data.Entities.Reminder { ReminderMessage = "User Reminder 2", UserId = "user1", ReminderDate = DateTime.Now },
            new SmartAssistant.WebApp.Data.Entities.Reminder { ReminderMessage = "Other User Reminder", UserId = "user2", ReminderDate = DateTime.Now }
        };

            context.Reminders.AddRange(reminders);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetRemindersByUserIdAsync("user1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal("user1", r.UserId));
        }

        [Fact]
        public async Task GetRemindersDueSoonAsync_ShouldReturnUpcomingReminders()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new ReminderRepository(context, _mapper);
            var currentTime = DateTime.Now;
            var reminders = new List<SmartAssistant.WebApp.Data.Entities.Reminder>
        {
            new SmartAssistant.WebApp.Data.Entities.Reminder { ReminderMessage = "Reminder 1", ReminderDate = currentTime.AddMinutes(5), UserId = "user1" },
            new SmartAssistant.WebApp.Data.Entities.Reminder { ReminderMessage = "Reminder 2", ReminderDate = currentTime.AddMinutes(15), UserId = "user2" },
            new SmartAssistant.WebApp.Data.Entities.Reminder { ReminderMessage = "Reminder 3", ReminderDate = currentTime.AddMinutes(25), UserId = "user3" }
        };

            context.Reminders.AddRange(reminders);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetRemindersDueSoonAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only one reminder is due within the next 10 minutes
            Assert.Equal("Reminder 1", result[0].ReminderMessage);
        }
    }
}
