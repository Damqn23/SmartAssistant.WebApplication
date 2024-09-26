using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
    public class TeamCreateModel
    {
        [Required(ErrorMessage = "Team name is required")]
        [MaxLength(100)]
        public string TeamName { get; set; }
    }
}
