using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Models
{
	public class ReminderModel
	{
		public int Id { get; set; }
		
		public string ReminderMessage { get; set; }

		public DateTime ReminderDate { get; set; }
		public string UserId { get; set; }

		public UserModel User { get; set; }
	}
}
