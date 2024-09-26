using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
    public class TeamModel
    {
        public int Id { get; set; }  // Unique identifier
        public string TeamName { get; set; }  // Team name
        public string OwnerId { get; set; }  // Owner (creator) of the team
        public List<UserModel> Members { get; set; } = new List<UserModel>();  // List of team members
    }
}
