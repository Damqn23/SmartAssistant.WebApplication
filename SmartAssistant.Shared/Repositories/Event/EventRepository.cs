using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.WebApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Repositories.Event
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public EventRepository(ApplicationDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async Task AddAsync(EventModel entity)
        {
            var eventEntity = mapper.Map<WebApp.Data.Entities.Event>(entity);
            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();
        }

        public async Task AddTeamEventAsync(TeamEventCreateModel entity)
        {
            var eventEntity = new WebApp.Data.Entities.Event
            {
                TeamId = entity.TeamId,
                EventTitle = entity.EventTitle,
                EventDate = entity.EventDate,
                UserId = entity.AssignedUserId // Assign to a team member
            };
            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteAsync(EventModel entity)
        {
            var eventEntity = await context.Events.FindAsync(entity.Id);
            if (eventEntity != null)
            {
                context.Events.Remove(eventEntity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<EventModel>> GetAllAsync()
        {
            var events = await context.Events.ToListAsync();
            return mapper.Map<List<EventModel>>(events);
        }

        public async Task<EventModel> GetByIdAsync(int id)
        {
            var eventEntity = await context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return mapper.Map<EventModel>(eventEntity);
        }

        public async Task<List<EventModel>> GetEventsByUserIdAsync(string userId)
        {
            var events = await context.Events.Where(e => e.UserId == userId).ToListAsync();
            return mapper.Map<List<EventModel>>(events);
        }

        public async Task UpdateAsync(EventModel entity)
        {
            var eventEntity = mapper.Map<WebApp.Data.Entities.Event>(entity);
            context.Events.Update(eventEntity);
            await context.SaveChangesAsync();
        }

        public async Task RemoveExpiredEventsAsync()
        {
            var currentDate = DateTime.Now;
            var expiredEvents = await context.Events
                .Where(e => e.EventDate < currentDate)
                .ToListAsync();

            if (expiredEvents.Any())
            {
                context.Events.RemoveRange(expiredEvents);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<EventModel>> GetEventsByTeamIdAsync(int teamId)
        {
            var events = await context.Events
                .Include(e => e.User) // Ensure the User data is included
                .Where(e => e.TeamId == teamId)
                .ToListAsync();
            return mapper.Map<List<EventModel>>(events);
        }


    }
}
