using AutoMapper;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.WebApplication.Data;
using SmartAssistant.WebApplication.Data.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Repositories.Team
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<TeamRepository> logger;

        public TeamRepository(ApplicationDbContext _context, IMapper _mapper, ILogger<TeamRepository> _logger)
        {
            context = _context;
            mapper = _mapper;
        }
        public async Task AddAsync(TeamModel entity)
        {
            var teamEntity = mapper.Map<WebApp.Data.Entities.Team>(entity);  // Map to Team entity

            context.Teams.Add(teamEntity);
            await context.SaveChangesAsync();  // Ensure this is called only once
        }


        public async Task AddUserToTeamAsync(string userId, int teamId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
            }

            var userTeam = new UserTeam
            {
                UserId = userId,  // This should now be correctly set
                TeamId = teamId
            };

            await context.UserTeams.AddAsync(userTeam);
            await context.SaveChangesAsync();
        }


        public async Task DeleteAsync(TeamModel entity)
        {
            var teamEntity = await context.Teams
                .Include(t => t.Messages) // Include related messages
                .FirstOrDefaultAsync(t => t.Id == entity.Id);

            if (teamEntity != null)
            {
                var userTeams = await context.UserTeams.Where(ut => ut.TeamId == teamEntity.Id).ToListAsync();
                if (userTeams.Any())
                {
                    context.UserTeams.RemoveRange(userTeams);
                }

                if (teamEntity.Messages != null && teamEntity.Messages.Any())
                {
                    context.Messages.RemoveRange(teamEntity.Messages);
                }

                context.Teams.Remove(teamEntity);

                await context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<TeamModel>> GetAllAsync()
        {
            var teams = await context.Teams
                .Include(t => t.Owner)  // Eager load the Owner property
                .ToListAsync();

            return mapper.Map<List<TeamModel>>(teams);  
        }


        public async Task<TeamModel> GetByIdAsync(int id)
        {
            var teamEntity = await context.Teams
                .Include(t => t.Owner) 
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            return mapper.Map<TeamModel>(teamEntity);
        }

        public async Task<IEnumerable<TeamModel>> GetTeamsByUserIdAsync(string userId)
        {
            var memberTeams = await context.Teams
                .Where(t => t.UserTeams.Any(ut => ut.UserId == userId))
                .Include(t => t.Owner) // Ensure Owner data is loaded
                .ToListAsync();

            var ownedTeams = await context.Teams
                .Where(t => t.OwnerId == userId)
                .Include(t => t.Owner)
                .ToListAsync();

            var allTeams = memberTeams.Concat(ownedTeams)
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .ToList();

            return mapper.Map<List<TeamModel>>(allTeams);
        }



        public async Task RemoveUserFromTeamAsync(int teamId, string userId)
        {
            var userTeam = await context.UserTeams
                .FirstOrDefaultAsync(ut => ut.TeamId == teamId && ut.UserId == userId);

            if (userTeam != null)
            {
                context.UserTeams.Remove(userTeam);
                await context.SaveChangesAsync();
            }
        }


        public async Task UpdateAsync(TeamModel entity)
        {
            var teamEntity = await context.Teams.FirstOrDefaultAsync(t => t.Id == entity.Id);
            if (teamEntity != null)
            {
                teamEntity.TeamName = entity.TeamName;
                teamEntity.OwnerId = entity.OwnerId;

                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TeamModel>> GetTeamsByOwnerIdAsync(string ownerId)
        {
            var ownedTeams = await context.Teams
                .Where(t => t.OwnerId == ownerId)
                .ToListAsync();

            return mapper.Map<List<TeamModel>>(ownedTeams);
        }
        public async Task<IEnumerable<UserModel>> GetTeamMembersByTeamIdAsync(int teamId)
        {
            var members = await context.UserTeams
                                       .Where(ut => ut.TeamId == teamId)
                                       .Select(ut => ut.User) // Ensure 'User' is properly included
                                       .ToListAsync();

            return mapper.Map<List<UserModel>>(members);
        }

        public async Task<IEnumerable<TaskModel>> GetTasksByTeamIdAsync(int teamId)
        {
            var taskEntities = await context.Tasks.Where(t => t.TeamId == teamId).ToListAsync();
            return mapper.Map<IEnumerable<TaskModel>>(taskEntities); // Use AutoMapper to map entities to models
        }

        public async Task<IEnumerable<EventModel>> GetEventsByTeamIdAsync(int teamId)
        {
            var eventEntities = await context.Events.Where(e => e.TeamId == teamId).ToListAsync();
            return mapper.Map<IEnumerable<EventModel>>(eventEntities); // Use AutoMapper to map entities to models
        }

    }
}
