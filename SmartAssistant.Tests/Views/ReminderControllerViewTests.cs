using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartAssistant.Shared.Interfaces.Reminder;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Views
{
    public class ReminderControllerViewTests
    {
        private readonly Mock<IReminderService> _mockReminderService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ReminderController _controller;

        public ReminderControllerViewTests()
        {
            _mockReminderService = new Mock<IReminderService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ReminderController(_mockReminderService.Object, _mockMapper.Object, null);
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithReminders()
        {
            var reminders = new List<ReminderModel>
    {
        new ReminderModel { Id = 1, ReminderMessage = "Test Reminder 1", ReminderDate = DateTime.Now },
        new ReminderModel { Id = 2, ReminderMessage = "Test Reminder 2", ReminderDate = DateTime.Now.AddDays(1) }
    };

            _mockReminderService.Setup(s => s.GetRemindersByUserIdAsync(It.IsAny<string>())).ReturnsAsync(reminders);

            var mockHttpContext = new Mock<HttpContext>();
            var mockUser = new Mock<ClaimsPrincipal>();
            mockUser.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "test-user-id"));
            mockHttpContext.Setup(c => c.User).Returns(mockUser.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ReminderModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_Get_ShouldReturnView()
        {
            var result = _controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_ShouldReturnViewWithEditModel()
        {
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder", ReminderDate = DateTime.Now };
            var editModel = new ReminderEditModel { Id = 1, ReminderMessage = "Test Reminder", ReminderDate = DateTime.Now };
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);
            _mockMapper.Setup(m => m.Map<ReminderEditModel>(reminder)).Returns(editModel);

            var result = await _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReminderEditModel>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task Delete_Get_ShouldReturnViewWithDeleteModel()
        {
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder", ReminderDate = DateTime.Now };
            var deleteModel = new ReminderDeleteModel { Id = 1, ReminderMessage = "Test Reminder", ReminderDate = DateTime.Now };
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);
            _mockMapper.Setup(m => m.Map<ReminderDeleteModel>(reminder)).Returns(deleteModel);

            var result = await _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReminderDeleteModel>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldRedirectToIndex()
        {
            _mockReminderService.Setup(s => s.DeleteReminderAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Details_ShouldReturnViewWithReminder()
        {
            var reminder = new ReminderModel { Id = 1, ReminderMessage = "Test Reminder", ReminderDate = DateTime.Now };
            _mockReminderService.Setup(s => s.GetReminderByIdAsync(1)).ReturnsAsync(reminder);

            var result = await _controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReminderModel>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }
    }
}
