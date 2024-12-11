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
        [MaxLength(100, ErrorMessage = "Team name cannot exceed 100 characters")]
        [Display(Name = "Team Name", Prompt = "Enter the name of the team")]
        public string TeamName { get; set; }
    }
}
