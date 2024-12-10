using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models.Team
{
	public class TeamErrorViewModel
    {
		public string ErrorMessage { get; set; }
		public string RequestId { get; set; }
		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}

}
