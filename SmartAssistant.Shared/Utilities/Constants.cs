using SmartAssistant.WebApplication.Data.Enumerations;

namespace SmartAssistant.WebApplication.Utilities
{
    public static class Constants
    {
        
        public const int MaxUserNameLength = 50;

		public const string DefaultReminderMessage = "Don't forget!";

		public const string DefaultDescription = "New Task";
		public const bool DefaultCompletionStatus = false;

		public const int MaxTeamNameLength = 10;

		public const int DescriptionMaxLength = 200;

		public const int EventTitleMaxLength = 50;

		public const int CategoryNameMaxLength = 50;

		public const int DefaultSubscriptionDurationMonths = 1;
		public const PaymentStatus DefaultPaymentStatus = PaymentStatus.Pending;
	}
}
