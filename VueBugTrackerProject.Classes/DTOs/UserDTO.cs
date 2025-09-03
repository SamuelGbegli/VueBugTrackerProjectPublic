namespace VueBugTrackerProject.Classes
{
	public class UserDTO
	{
		/// <summary>
		/// The account's username.
		/// </summary>
		public string Username { get; set; }
	
		/// <summary>
		/// The account's email address.
		/// </summary>
		public string EmailAddress { get; set; }
	
		/// <summary>
		/// The account's password.
		/// </summary>
		public string Password { get; set; }
	
		/// <summary>
		/// The account's ID. Only used when modifying an existing account.
		/// </summary>
		public string AccountID { get; set; }
	
		/// <summary>
		/// The JWT of the logged in user. Used to verify the user when making
		/// changes.
		/// </summary>
		public string AccountJWT { get; set; }
	
	}
}
