using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Services.CleanUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Services
{
    public class EventCleanupServiceTests
    {
        private readonly Mock<IServiceProvider> _mockRootServiceProvider;
        private readonly Mock<IServiceProvider> _mockScopedServiceProvider;
        private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
        private readonly Mock<IServiceScope> _mockServiceScope;
        private readonly Mock<IEventService> _mockEventService;
        private readonly EventCleanupService _eventCleanupService;

        public EventCleanupServiceTests()
        {
            // Mocks
            _mockRootServiceProvider = new Mock<IServiceProvider>();
            _mockScopedServiceProvider = new Mock<IServiceProvider>();
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockServiceScope = new Mock<IServiceScope>();
            _mockEventService = new Mock<IEventService>();

            // Mock IServiceProvider to return IServiceScopeFactory
            _mockRootServiceProvider
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(_mockScopeFactory.Object);

            // Mock IServiceScopeFactory to create a scope
            _mockScopeFactory
                .Setup(sf => sf.CreateScope())
                .Returns(_mockServiceScope.Object);

            // Mock IServiceScope to return the scoped IServiceProvider
            _mockServiceScope
                .Setup(s => s.ServiceProvider)
                .Returns(_mockScopedServiceProvider.Object);

            // Mock the scoped IServiceProvider to return IEventService
            _mockScopedServiceProvider
                .Setup(sp => sp.GetService(typeof(IEventService)))
                .Returns(_mockEventService.Object);

            // Create EventCleanupService instance
            _eventCleanupService = new EventCleanupService(_mockRootServiceProvider.Object);
        }

        [Fact]
        public async Task CleanupExpiredEvents_ShouldCallRemoveExpiredEventsAsync()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _eventCleanupService.StartAsync(cancellationToken);

            // Assert
            _mockEventService.Verify(es => es.RemoveExpiredEventsAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRespectCancellationToken()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(100)); // Cancel quickly
            var token = cts.Token;

            // Act
            var executeTask = _eventCleanupService.StartAsync(token);

            // Assert
            await executeTask;
            _mockEventService.Verify(es => es.RemoveExpiredEventsAsync(), Times.AtMostOnce);
        }
    }
}
