using AutoMapper;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Repositories.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Repositories
{
    public class EventRepositoryTests : RepositoryTestBase
    {
        private readonly IMapper _mapper;

        public EventRepositoryTests()
        {
            // Initialize AutoMapper (mock or real configuration depending on setup)
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventModel, SmartAssistant.WebApp.Data.Entities.Event>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task AddAsync_ShouldAddEventToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new EventRepository(context, _mapper);
            var eventModel = new EventModel
            {
                EventTitle = "Test Event",
                EventDate = DateTime.Now.AddDays(1),
                UserId = "user123"
            };

            // Act
            await repository.AddAsync(eventModel);

            // Assert
            var addedEvent = context.Events.FirstOrDefault(e => e.EventTitle == "Test Event");
            Assert.NotNull(addedEvent);
            Assert.Equal("Test Event", addedEvent.EventTitle);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectEvent()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new EventRepository(context, _mapper);
            var eventEntity = new SmartAssistant.WebApp.Data.Entities.Event
            {
                EventTitle = "Test Event",
                EventDate = DateTime.Now.AddDays(1),
                UserId = "user123"
            };
            context.Events.Add(eventEntity);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(eventEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventEntity.EventTitle, result.EventTitle);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEventFromDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new EventRepository(context, _mapper);
            var eventEntity = new SmartAssistant.WebApp.Data.Entities.Event
            {
                EventTitle = "Event to Delete",
                EventDate = DateTime.Now.AddDays(1),
                UserId = "user123"
            };
            context.Events.Add(eventEntity);
            await context.SaveChangesAsync();

            var eventModel = _mapper.Map<EventModel>(eventEntity);

            // Act
            await repository.DeleteAsync(eventModel);

            // Assert
            var deletedEvent = context.Events.FirstOrDefault(e => e.Id == eventEntity.Id);
            Assert.Null(deletedEvent);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEvent()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new EventRepository(context, _mapper);
            var eventEntity = new SmartAssistant.WebApp.Data.Entities.Event
            {
                EventTitle = "Old Event Title",
                EventDate = DateTime.Now.AddDays(1),
                UserId = "user123"
            };
            context.Events.Add(eventEntity);
            await context.SaveChangesAsync();

            var updatedEventModel = _mapper.Map<EventModel>(eventEntity);
            updatedEventModel.EventTitle = "Updated Event Title";

            // Act
            await repository.UpdateAsync(updatedEventModel);

            // Assert
            var updatedEvent = context.Events.FirstOrDefault(e => e.Id == eventEntity.Id);
            Assert.NotNull(updatedEvent);
            Assert.Equal("Updated Event Title", updatedEvent.EventTitle);
        }


        [Fact]
        public async Task RemoveExpiredEventsAsync_ShouldRemoveExpiredEvents()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new EventRepository(context, _mapper);
            var currentTime = DateTime.Now;
            var events = new List<SmartAssistant.WebApp.Data.Entities.Event>
        {
            new SmartAssistant.WebApp.Data.Entities.Event { EventTitle = "Upcoming Event", EventDate = currentTime.AddDays(1), UserId = "user123" },
            new SmartAssistant.WebApp.Data.Entities.Event { EventTitle = "Expired Event", EventDate = currentTime.AddDays(-1), UserId = "user123" }
        };

            context.Events.AddRange(events);
            await context.SaveChangesAsync();

            // Act
            await repository.RemoveExpiredEventsAsync();

            // Assert
            var remainingEvents = context.Events.ToList();
            Assert.Single(remainingEvents);
            Assert.Equal("Upcoming Event", remainingEvents[0].EventTitle);
        }

        [Fact]
        public async Task GetEventsByUserIdAsync_ShouldReturnUserEvents()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new EventRepository(context, _mapper);
            var userEvents = new List<SmartAssistant.WebApp.Data.Entities.Event>
        {
            new SmartAssistant.WebApp.Data.Entities.Event { EventTitle = "User Event 1", UserId = "user1", EventDate = DateTime.Now },
            new SmartAssistant.WebApp.Data.Entities.Event { EventTitle = "User Event 2", UserId = "user1", EventDate = DateTime.Now },
            new SmartAssistant.WebApp.Data.Entities.Event { EventTitle = "Other User Event", UserId = "user2", EventDate = DateTime.Now }
        };

            context.Events.AddRange(userEvents);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetEventsByUserIdAsync("user1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, e => Assert.Equal("user1", e.UserId));
        }
    }
}
