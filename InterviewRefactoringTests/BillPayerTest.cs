using System;
using InterviewRefactoring;
using Moq;
using NUnit.Framework;

namespace InterviewRefactoringTests
{
	[TestFixture]
	public class BillPayerTest
	{
		private static User activeUser;
		private const string UserName = "ngoede";

		[SetUp]
		public void SetUp()
		{
			activeUser = new User { Id = 42, IsActive = true };
		}

		[Test]
		public void CalculateWaterBillDueDate_User_Must_Be_Active()
		{
			var userRepository = SetupFindUserThatReturns(new User() { IsActive = false });


			Assert.That(() => new BillPayer(null, userRepository.Object).CalculateWaterBillDueDate(UserName), Throws.Exception.Message.EqualTo("User ngoede is no longer active"));
		}

		[Test]
		public void CalculateMortgageBillDueDate_User_Must_Be_Active()
		{
			var userRepository = SetupFindUserThatReturns(new User() { IsActive = false });

			Assert.That(() => new BillPayer(null, userRepository.Object).CalculateMortgageBillDueDate(UserName), Throws.Exception.Message.EqualTo("User ngoede is no longer active"));
		}

		[Test]
		public void CalculateWaterBillDueDate_For_NonWeekend_Day([Values(25, 26, 27, 28, 29)]int dayNumber)
		{
			var userRepository = SetupForActiveUser();
			var rawBillDueDate = new DateTime(2015, 5, dayNumber);
			var billService = SetupBillServiceForWaterBillDueDate(rawBillDueDate);

			var billPayer = new BillPayer(billService.Object, userRepository.Object);
			Assert.That(billPayer.CalculateWaterBillDueDate(UserName), Is.EqualTo(rawBillDueDate));
		}

		[Test]
		public void CalculateWaterBillDueDate_For_Saturday()
		{
			var userRepository = SetupForActiveUser();
			var rawBillDueDate = new DateTime(2015, 5, 30);
			var billService = SetupBillServiceForWaterBillDueDate(rawBillDueDate);

			var billPayer = new BillPayer(billService.Object, userRepository.Object);
			Assert.That(billPayer.CalculateWaterBillDueDate(UserName), Is.EqualTo(new DateTime(2015, 5, 29)));
		}

		[Test]
		public void CalculateWaterBillDueDate_For_Sunday()
		{
			var userRepository = SetupForActiveUser();
			var rawBillDueDate = new DateTime(2015, 5, 31);
			var billService = SetupBillServiceForWaterBillDueDate(rawBillDueDate);

			var billPayer = new BillPayer(billService.Object, userRepository.Object);
			Assert.That(billPayer.CalculateWaterBillDueDate(UserName), Is.EqualTo(new DateTime(2015, 6, 1)));
		}

		[Test]
		public void CalculateMortgageBillDueDate_For_NonWeekend_Day([Values(25, 26, 27, 28, 29)]int dayNumber)
		{
			var userRepository = SetupForActiveUser();
			var rawBillDueDate = new DateTime(2015, 5, dayNumber);
			var billService = SetupBillServiceForMortgageBillDueDate(rawBillDueDate);

			var billPayer = new BillPayer(billService.Object, userRepository.Object);
			Assert.That(billPayer.CalculateMortgageBillDueDate(UserName), Is.EqualTo(rawBillDueDate));
		}

		[Test]
		public void CalculateMortgageBillDueDate_For_Saturday()
		{
			var userRepository = SetupForActiveUser();
			var rawBillDueDate = new DateTime(2015, 5, 30);
			var billService = SetupBillServiceForMortgageBillDueDate(rawBillDueDate);

			var billPayer = new BillPayer(billService.Object, userRepository.Object);
			Assert.That(billPayer.CalculateMortgageBillDueDate(UserName), Is.EqualTo(new DateTime(2015, 6, 1)));
		}

		[Test]
		public void CalculateMortgageBillDueDate_For_Sunday()
		{
			var userRepository = SetupForActiveUser();
			var rawBillDueDate = new DateTime(2015, 5, 31);
			var billService = SetupBillServiceForMortgageBillDueDate(rawBillDueDate);

			var billPayer = new BillPayer(billService.Object, userRepository.Object);
			Assert.That(billPayer.CalculateMortgageBillDueDate(UserName), Is.EqualTo(new DateTime(2015, 6, 1)));
		}

		private static Mock<IBillService> SetupBillServiceForWaterBillDueDate(DateTime rawBillDueDate)
		{
			var billService = new Mock<IBillService>();
			var account = new Account() {Number = "accountNumber-2"};
			billService.Setup(b => b.GetWaterBillAccount(activeUser.Id)).Returns(account);
			billService.Setup(b => b.GetWaterBillDueDate(account.Number)).Returns(rawBillDueDate);
			return billService;
		}

		private static Mock<IBillService> SetupBillServiceForMortgageBillDueDate(DateTime rawBillDueDate)
		{
			var billService = new Mock<IBillService>();
			var account = new Account() { Number = "accountNumber-2" };
			billService.Setup(b => b.GetMortgageBillAccount(activeUser.Id)).Returns(account);
			billService.Setup(b => b.GetMortgageBillDueDate(account.Number)).Returns(rawBillDueDate);
			return billService;
		}

		private static Mock<IUserRepository> SetupForActiveUser()
		{
			return SetupFindUserThatReturns(activeUser);
		}

		private static Mock<IUserRepository> SetupFindUserThatReturns(User userToReturn)
		{
			var userRepository = new Mock<IUserRepository>();
			var user = userToReturn;
			userRepository.Setup(u => u.FindUser(UserName)).Returns(user);
			return userRepository;
		}
	}
}
