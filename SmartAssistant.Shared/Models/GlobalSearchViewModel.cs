using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models
{
    public class GlobalSearchViewModel
    {
        public List<TaskModel> Tasks { get; set; }
        public List<EventModel> Events { get; set; }
        public List<TeamModel> Teams { get; set; }
    }
}
