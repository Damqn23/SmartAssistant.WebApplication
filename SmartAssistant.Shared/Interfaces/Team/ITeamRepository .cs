﻿using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Team
{
    public interface ITeamRepository : IRepository<TeamModel>
    {
        System.Threading.Tasks.Task AddUserToTeamAsync(string userId, int teamId);
        Task<IEnumerable<TeamModel>> GetTeamsByUserIdAsync(string userId);

        System.Threading.Tasks.Task RemoveUserFromTeamAsync(int teamId, string userId);

    }
}