using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Event;
using SmartAssistant.Shared.Models.Event;
using System.Security.Claims;

namespace SmartAssistant.WebApplication.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService eventService;
        private readonly IMapper mapper;

        public EventController(IEventService _eventService, IMapper _mapper)
        {
            eventService = _eventService;
            mapper = _mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var events = await eventService.GetEventsByUserIdAsync(userId);
            return View(events);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateModel eventCreateModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await eventService.AddEventAsync(eventCreateModel, userId);
                return RedirectToAction(nameof(Index));
            }

            return View(eventCreateModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var eventModel = await eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            var editModel = mapper.Map<EventEditModel>(eventModel);
            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventEditModel editModel)
        {
            if (ModelState.IsValid)
            {
                await eventService.UpdateEventAsync(editModel);
                return RedirectToAction(nameof(Index));
            }

            return View(editModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var eventModel = await eventService.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await eventService.DeleteEventAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
