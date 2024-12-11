using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
    public class TeamModel
    {
        public int Id { get; set; }          public string TeamName { get; set; }          public string OwnerId { get; set; }          public string OwnerUserName { get; set; }          public List<UserModel> Members { get; set; } = new List<UserModel>();      }
}
