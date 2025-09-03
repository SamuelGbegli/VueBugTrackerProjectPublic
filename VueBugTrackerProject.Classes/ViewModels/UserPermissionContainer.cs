namespace VueBugTrackerProject.Classes
{
	/// <summary>
	/// Container to return user permissions
	/// </summary>
	public class UserPermissionContainer
	{
		/// <summary>
		/// The number of user permissions in the project.
		/// </summary>
		public int TotalPermissions { get; set; }
	
		/// <summary>
		/// The number of pages of user permissions the user can see.
		/// </summary>
		public int Pages { get; set; }
	
		/// <summary>
		/// The current page of user permissions that are visible.
		/// </summary>
		public int CurrentPage { get; set; }

		/// <summary>
		/// The current list of user permissions.
		/// </summary>
		public List<UserPermissionViewModel> UserPermissions { get; set; } = new List<UserPermissionViewModel>();
	
	}
}
