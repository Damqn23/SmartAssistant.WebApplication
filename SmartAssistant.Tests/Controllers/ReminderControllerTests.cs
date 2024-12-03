using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SmartAssistant.Shared.Hubs;
using SmartAssistant.Shared.Interfaces.Reminder;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Controllers
{
    public class ReminderControllerTests
    {
        private readonly Mock<IReminderService> _mockReminderService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHubContext<NotificationHub>> _mockHubContext;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly ReminderController _controller;

        public ReminderControllerTests()
        {
            _mockReminderService = new Mock<IReminderService>();
            _mockMapper = new Mock<IMapper>();
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
            _mockClientProxy = new Mock<IClientProxy>();

            var userContext = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }));

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_mockClientProxy.Object);
            _mockHubContext.Setup(h => h.Clients).Returns(clientsMock.Object);

            _controller = new ReminderController(
                _mockReminderService.Object,
                _mockMapper.Object,
                _mockHubContext.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = userContext }
                }
            };
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithReminders()
        {
            // Arrange
            var reminders = new List<ReminderModel> { new ReminderModel { Id = 1, ReminderMessage = "Test Reminder" } };
            _mockReminderService.Setup(s => s.GetRemindersByUserIdAsync("test-user-id")).ReturnsAsync(reminders);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ReminderModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ShouldReturnViewWhenReminderExists()
        {
            // Arrange
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder" };
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(reminder, viewResult.Model);
        }

        [Fact]
        public async Task Details_ShouldReturnNotFoundWhenReminderDoesNotExist()
        {
            // Arrange
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync((ReminderModel)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldRedirectToIndexOnValidModel()
        {
            // Arrange
            var reminderCreateModel = new ReminderCreateModel { ReminderMessage = "New Reminder" };
            var reminderModel = new ReminderModel { Id = 1, ReminderMessage = "New Reminder" };

            _mockReminderService.Setup(s => s.AddReminderAsync(reminderCreateModel, "test-user-id"))
                .ReturnsAsync(reminderModel);

            // Act
            var result = await _controller.Create(reminderCreateModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_ShouldReturnViewOnInvalidModelState()
        {
            // Arrange
            _controller.ModelState.AddModelError("ReminderMessage", "Required");

            var reminderCreateModel = new ReminderCreateModel();

            // Act
            var result = await _controller.Create(reminderCreateModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(reminderCreateModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ShouldReturnViewWhenReminderExists()
        {
            // Arrange
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder" };
            var editModel = new ReminderEditModel { Id = 1, ReminderMessage = "Test Reminder" };

            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);
            _mockMapper.Setup(m => m.Map<ReminderEditModel>(reminder)).Returns(editModel);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ShouldReturnNotFoundWhenReminderDoesNotExist()
        {
            // Arrange
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync((ReminderModel)null);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnViewWhenReminderExists()
        {
            // Arrange
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder" };
            var deleteModel = new ReminderDeleteModel { Id = 1, ReminderMessage = "Test Reminder" };

            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);
            _mockMapper.Setup(m => m.Map<ReminderDeleteModel>(reminder)).Returns(deleteModel);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(deleteModel, viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldRedirectToIndex()
        {
            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task UpdateReminderStatus_ShouldRedirectToIndex()
        {
            // Act
            var result = await _controller.UpdateReminderStatus(1, true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task GetUpcomingReminders_ShouldReturnOk()
        {
            // Act
            var result = await _controller.GetUpcomingReminders();

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
