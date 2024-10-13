using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Services;
using SmartAssistant.Shared.Services.Event;
using SmartAssistant.Shared.Services.Teams;
using SmartAssistant.WebApplication.Models;
using System.Diagnostics;

namespace SmartAssistant.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITaskService taskService;
        private readonly IEventService eventService;
        private readonly ITeamService teamService;

        public HomeController(ILogger<HomeController> logger, ITaskService _taskService, IEventService _eventService, ITeamService _teamService)
        {
            _logger = logger;
            taskService = _taskService;
            eventService = _eventService;
            teamService = _teamService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GlobalSearch(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return View("Index"); // Return home page if no query is provided
            }

            // Get tasks, events, and teams based on the search query
            var tasks = await taskService.GetTasksBySearchQueryAsync(searchQuery);
            var events = await eventService.GetEventsBySearchQueryAsync(searchQuery);
            var teams = await teamService.GetTeamsBySearchQueryAsync(searchQuery);

            // Combine the results into a search view model
            var searchResults = new GlobalSearchViewModel
            {
                Tasks = tasks,
                Events = events,
                Teams = teams
            };

            return View("GlobalSearchResults", searchResults); // Show the results
        }

    }
}
