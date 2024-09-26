﻿using SmartAssistant.Shared.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Team
{
    public interface ITeamService
    {
        Task<TeamModel> GetTeamByIdAsync(int id);
        Task<IEnumerable<TeamModel>> GetAllTeamsAsync();
        System.Threading.Tasks.Task CreateTeamAsync(TeamCreateModel teamCreateModel, string ownerId);
        System.Threading.Tasks.Task AddUserToTeamAsync(int teamId, string userId);
        System.Threading.Tasks.Task RemoveUserFromTeamAsync(int teamId, string userId);
        System.Threading.Tasks.Task DeleteTeamAsync(int id);
    }
}