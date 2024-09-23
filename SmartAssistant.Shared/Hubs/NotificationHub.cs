using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendReminderNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveReminderNotification", message);
        }
    }
}
