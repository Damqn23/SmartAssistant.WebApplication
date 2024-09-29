using SmartAssistant.Shared.Models.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Event
{
    public interface IEventRepository : IRepository<EventModel>
    {
        Task<List<EventModel>> GetEventsByUserIdAsync(string userId);
        System.Threading.Tasks.Task RemoveExpiredEventsAsync(); 
    }
}
