using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Admin;
using SmartAssistant.Shared.Models.Team;

namespace SmartAssistant.WebApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;

        public AdminController(ITeamService teamService, IUserService userService)
        {
            _teamService = teamService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync(); 
            var users = await _userService.GetAllUsersAsync(); 

            var model = new AdminViewModel
            {
                Teams = teams.Select(t => new TeamModel
                {
                    Id = t.Id,
                    TeamName = t.TeamName,
                    OwnerUserName = t.OwnerUserName
                }).ToList(),
                Users = users.Select(u => new UserModel
                {
                    Id = u.Id,
                    UserName = u.UserName
                }).ToList()
            };

            return View(model);
        }
    }

}
