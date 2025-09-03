using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject
{
	public class AccountContainer
	{
		/// <summary>
		/// The number of accounts in the project.
		/// </summary>
		public int TotalAccounts { get; set; }
	
		/// <summary>
		/// The number of pages of accounts the user can see.
		/// </summary>
		public int Pages { get; set; }
	
		/// <summary>
		/// The current page of accounts that are visible.
		/// </summary>
		public int CurrentPage { get; set; }

		/// <summary>
		/// The current list of accounts.
		/// </summary>
		public List<AccountViewModel> Accounts { get; set; } = new List<AccountViewModel>();
	
	}
}
