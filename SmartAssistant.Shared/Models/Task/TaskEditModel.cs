using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Task
{
    public class TaskEditModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int EstimatedTimeToComplete { get; set; }
        public int Priority { get; set; }
    }
}
