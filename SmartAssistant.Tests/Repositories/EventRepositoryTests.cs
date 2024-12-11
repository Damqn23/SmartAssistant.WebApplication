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
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EventModel, SmartAssistant.WebApp.Data.Entities.Event>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task AddAsync_ShouldAddEventToDatabase()
        {
            using var context = CreateDbContext();
            var repository = new EventRepository(context, _mapper);
            var eventModel = new EventModel
            {
                EventTitle = "Test Event",
                EventDate = DateTime.Now.AddDays(1),
                UserId = "user123"
            };

            await repository.AddAsync(eventModel);

            var addedEvent = context.Events.FirstOrDefault(e => e.EventTitle == "Test Event");
            Assert.NotNull(addedEvent);
            Assert.Equal("Test Event", addedEvent.EventTitle);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectEvent()
        {
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

            var result = await repository.GetByIdAsync(eventEntity.Id);

            Assert.NotNull(result);
            Assert.Equal(eventEntity.EventTitle, result.EventTitle);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEventFromDatabase()
        {
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

            await repository.DeleteAsync(eventModel);

            var deletedEvent = context.Events.FirstOrDefault(e => e.Id == eventEntity.Id);
            Assert.Null(deletedEvent);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEvent()
        {
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

            await repository.UpdateAsync(updatedEventModel);

            var updatedEvent = context.Events.FirstOrDefault(e => e.Id == eventEntity.Id);
            Assert.NotNull(updatedEvent);
            Assert.Equal("Updated Event Title", updatedEvent.EventTitle);
        }


        [Fact]
        public async Task RemoveExpiredEventsAsync_ShouldRemoveExpiredEvents()
        {
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

            await repository.RemoveExpiredEventsAsync();

            var remainingEvents = context.Events.ToList();
            Assert.Single(remainingEvents);
            Assert.Equal("Upcoming Event", remainingEvents[0].EventTitle);
        }

        [Fact]
        public async Task GetEventsByUserIdAsync_ShouldReturnUserEvents()
        {
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

            var result = await repository.GetEventsByUserIdAsync("user1");

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, e => Assert.Equal("user1", e.UserId));
        }
    }
}
