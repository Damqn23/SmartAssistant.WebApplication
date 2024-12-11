using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Event
{
    public class EventEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event title is required")]
        [MaxLength(100, ErrorMessage = "Event title is too long")]
        [Display(Name = "Event Title", Prompt = "Enter the title of the event")]
        public string EventTitle { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [Display(Name = "Event Date", Prompt = "Select the event date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }
    }
}
