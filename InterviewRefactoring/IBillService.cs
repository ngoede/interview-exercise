using System;

namespace InterviewRefactoring
{
	public interface IBillService
	{
		Account GetWaterBillAccount(long userId);
		Account GetMortgageBillAccount(long userId);
		DateTime GetWaterBillDueDate(string account);
		DateTime GetMortgageBillDueDate(string account);
	}

	public interface IUserRepository
	{
		User FindUser(string username);
	}

	public class User
	{
		public long Id;
		public string FirstName;
		public string LastName;
		public bool IsActive { get; set; }
	}
}