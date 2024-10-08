using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
    public class TeamTaskCreateModel
    {
        public int TeamId { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int EstimatedTimeToComplete { get; set; }
        public int Priority { get; set; }
        public string AssignedUserId { get; set; } // ID of the team member the task is assigned to
        public IEnumerable<SelectListItem> TeamMembers { get; set; }
        public string AssignedUserName { get; set; } // Add this property

    }
}
