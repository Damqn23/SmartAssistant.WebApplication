using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces.Reminder;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Services
{
    public class ReminderServiceTests
    {
        private readonly Mock<IReminderRepository> _mockReminderRepository;
        private readonly Mock<IHubContext<NotificationHub>> _mockHubContext;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly ReminderService _reminderService;

        public ReminderServiceTests()
        {
            _mockReminderRepository = new Mock<IReminderRepository>();
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
            _mockClientProxy = new Mock<IClientProxy>();

            // Setup the HubContext mock
            var mockClients = new Mock<IHubClients>();
            _mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.User(It.IsAny<string>())).Returns(_mockClientProxy.Object);

            _reminderService = new ReminderService(_mockReminderRepository.Object, _mockHubContext.Object);
        }

        [Fact]
        public async Task AddReminderAsync_ShouldAddReminder()
        {
            // Arrange
            var reminderModel = new ReminderCreateModel
            {
                ReminderMessage = "Test Reminder",
                ReminderDate = DateTime.Now.AddMinutes(30)
            };

            var userId = "123";

            // Act
            var result = await _reminderService.AddReminderAsync(reminderModel, userId);

            // Assert
            result.ReminderMessage.Should().Be(reminderModel.ReminderMessage);
            result.ReminderDate.Should().Be(reminderModel.ReminderDate);
            result.UserId.Should().Be(userId);

            _mockReminderRepository.Verify(r => r.AddAsync(It.IsAny<ReminderModel>()), Times.Once);
        }

        [Fact]
        public async Task DeleteReminderAsync_ShouldDeleteReminder_WhenReminderExists()
        {
            // Arrange
            var reminderId = 1;
            var reminder = new ReminderModel { Id = reminderId };

            _mockReminderRepository.Setup(r => r.GetByIdAsync(reminderId))
                                   .ReturnsAsync(reminder);

            // Act
            await _reminderService.DeleteReminderAsync(reminderId);

            // Assert
            _mockReminderRepository.Verify(r => r.DeleteAsync(reminder), Times.Once);
        }

        [Fact]
        public async Task GetAllRemindersAsync_ShouldReturnAllReminders()
        {
            // Arrange
            var reminders = new List<ReminderModel>
            {
                new ReminderModel { Id = 1, ReminderMessage = "Reminder 1" },
                new ReminderModel { Id = 2, ReminderMessage = "Reminder 2" }
            };

            _mockReminderRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(reminders);

            // Act
            var result = await _reminderService.GetAllRemindersAsync();

            // Assert
            result.Should().BeEquivalentTo(reminders);
            _mockReminderRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetReminderByIdAsync_ShouldReturnReminder_WhenExists()
        {
            // Arrange
            var reminderId = 1;
            var reminder = new ReminderModel { Id = reminderId };

            _mockReminderRepository.Setup(r => r.GetByIdAsync(reminderId)).ReturnsAsync(reminder);

            // Act
            var result = await _reminderService.GetReminderByIdAsync(reminderId);

            // Assert
            result.Should().BeEquivalentTo(reminder);
            _mockReminderRepository.Verify(r => r.GetByIdAsync(reminderId), Times.Once);
        }

        [Fact]
        public async Task GetRemindersDueSoonAsync_ShouldNotifyUsers()
        {
            // Arrange
            var minutes = 10;
            var reminders = new List<ReminderModel>
    {
        new ReminderModel
        {
            Id = 1,
            ReminderMessage = "Reminder 1",
            ReminderDate = DateTime.Now.AddMinutes(5),
            UserId = "123"
        }
    };

            _mockReminderRepository.Setup(r => r.GetRemindersDueSoonAsync(minutes)).ReturnsAsync(reminders);

            // Mock the ClientProxy's SendAsync behavior
            _mockClientProxy
                .Setup(client => client.SendCoreAsync(
                    It.Is<string>(method => method == "ReceiveReminderNotification"),
                    It.Is<object[]>(args => args.Length == 1 && args[0].ToString().Contains("Reminder: Reminder 1")),
                    default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _reminderService.GetRemindersDueSoonAsync(minutes);

            // Assert
            result.Should().BeEquivalentTo(reminders);
            _mockReminderRepository.Verify(r => r.GetRemindersDueSoonAsync(minutes), Times.Once);

            // Verify that SendCoreAsync was called on the client proxy
            _mockClientProxy.Verify(client => client.SendCoreAsync(
                It.Is<string>(method => method == "ReceiveReminderNotification"),
                It.IsAny<object[]>(),
                default), Times.Once);
        }

    }
}
