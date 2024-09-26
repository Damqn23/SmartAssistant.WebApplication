using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartAssistant.Shared.Interfaces.Reminder;
using SmartAssistant.Shared.Interfaces.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services.CleanUps
{
    public class TaskReminderCleanupService : BackgroundService
    {

        private readonly IServiceProvider serviceProvider;

        private readonly TimeSpan cleanUpInterval = TimeSpan.FromMinutes(60);

        public TaskReminderCleanupService(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupExpiredTaskAndReminder(stoppingToken);
                await Task.Delay(cleanUpInterval, stoppingToken);
            }
        }

        private async Task CleanupExpiredTaskAndReminder(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
                var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();

                await taskService.RemoveExpiredTasksAsync();
                await reminderService.RemoveExpiredRemindersAsync();
            }
        }
    }
}
