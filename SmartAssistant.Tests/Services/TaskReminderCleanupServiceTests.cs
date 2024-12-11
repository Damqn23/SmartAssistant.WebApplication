using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartAssistant.Shared.Interfaces.Reminder;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Services.CleanUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Services
{
    public class TaskReminderCleanupServiceTests
    {
        private readonly Mock<IServiceProvider> _mockRootServiceProvider;
        private readonly Mock<IServiceProvider> _mockScopedServiceProvider;
        private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
        private readonly Mock<IServiceScope> _mockServiceScope;
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IReminderService> _mockReminderService;
        private readonly TaskReminderCleanupService _taskReminderCleanupService;

        public TaskReminderCleanupServiceTests()
        {
            _mockRootServiceProvider = new Mock<IServiceProvider>();
            _mockScopedServiceProvider = new Mock<IServiceProvider>();
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockServiceScope = new Mock<IServiceScope>();
            _mockTaskService = new Mock<ITaskService>();
            _mockReminderService = new Mock<IReminderService>();

            _mockRootServiceProvider
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(_mockScopeFactory.Object);

            _mockScopeFactory
                .Setup(sf => sf.CreateScope())
                .Returns(_mockServiceScope.Object);

            _mockServiceScope
                .Setup(s => s.ServiceProvider)
                .Returns(_mockScopedServiceProvider.Object);

            _mockScopedServiceProvider
                .Setup(sp => sp.GetService(typeof(ITaskService)))
                .Returns(_mockTaskService.Object);

            _mockScopedServiceProvider
                .Setup(sp => sp.GetService(typeof(IReminderService)))
                .Returns(_mockReminderService.Object);

            _taskReminderCleanupService = new TaskReminderCleanupService(_mockRootServiceProvider.Object);
        }

        [Fact]
        public async Task CleanupExpiredTaskAndReminder_ShouldCallServices()
        {
            var cancellationToken = new CancellationTokenSource().Token;

            await _taskReminderCleanupService.StartAsync(cancellationToken);

            _mockTaskService.Verify(ts => ts.RemoveExpiredTasksAsync(), Times.AtLeastOnce);
            _mockReminderService.Verify(rs => rs.RemoveExpiredRemindersAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRespectCancellationToken()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(100)); // Cancel quickly
            var token = cts.Token;

            var executeTask = _taskReminderCleanupService.StartAsync(token);

            await executeTask;
            _mockTaskService.Verify(ts => ts.RemoveExpiredTasksAsync(), Times.AtMostOnce);
            _mockReminderService.Verify(rs => rs.RemoveExpiredRemindersAsync(), Times.AtMostOnce);
        }
    }
}
