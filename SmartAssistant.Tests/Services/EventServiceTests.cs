using FluentAssertions;
using Moq;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Services.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _eventService = new EventService(_mockEventRepository.Object);
        }

        [Fact]
        public async Task AddEventAsync_ShouldAddEvent()
        {
            var eventCreateModel = new EventCreateModel
            {
                EventTitle = "Meeting",
                EventDate = DateTime.Now.AddDays(1)
            };
            var userId = "user123";

            await _eventService.AddEventAsync(eventCreateModel, userId);

            _mockEventRepository.Verify(r => r.AddAsync(It.Is<EventModel>(
                e => e.EventTitle == eventCreateModel.EventTitle &&
                     e.EventDate == eventCreateModel.EventDate &&
                     e.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task DeleteEventAsync_ShouldDeleteEvent_WhenEventExists()
        {
            var eventId = 1;
            var eventModel = new EventModel { Id = eventId };

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(eventModel);

            await _eventService.DeleteEventAsync(eventId);

            _mockEventRepository.Verify(r => r.DeleteAsync(eventModel), Times.Once);
        }

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            var events = new List<EventModel>
            {
                new EventModel { Id = 1, EventTitle = "Event 1" },
                new EventModel { Id = 2, EventTitle = "Event 2" }
            };

            _mockEventRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

            var result = await _eventService.GetAllEventsAsync();

            result.Should().BeEquivalentTo(events);
            _mockEventRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnEvent_WhenExists()
        {
            var eventId = 1;
            var eventModel = new EventModel { Id = eventId };

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(eventModel);

            var result = await _eventService.GetEventByIdAsync(eventId);

            result.Should().BeEquivalentTo(eventModel);
            _mockEventRepository.Verify(r => r.GetByIdAsync(eventId), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_ShouldUpdateEvent_WhenEventExists()
        {
            var eventEditModel = new EventEditModel
            {
                Id = 1,
                EventTitle = "Updated Event",
                EventDate = DateTime.Now.AddDays(2)
            };
            var existingEvent = new EventModel { Id = eventEditModel.Id };

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventEditModel.Id)).ReturnsAsync(existingEvent);

            await _eventService.UpdateEventAsync(eventEditModel);

            _mockEventRepository.Verify(r => r.UpdateAsync(It.Is<EventModel>(
                e => e.Id == eventEditModel.Id &&
                     e.EventTitle == eventEditModel.EventTitle &&
                     e.EventDate == eventEditModel.EventDate)), Times.Once);
        }

        [Fact]
        public async Task RemoveExpiredEventsAsync_ShouldRemoveExpiredEvents()
        {
            await _eventService.RemoveExpiredEventsAsync();

            _mockEventRepository.Verify(r => r.RemoveExpiredEventsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEventsBySearchQueryAsync_ShouldReturnMatchingEvents()
        {
            var searchQuery = "Meeting";
            var events = new List<EventModel>
            {
                new EventModel { Id = 1, EventTitle = "Meeting with team" },
                new EventModel { Id = 2, EventTitle = "Workshop" }
            };

            _mockEventRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

            var result = await _eventService.GetEventsBySearchQueryAsync(searchQuery);

            result.Should().HaveCount(1);
            result.First().EventTitle.Should().Contain(searchQuery, StringComparison.OrdinalIgnoreCase.ToString());
        }

        [Fact]
        public async Task GetEventsByTeamIdAsync_ShouldReturnTeamEvents()
        {
            var teamId = 1; // This will still represent the team filter
            var events = new List<EventModel>
    {
        new EventModel { Id = 1, EventTitle = "Team Meeting", UserId = "user1" },
        new EventModel { Id = 2, EventTitle = "Project Kickoff", UserId = "user2" }
    };

            _mockEventRepository.Setup(r => r.GetEventsByTeamIdAsync(teamId)).ReturnsAsync(events);

            var result = await _eventService.GetEventsByTeamIdAsync(teamId);

            result.Should().BeEquivalentTo(events);
            _mockEventRepository.Verify(r => r.GetEventsByTeamIdAsync(teamId), Times.Once);
        }
    }
}
