using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Models.Event;
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
            context.Events.Add(eventEntity);
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
    }
}
