using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Task
{
    public class TaskModel
    {
        public int Id { get; set; } // Unique identifier for the task

        public string Description { get; set; } // Task description

        public bool IsCompleted { get; set; } // Indicates whether the task is completed

        public DateTime DueDate { get; set; } // The date and time when the task is due

        public int EstimatedTimeToComplete { get; set; } // Estimated time to complete the task (in minutes)

        public int Priority { get; set; } // Task priority level (1-5)

        public string UserId { get; set; } // Foreign key to the user

        public UserModel User { get; set; } // Navigation property for the associated user
    }
}
