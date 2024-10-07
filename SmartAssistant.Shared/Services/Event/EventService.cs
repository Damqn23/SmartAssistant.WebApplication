using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services.Event
{
    public class EventService : IEventService
    {
        private readonly IEventRepository eventRepository;

        public EventService(IEventRepository _eventRepository)
        {
            eventRepository = _eventRepository;
        }

        public async Task AddEventAsync(EventCreateModel model, string userId)
        {
            var eventModel = new EventModel
            {
                EventTitle = model.EventTitle,
                EventDate = model.EventDate,
                UserId = userId
            };

            await eventRepository.AddAsync(eventModel);
        }

        public async Task DeleteEventAsync(int id)
        {
            var eventModel = await eventRepository.GetByIdAsync(id);
            if (eventModel != null)
            {
                await eventRepository.DeleteAsync(eventModel);
            }
        }

        public async Task<IEnumerable<EventModel>> GetAllEventsAsync()
        {
            return await eventRepository.GetAllAsync();
        }

        public async Task<EventModel> GetEventByIdAsync(int id)
        {
            return await eventRepository.GetByIdAsync(id);
        }

        public async Task<List<EventModel>> GetEventsByUserIdAsync(string userId)
        {
            return await eventRepository.GetEventsByUserIdAsync(userId);
        }

        public async Task UpdateEventAsync(EventEditModel model)
        {
            var eventModel = await eventRepository.GetByIdAsync(model.Id);
            if (eventModel != null)
            {
                eventModel.EventTitle = model.EventTitle;
                eventModel.EventDate = model.EventDate;
                await eventRepository.UpdateAsync(eventModel);
            }
        }

        public async Task RemoveExpiredEventsAsync()
        {
            await eventRepository.RemoveExpiredEventsAsync();
        }

        public async Task<List<EventModel>> GetEventsByTeamIdAsync(int teamId)
        {
           return await eventRepository.GetEventsByTeamIdAsync(teamId);
        }
        public async Task AddTeamEventAsync(TeamEventCreateModel model, string userId)
        {
            await eventRepository.AddTeamEventAsync(model);
        }
    }
}
