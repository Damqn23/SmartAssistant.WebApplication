using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartAssistant.Shared.Interfaces.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services.CleanUps
{
    public class EventCleanupService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TimeSpan cleanUpInterval = TimeSpan.FromMinutes(60);

        public EventCleanupService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupExpiredEvents(stoppingToken);
                await Task.Delay(cleanUpInterval, stoppingToken);
            }
        }

        private async Task CleanupExpiredEvents(CancellationToken stoppingToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                await eventService.RemoveExpiredEventsAsync();
            }
        }
    }


}
