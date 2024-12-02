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
            // Mocks
            _mockRootServiceProvider = new Mock<IServiceProvider>();
            _mockScopedServiceProvider = new Mock<IServiceProvider>();
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockServiceScope = new Mock<IServiceScope>();
            _mockTaskService = new Mock<ITaskService>();
            _mockReminderService = new Mock<IReminderService>();

            // Setup the root IServiceProvider to return the IServiceScopeFactory
            _mockRootServiceProvider
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(_mockScopeFactory.Object);

            // Setup the IServiceScopeFactory to create a mock IServiceScope
            _mockScopeFactory
                .Setup(sf => sf.CreateScope())
                .Returns(_mockServiceScope.Object);

            // Setup the IServiceScope to return the scoped IServiceProvider
            _mockServiceScope
                .Setup(s => s.ServiceProvider)
                .Returns(_mockScopedServiceProvider.Object);

            // Setup the scoped IServiceProvider to return ITaskService and IReminderService
            _mockScopedServiceProvider
                .Setup(sp => sp.GetService(typeof(ITaskService)))
                .Returns(_mockTaskService.Object);

            _mockScopedServiceProvider
                .Setup(sp => sp.GetService(typeof(IReminderService)))
                .Returns(_mockReminderService.Object);

            // Create an instance of TaskReminderCleanupService
            _taskReminderCleanupService = new TaskReminderCleanupService(_mockRootServiceProvider.Object);
        }

        [Fact]
        public async Task CleanupExpiredTaskAndReminder_ShouldCallServices()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _taskReminderCleanupService.StartAsync(cancellationToken);

            // Assert
            _mockTaskService.Verify(ts => ts.RemoveExpiredTasksAsync(), Times.AtLeastOnce);
            _mockReminderService.Verify(rs => rs.RemoveExpiredRemindersAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRespectCancellationToken()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(100)); // Cancel quickly
            var token = cts.Token;

            // Act
            var executeTask = _taskReminderCleanupService.StartAsync(token);

            // Assert
            await executeTask;
            _mockTaskService.Verify(ts => ts.RemoveExpiredTasksAsync(), Times.AtMostOnce);
            _mockReminderService.Verify(rs => rs.RemoveExpiredRemindersAsync(), Times.AtMostOnce);
        }
    }
}
