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
            var reminders = new List<ReminderModel> { new ReminderModel { Id = 1, ReminderMessage = "Test Reminder" } };
            _mockReminderService.Setup(s => s.GetRemindersByUserIdAsync("test-user-id")).ReturnsAsync(reminders);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ReminderModel>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ShouldReturnViewWhenReminderExists()
        {
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder" };
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);

            var result = await _controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(reminder, viewResult.Model);
        }

        [Fact]
        public async Task Details_ShouldReturnNotFoundWhenReminderDoesNotExist()
        {
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync((ReminderModel)null);

            var result = await _controller.Details(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldRedirectToIndexOnValidModel()
        {
            var reminderCreateModel = new ReminderCreateModel { ReminderMessage = "New Reminder" };
            var reminderModel = new ReminderModel { Id = 1, ReminderMessage = "New Reminder" };

            _mockReminderService.Setup(s => s.AddReminderAsync(reminderCreateModel, "test-user-id"))
                .ReturnsAsync(reminderModel);

            var result = await _controller.Create(reminderCreateModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_ShouldReturnViewOnInvalidModelState()
        {
            _controller.ModelState.AddModelError("ReminderMessage", "Required");

            var reminderCreateModel = new ReminderCreateModel();

            var result = await _controller.Create(reminderCreateModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(reminderCreateModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ShouldReturnViewWhenReminderExists()
        {
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder" };
            var editModel = new ReminderEditModel { Id = 1, ReminderMessage = "Test Reminder" };

            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);
            _mockMapper.Setup(m => m.Map<ReminderEditModel>(reminder)).Returns(editModel);

            var result = await _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(editModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ShouldReturnNotFoundWhenReminderDoesNotExist()
        {
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync((ReminderModel)null);

            var result = await _controller.Edit(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnViewWhenReminderExists()
        {
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder" };
            var deleteModel = new ReminderDeleteModel { Id = 1, ReminderMessage = "Test Reminder" };

            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);
            _mockMapper.Setup(m => m.Map<ReminderDeleteModel>(reminder)).Returns(deleteModel);

            var result = await _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(deleteModel, viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldRedirectToIndex()
        {
            var result = await _controller.DeleteConfirmed(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task UpdateReminderStatus_ShouldRedirectToIndex()
        {
            var result = await _controller.UpdateReminderStatus(1, true);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task GetUpcomingReminders_ShouldReturnOk()
        {
            var result = await _controller.GetUpcomingReminders();

            Assert.IsType<OkResult>(result);
        }
    }
}
