using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Admin
{
    public class AdminViewModel
    {
        public IEnumerable<TeamModel> Teams { get; set; }
        public IEnumerable<UserModel> Users { get; set; }
    }
}
