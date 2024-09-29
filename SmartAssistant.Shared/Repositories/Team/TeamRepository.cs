using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.WebApplication.Data;
using SmartAssistant.WebApplication.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Repositories.Team
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public TeamRepository(ApplicationDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }
        public async Task AddAsync(TeamModel entity)
        {
            var teamEntity = mapper.Map<WebApp.Data.Entities.Team>(entity);  // Map to Team entity

            // Add the team entity once and save changes
            context.Teams.Add(teamEntity);
            await context.SaveChangesAsync();  // Ensure this is called only once
        }


        public async Task AddUserToTeamAsync(string userId, int teamId)
        {
            // Ensure that userId is not null or empty
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
            var teamEntity = await context.Teams.FindAsync(entity.Id);

            if (teamEntity != null)
            {
                // Delete all related UserTeam entries before deleting the team
                var userTeams = await context.UserTeams.Where(ut => ut.TeamId == teamEntity.Id).ToListAsync();
                if (userTeams.Any())
                {
                    context.UserTeams.RemoveRange(userTeams);
                }

                // Now you can delete the team
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
            // Fetch teams where the user is a member
            var memberTeams = await context.Teams
                .Where(t => t.UserTeams.Any(ut => ut.UserId == userId))
                .ToListAsync();

            // Fetch teams where the user is the owner
            var ownedTeams = await context.Teams
                .Where(t => t.OwnerId == userId)
                .ToListAsync();

            // Combine both sets of teams and return distinct results
            var allTeams = memberTeams.Concat(ownedTeams)
                .GroupBy(t => t.Id)        // Group by Team ID
                .Select(g => g.First())    // Select only the first occurrence of each team
                .ToList();

            // Debugging: Output the teams to verify

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
            var teamEntity = mapper.Map<WebApp.Data.Entities.Team>(entity);
            context.Teams.Update(teamEntity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TeamModel>> GetTeamsByOwnerIdAsync(string ownerId)
        {
            // Fetch teams where the user is the owner
            var ownedTeams = await context.Teams
                .Where(t => t.OwnerId == ownerId)
                .ToListAsync();

            return mapper.Map<List<TeamModel>>(ownedTeams);
        }

    }
}
