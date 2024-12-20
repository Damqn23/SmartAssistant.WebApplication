﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
    public class TeamEventCreateModel
    {
        public int TeamId { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public string AssignedUserId { get; set; }         
        public IEnumerable<SelectListItem> TeamMembers { get; set; }
        public string AssignedUserName { get; set; } 
    }
}
