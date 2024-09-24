using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Task
{
    public class TaskDeleteModel
    {
        public int Id { get; set; } // Task ID
        public string Description { get; set; } // Task description
        public DateTime DueDate { get; set; } // Task due date
    }
}
