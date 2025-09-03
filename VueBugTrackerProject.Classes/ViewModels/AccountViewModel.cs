namespace VueBugTrackerProject.Classes
{
	/// <summary>
	/// View model for an account.
	/// </summary>
	public class AccountViewModel
	{
		/// <summary>
		/// Unique identifier for the account.
		/// </summary>
		public string ID { get; set; }
	
		/// <summary>
		/// The account's username.
		/// </summary>
		public string Username { get; set; }
	
		/// <summary>
		/// The icon that will be shown with the account's username.
		/// </summary>
		public string Icon { get; set; }
	
		/// <summary>
		/// The role and privileges the account has in the application.
		/// </summary>
		public AccountRole Role { get; set; }
	
		/// <summary>
		/// If true, the user cannot login with the account.
		/// </summary>
		public bool Suspended { get; set; }
	
		/// <summary>
		/// The date and time the account was created.
		/// </summary>
		public DateTime DateCreated { get; set; }

        public AccountViewModel(Account account)
        {
			ID = account.Id;
			Username = account.UserName;
			Icon = account.Icon;
			Role = account.Role;
			Suspended = account.Suspended;
			DateCreated = account.DateCreated;
        }

    }
}
