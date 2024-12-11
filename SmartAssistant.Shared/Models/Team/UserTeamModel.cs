using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
    public class UserTeamModel
    {
        public string UserId { get; set; }          public UserModel User { get; set; }  
        public int TeamId { get; set; }          public TeamModel Team { get; set; }      }
}
