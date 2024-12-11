using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Interfaces.Team;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Calendar;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using System.Security.Claims;

namespace SmartAssistant.WebApplication.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IEventService eventService;
        private readonly ITeamService teamService;
        private readonly IMapper mapper;

        public CalendarController(ITaskService _taskService, IEventService _eventService, ITeamService _teamService, IMapper _mapper)
        {
            taskService = _taskService;
            eventService = _eventService;
            teamService = _teamService;
            mapper = _mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetTasksByUserIdAsync(userId);
            var events = await eventService.GetEventsByUserIdAsync(userId);

            var currentMonth = DateTime.Now; 

            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month))
                .Select(day => new DateTime(currentMonth.Year, currentMonth.Month, day))
                .ToList();

            var days = daysInMonth.Select(date => new DayViewModel
            {
                Date = date,
                Tasks = tasks.Where(t => t.DueDate.Date == date.Date).ToList(),
                Events = events.Where(e => e.EventDate.Date == date.Date).ToList()
            }).ToList();

            var viewModel = new CalendarViewModel
            {
                CurrentMonth = currentMonth,
                Days = days
            };

            return View(viewModel);
        }

        public async Task<IActionResult> TeamIndex(int teamId)
        {
            var team = await teamService.GetTeamByIdAsync(teamId);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            var tasks = await taskService.GetTasksByTeamIdAsync(teamId);
            var events = await eventService.GetEventsByTeamIdAsync(teamId);

            var currentMonth = DateTime.Now;
            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month))
                .Select(day => new DateTime(currentMonth.Year, currentMonth.Month, day))
                .ToList();

            var days = daysInMonth.Select(date => new DayViewModel
            {
                Date = date,
                Tasks = tasks.Where(t => t.DueDate.Date == date.Date).ToList(),
                Events = events.Where(e => e.EventDate.Date == date.Date).ToList()
            }).ToList();

            var viewModel = new CalendarViewModel
            {
                CurrentMonth = currentMonth,
                Days = days,
                TeamOwnerUserName = team.OwnerUserName,
                TeamId = teamId
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddTeamTask(int teamId, DateTime date)
        {
            var team = await teamService.GetTeamByIdAsync(teamId);
            if (team == null || team.OwnerUserName != User.Identity.Name)
            {
                return Unauthorized("Only the team owner can add tasks.");
            }

            var teamMembers = (await teamService.GetTeamMembersByTeamIdAsync(teamId)).ToList();

            if (team.OwnerId != null && !teamMembers.Any(m => m.Id == team.OwnerId))
            {
                teamMembers.Add(new UserModel
                {
                    Id = team.OwnerId,
                    UserName = team.OwnerUserName
                });
            }
            if (teamMembers == null || !teamMembers.Any())
            {
                throw new Exception("No team members found.");
            }

            var model = new TeamTaskCreateModel
            {
                TeamId = teamId,
                DueDate = date,
                TeamMembers = teamMembers.Select(m => new SelectListItem
                {
                    Value = m.Id,   
                    Text = m.UserName   
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeamTask(TeamTaskCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var teamMembers = await teamService.GetTeamMembersByTeamIdAsync(model.TeamId);

                model.TeamMembers = teamMembers.Select(m => new SelectListItem
                {
                    Value = m.Id,
                    Text = m.UserName
                });


                return View(model);
            }
            await taskService.AddTeamTaskAsync(model, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return RedirectToAction("TeamIndex", new { teamId = model.TeamId });
        }

        [HttpGet]
        public async Task<IActionResult> AddTeamEvent(int teamId, DateTime date)
        {
            var team = await teamService.GetTeamByIdAsync(teamId);
            if (team == null || team.OwnerUserName != User.Identity.Name)
            {
                return Unauthorized("Only the team owner can add events.");
            }

            var teamMembers = (await teamService.GetTeamMembersByTeamIdAsync(teamId)).ToList();

            if (team.OwnerId != null && !teamMembers.Any(m => m.Id == team.OwnerId))
            {
                teamMembers.Add(new UserModel
                {
                    Id = team.OwnerId,
                    UserName = team.OwnerUserName
                });
            }

            if (teamMembers == null || !teamMembers.Any())
            {
                throw new Exception("No team members found.");
            }

            var model = new TeamEventCreateModel
            {
                TeamId = teamId,
                EventDate = date,
                TeamMembers = teamMembers.Select(m => new SelectListItem
                {
                    Value = m.Id,   
                    Text = m.UserName   
                })
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddTeamEvent(TeamEventCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var teamMembers = await teamService.GetTeamMembersByTeamIdAsync(model.TeamId);

                model.TeamMembers = teamMembers.Select(m => new SelectListItem
                {
                    Value = m.Id,
                    Text = m.UserName
                });

                return View(model);
            }

            await eventService.AddTeamEventAsync(model, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return RedirectToAction("TeamIndex", new { teamId = model.TeamId });
        }

    }
}
