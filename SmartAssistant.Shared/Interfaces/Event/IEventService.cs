using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Event
{
    public interface IEventService
    {
        System.Threading.Tasks.Task AddEventAsync(EventCreateModel model, string userId);
        System.Threading.Tasks.Task UpdateEventAsync(EventEditModel model);
        System.Threading.Tasks.Task DeleteEventAsync(int id);
        Task<IEnumerable<EventModel>> GetAllEventsAsync();
        Task<EventModel> GetEventByIdAsync(int id);
        Task<List<EventModel>> GetEventsByUserIdAsync(string userId);

        System.Threading.Tasks.Task RemoveExpiredEventsAsync();

        Task<List<EventModel>> GetEventsByTeamIdAsync(int teamId);

        System.Threading.Tasks.Task AddTeamEventAsync(TeamEventCreateModel model, string userId);
        Task<List<EventModel>> GetEventsBySearchQueryAsync(string searchQuery);

    }
}
