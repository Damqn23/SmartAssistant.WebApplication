using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Task
{
    public class TaskEditModel
    {
        [Display(Name = "Task ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Task Description", Prompt = "Enter task description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        [Display(Name = "Due Date", Prompt = "Select a due date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Estimated time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Estimated time must be greater than 0")]
        [Display(Name = "Estimated Time to Complete (hours)", Prompt = "Enter estimated time in hours")]
        public int EstimatedTimeToComplete { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Task Priority", Prompt = "Select priority level")]
        public PriorityLevel Priority { get; set; }

        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Assigned User ID")]
        public string? UserId { get; set; }
    }
}
