using Microsoft.EntityFrameworkCore;
using SmartAssistant.WebApplication.Data.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartAssistant.WebApp.Data.Entities
{
    public class Subscription
    {
		public int Id { get; set; }

		[Required]
		public string SubscriptionType { get; set; }

		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

		[EnumDataType(typeof(PaymentStatus))]
		public PaymentStatus PaymentStatus { get; set; }

		// Navigation property example
		public User User { get; set; }
		public int UserId { get; set; }

		// Custom method example
		public void RenewSubscription()
		{
			// Check if the current date is after the subscription end date
			if (DateTime.Now > EndDate)
			{		
				EndDate = EndDate.AddMonths(1);
			    PaymentStatus = PaymentStatus.Paid;
			}
			else
			{
				throw new InvalidOperationException("Subscription has not expired yet.");
			}
		}
	}
}
