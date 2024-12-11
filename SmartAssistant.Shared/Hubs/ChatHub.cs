using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Data.Entities;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.WebApplication.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext context;

        public ChatHub(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task JoinTeamChat(int teamId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamId.ToString());

            var messages = await context.Messages
                .Where(m => m.TeamId == teamId)
                .Include(m => m.User)
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Content = m.Content,
                    SentAt = m.SentAt,
                    UserName = m.User != null ? m.User.UserName : "Unknown"
                })
                .ToListAsync();

            await Clients.Caller.SendAsync("LoadPreviousMessages", messages);
        }






        public async Task SendMessageToTeam(int teamId, string userId, string messageContent)
        {

            var message = new SmartAssistant.Shared.Data.Entities.Message
            {
                TeamId = teamId,
                UserId = userId,
                Content = messageContent,
                SentAt = DateTime.UtcNow
            };

            context.Messages.Add(message);
            await context.SaveChangesAsync();

            var user = await context.Users.FindAsync(userId);
            await Clients.Group(teamId.ToString()).SendAsync("ReceiveMessage", user.UserName, messageContent);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
