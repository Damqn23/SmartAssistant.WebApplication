using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Task
{
    public class TaskModel
    {
        public int Id { get; set; } 
        public string Description { get; set; } 
        public bool IsCompleted { get; set; } 
        public DateTime DueDate { get; set; } 
        public int EstimatedTimeToComplete { get; set; } 
        public int Priority { get; set; } 
        public string UserId { get; set; } 
        public UserModel User { get; set; }     }
}
