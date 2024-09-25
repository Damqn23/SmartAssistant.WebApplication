using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Event
{
    public class EventCreateModel
    {
        [Required(ErrorMessage = "Event title is required")]
        [MaxLength(100, ErrorMessage = "Event title is too long")]
        public string EventTitle { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        public DateTime EventDate { get; set; }
    }
}
