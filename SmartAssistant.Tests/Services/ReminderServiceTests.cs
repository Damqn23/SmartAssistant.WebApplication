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

            var mockClients = new Mock<IHubClients>();
            _mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(x => x.User(It.IsAny<string>())).Returns(_mockClientProxy.Object);

            _reminderService = new ReminderService(_mockReminderRepository.Object, _mockHubContext.Object);
        }

        [Fact]
        public async Task AddReminderAsync_ShouldAddReminder()
        {
            var reminderModel = new ReminderCreateModel
            {
                ReminderMessage = "Test Reminder",
                ReminderDate = DateTime.Now.AddMinutes(30)
            };

            var userId = "123";

            var result = await _reminderService.AddReminderAsync(reminderModel, userId);

            result.ReminderMessage.Should().Be(reminderModel.ReminderMessage);
            result.ReminderDate.Should().Be(reminderModel.ReminderDate);
            result.UserId.Should().Be(userId);

            _mockReminderRepository.Verify(r => r.AddAsync(It.IsAny<ReminderModel>()), Times.Once);
        }

        [Fact]
        public async Task DeleteReminderAsync_ShouldDeleteReminder_WhenReminderExists()
        {
            var reminderId = 1;
            var reminder = new ReminderModel { Id = reminderId };

            _mockReminderRepository.Setup(r => r.GetByIdAsync(reminderId))
                                   .ReturnsAsync(reminder);

            await _reminderService.DeleteReminderAsync(reminderId);

            _mockReminderRepository.Verify(r => r.DeleteAsync(reminder), Times.Once);
        }

        [Fact]
        public async Task GetAllRemindersAsync_ShouldReturnAllReminders()
        {
            var reminders = new List<ReminderModel>
            {
                new ReminderModel { Id = 1, ReminderMessage = "Reminder 1" },
                new ReminderModel { Id = 2, ReminderMessage = "Reminder 2" }
            };

            _mockReminderRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(reminders);

            var result = await _reminderService.GetAllRemindersAsync();

            result.Should().BeEquivalentTo(reminders);
            _mockReminderRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetReminderByIdAsync_ShouldReturnReminder_WhenExists()
        {
            var reminderId = 1;
            var reminder = new ReminderModel { Id = reminderId };

            _mockReminderRepository.Setup(r => r.GetByIdAsync(reminderId)).ReturnsAsync(reminder);

            var result = await _reminderService.GetReminderByIdAsync(reminderId);

            result.Should().BeEquivalentTo(reminder);
            _mockReminderRepository.Verify(r => r.GetByIdAsync(reminderId), Times.Once);
        }

        [Fact]
        public async Task GetRemindersDueSoonAsync_ShouldNotifyUsers()
        {
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

            _mockClientProxy
                .Setup(client => client.SendCoreAsync(
                    It.Is<string>(method => method == "ReceiveReminderNotification"),
                    It.Is<object[]>(args => args.Length == 1 && args[0].ToString().Contains("Reminder: Reminder 1")),
                    default))
                .Returns(Task.CompletedTask);

            var result = await _reminderService.GetRemindersDueSoonAsync(minutes);

            result.Should().BeEquivalentTo(reminders);
            _mockReminderRepository.Verify(r => r.GetRemindersDueSoonAsync(minutes), Times.Once);

            _mockClientProxy.Verify(client => client.SendCoreAsync(
                It.Is<string>(method => method == "ReceiveReminderNotification"),
                It.IsAny<object[]>(),
                default), Times.Once);
        }

    }
}
