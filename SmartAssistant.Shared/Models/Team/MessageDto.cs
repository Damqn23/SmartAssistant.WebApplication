using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
    public class MessageDto
    {
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public string UserName { get; set; }
    }

}
