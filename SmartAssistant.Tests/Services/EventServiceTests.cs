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
            // Arrange
            var eventCreateModel = new EventCreateModel
            {
                EventTitle = "Meeting",
                EventDate = DateTime.Now.AddDays(1)
            };
            var userId = "user123";

            // Act
            await _eventService.AddEventAsync(eventCreateModel, userId);

            // Assert
            _mockEventRepository.Verify(r => r.AddAsync(It.Is<EventModel>(
                e => e.EventTitle == eventCreateModel.EventTitle &&
                     e.EventDate == eventCreateModel.EventDate &&
                     e.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task DeleteEventAsync_ShouldDeleteEvent_WhenEventExists()
        {
            // Arrange
            var eventId = 1;
            var eventModel = new EventModel { Id = eventId };

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(eventModel);

            // Act
            await _eventService.DeleteEventAsync(eventId);

            // Assert
            _mockEventRepository.Verify(r => r.DeleteAsync(eventModel), Times.Once);
        }

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            // Arrange
            var events = new List<EventModel>
            {
                new EventModel { Id = 1, EventTitle = "Event 1" },
                new EventModel { Id = 2, EventTitle = "Event 2" }
            };

            _mockEventRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            result.Should().BeEquivalentTo(events);
            _mockEventRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnEvent_WhenExists()
        {
            // Arrange
            var eventId = 1;
            var eventModel = new EventModel { Id = eventId };

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(eventModel);

            // Act
            var result = await _eventService.GetEventByIdAsync(eventId);

            // Assert
            result.Should().BeEquivalentTo(eventModel);
            _mockEventRepository.Verify(r => r.GetByIdAsync(eventId), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_ShouldUpdateEvent_WhenEventExists()
        {
            // Arrange
            var eventEditModel = new EventEditModel
            {
                Id = 1,
                EventTitle = "Updated Event",
                EventDate = DateTime.Now.AddDays(2)
            };
            var existingEvent = new EventModel { Id = eventEditModel.Id };

            _mockEventRepository.Setup(r => r.GetByIdAsync(eventEditModel.Id)).ReturnsAsync(existingEvent);

            // Act
            await _eventService.UpdateEventAsync(eventEditModel);

            // Assert
            _mockEventRepository.Verify(r => r.UpdateAsync(It.Is<EventModel>(
                e => e.Id == eventEditModel.Id &&
                     e.EventTitle == eventEditModel.EventTitle &&
                     e.EventDate == eventEditModel.EventDate)), Times.Once);
        }

        [Fact]
        public async Task RemoveExpiredEventsAsync_ShouldRemoveExpiredEvents()
        {
            // Act
            await _eventService.RemoveExpiredEventsAsync();

            // Assert
            _mockEventRepository.Verify(r => r.RemoveExpiredEventsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEventsBySearchQueryAsync_ShouldReturnMatchingEvents()
        {
            // Arrange
            var searchQuery = "Meeting";
            var events = new List<EventModel>
            {
                new EventModel { Id = 1, EventTitle = "Meeting with team" },
                new EventModel { Id = 2, EventTitle = "Workshop" }
            };

            _mockEventRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

            // Act
            var result = await _eventService.GetEventsBySearchQueryAsync(searchQuery);

            // Assert
            result.Should().HaveCount(1);
            result.First().EventTitle.Should().Contain(searchQuery, StringComparison.OrdinalIgnoreCase.ToString());
        }

        [Fact]
        public async Task GetEventsByTeamIdAsync_ShouldReturnTeamEvents()
        {
            // Arrange
            var teamId = 1; // This will still represent the team filter
            var events = new List<EventModel>
    {
        new EventModel { Id = 1, EventTitle = "Team Meeting", UserId = "user1" },
        new EventModel { Id = 2, EventTitle = "Project Kickoff", UserId = "user2" }
    };

            // Mock the repository method to return events
            _mockEventRepository.Setup(r => r.GetEventsByTeamIdAsync(teamId)).ReturnsAsync(events);

            // Act
            var result = await _eventService.GetEventsByTeamIdAsync(teamId);

            // Assert
            result.Should().BeEquivalentTo(events);
            _mockEventRepository.Verify(r => r.GetEventsByTeamIdAsync(teamId), Times.Once);
        }
    }
}
