using System;

namespace InterviewRefactoring
{
	//Business Requirements:
	
	// Water Bill paid on first weekday closest to due date
	//If due on a Saturday it's paid on the previous Friday 
	//If due on a Sunday it's paid on following Monday

	//Mortgage is paid on first weekday after due date taking into account
	//any grace period

	public class BillPayer
	{
		private readonly IBillService billService;
		private readonly IUserRepository userRepository;

		public BillPayer(IBillService billService, IUserRepository userRepository)
		{
			this.billService = billService;
			this.userRepository = userRepository;
		}

		public DateTime CalculateWaterBillDueDate(string username)
		{
			var user = userRepository.FindUser(username);
			if (!user.IsActive)
			{
				throw new Exception(string.Format("User {0} is no longer active", username));
			}

			var account = billService.GetWaterBillAccount(user.Id);

			//Get Water Bill Due Date
			var dueDate = billService.GetWaterBillDueDate(account.Number);
			
			if (dueDate.DayOfWeek == DayOfWeek.Saturday)
			{
				return dueDate.AddDays(-1);
			}
			if (dueDate.DayOfWeek == DayOfWeek.Sunday)
			{
				return dueDate.AddDays(1);
			}
			return dueDate;
		}

		public DateTime CalculateMortgageBillDueDate(string username)
		{
			var user = userRepository.FindUser(username);
			if (!user.IsActive)
			{
				throw new Exception(string.Format("User {0} is no longer active", username));
			}
			var account = billService.GetMortgageBillAccount(user.Id);

			//Get Water Bill Due Date
			var dueDate = billService.GetMortgageBillDueDate(account.Number).AddDays(account.GracePeriodInDays);

			if (dueDate.DayOfWeek == DayOfWeek.Saturday)
			{
				return dueDate.AddDays(2);
			}
			if (dueDate.DayOfWeek == DayOfWeek.Sunday)
			{
				return dueDate.AddDays(1);
			}
			return dueDate;
		}
	}
}
