using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Models.Team;
using System.Diagnostics;
using System.Security.Claims;

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

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var teams = await _teamService.GetTeamsByUserIdAsync(userId);
            return View(teams);
        }



        public async Task<IActionResult> Details(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        public IActionResult Create()
        {
            var model = new TeamCreateModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var ownerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (ownerId != null)
                {
                    await _teamService.CreateTeamAsync(model, ownerId);
                }
                else
                {
                    ModelState.AddModelError("", "Unable to find the current user.");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToTeam(int teamId, string userName)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetUserByUserNameAsync(userName);  

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("Details", new { id = teamId });
            }

            try
            {
                await _teamService.AddUserToTeamAsync(teamId, user.Id, currentUserId);  
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("Details", new { id = teamId });
            }

            return RedirectToAction("Details", new { id = teamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromTeam(int teamId, string userName)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var user = await _userRepository.GetUserByUserNameAsync(userName);  

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("Details", new { id = teamId });
            }

            try
            {
                await _teamService.RemoveUserFromTeamAsync(teamId, user.Id, currentUserId);  
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("Details", new { id = teamId });
            }

            return RedirectToAction("Details", new { id = teamId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _teamService.DeleteTeamAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var errorModel = new TeamErrorViewModel
                {
                    ErrorMessage = ex.Message,
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };

                return View("~/Views/Team/Error.cshtml", errorModel); // Explicitly specify the custom view path
            }
        }




        public async Task<IActionResult> Chat(int teamId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var teams = await _teamService.GetTeamsByUserIdAsync(userId);

            return View(teams);
        }

        public IActionResult TeamChat(int teamId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            

            var model = new ChatViewModel
            {
                TeamId = teamId,
                UserId = userId
            };

            return View(model);
        }
    }
}
