using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Task
{
    public class TaskCreateModel
    {
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Estimated time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Estimated time must be greater than 0")]
        public int EstimatedTimeToComplete { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public PriorityLevel Priority { get; set; }
    }
}
