using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartAssistant.Shared.Interfaces.Task;
using SmartAssistant.Shared.Models.Task;
using System.Security.Claims;

namespace SmartAssistant.WebApplication.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IMapper mapper;

        public TaskController(ITaskService _taskService, IMapper _mapper)
        {
            taskService = _taskService;
            mapper = _mapper;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetTasksByUserIdAsync(userId);
            return View(tasks);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateModel taskCreateModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await taskService.AddTaskAsync(taskCreateModel, userId);

                return RedirectToAction(nameof(Index));
            }

            return View(taskCreateModel);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var task = await taskService.GetTaskByIdAsync(id);
            if(task == null)
            {
                return NotFound();
            }
            var editModel = mapper.Map<TaskEditModel>(task);

            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskEditModel editModel)
        {
            if (ModelState.IsValid)
            { 
                await taskService.UpdateTaskAsync(editModel);
                return RedirectToAction(nameof(Index));
            }
            return View(editModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var task = await taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var deleteModel = mapper.Map<TaskDeleteModel>(task);

            return View(deleteModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await taskService.DeleteTaskAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
