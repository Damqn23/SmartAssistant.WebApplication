using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Task
{
    public class TaskDeleteModel
    {
        public int Id { get; set; }         
        public string Description { get; set; } 
        [Display(Name = "Due Date", Prompt = "Select a due date")]
        public DateTime DueDate { get; set; }     }
}
