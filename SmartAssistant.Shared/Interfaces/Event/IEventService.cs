using SmartAssistant.Shared.Models.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Event
{
    public interface IEventService
    {
        Task AddEventAsync(EventCreateModel model, string userId);
        Task UpdateEventAsync(EventEditModel model);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<EventModel>> GetAllEventsAsync();
        Task<EventModel> GetEventByIdAsync(int id);
        Task<List<EventModel>> GetEventsByUserIdAsync(string userId);

        Task RemoveExpiredEventsAsync();
    }
}
