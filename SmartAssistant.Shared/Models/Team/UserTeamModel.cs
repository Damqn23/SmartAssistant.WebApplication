using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
    public class UserTeamModel
    {
        public string UserId { get; set; }  // User foreign key
        public UserModel User { get; set; }  // Navigation property to UserModel

        public int TeamId { get; set; }  // Team foreign key
        public TeamModel Team { get; set; }  // Navigation property to TeamModel
    }
}
