using Microsoft.Extensions.Hosting;
using SmartAssistant.Shared.Interfaces.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services.Event.CleanUps
{
    public class EventCleanupService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IEventService _eventService;

        public EventCleanupService(IEventService eventService)
        {
            _eventService = eventService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run the cleanup every hour
            _timer = new Timer(DeleteExpiredEvents, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async void DeleteExpiredEvents(object state)
        {
            await _eventService.RemoveExpiredEventsAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
