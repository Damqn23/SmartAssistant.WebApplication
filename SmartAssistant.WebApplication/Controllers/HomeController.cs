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
                return View("Index"); 
            }

            var tasks = await taskService.GetTasksBySearchQueryAsync(searchQuery);
            var events = await eventService.GetEventsBySearchQueryAsync(searchQuery);
            var teams = await teamService.GetTeamsBySearchQueryAsync(searchQuery);

            var searchResults = new GlobalSearchViewModel
            {
                Tasks = tasks,
                Events = events,
                Teams = teams
            };

            return View("GlobalSearchResults", searchResults); 
        }
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return Json(new { results = new List<object>() }); 
            }

            var tasks = await taskService.GetTasksBySearchQueryAsync(searchQuery);
            var events = await eventService.GetEventsBySearchQueryAsync(searchQuery);
            var teams = await teamService.GetTeamsBySearchQueryAsync(searchQuery);

            var results = tasks.Select(t => new
            {
                name = t.Description,
                type = "Task",
                url = Url.Action("Details", "Task", new { id = t.Id }) 
            })
            .Concat(events.Select(e => new
            {
                name = e.EventTitle,
                type = "Event",
                url = Url.Action("Details", "Event", new { id = e.Id }) 
            }))
            .Concat(teams.Select(te => new
            {
                name = te.TeamName,
                type = "Team",
                url = Url.Action("Details", "Team", new { id = te.Id }) 
            }))
            .ToList();

            return Json(new { results });
        }

    }
}
