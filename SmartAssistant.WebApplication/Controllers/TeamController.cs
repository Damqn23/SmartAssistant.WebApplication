using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Models.Team;

namespace SmartAssistant.WebApplication.Controllers
{
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IUserRepository _userRepository;

        public TeamController(ITeamService teamService, IUserRepository userRepository)
        {
            _teamService = teamService;
            _userRepository = userRepository;
        }

        // GET: /Team/Index
        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return View(teams);
        }

        // GET: /Team/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // GET: /Team/Create
        public IActionResult Create()
        {
            var model = new TeamCreateModel();
            return View(model);
        }

        // POST: /Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var ownerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                await _teamService.CreateTeamAsync(model, ownerId);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: /Team/AddUserToTeam/{teamId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToTeam(int teamId, string userName)
        {
            var user = await _userRepository.GetUserByUserNameAsync(userName);  // Fetch user by UserName
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("Details", new { id = teamId });
            }

            await _teamService.AddUserToTeamAsync(teamId, user.Id);  // Use UserId for internal handling
            return RedirectToAction("Details", new { id = teamId });
        }

        // POST: /Team/RemoveUserFromTeam/{teamId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromTeam(int teamId, string userName)
        {
            var user = await _userRepository.GetUserByUserNameAsync(userName);  // Fetch user by UserName
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("Details", new { id = teamId });
            }

            await _teamService.RemoveUserFromTeamAsync(teamId, user.Id);  // Use UserId for internal handling
            return RedirectToAction("Details", new { id = teamId });
        }

        // GET: /Team/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: /Team/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamService.DeleteTeamAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
