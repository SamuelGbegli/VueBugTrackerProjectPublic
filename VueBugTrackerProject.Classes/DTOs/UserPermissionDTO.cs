namespace VueBugTrackerProject.Classes
{
	/// <summary>
	/// DTO for user project permissions.
	/// </summary>
	public class UserPermissionDTO
	{
		/// <summary>
		/// The ID of the target project.
		/// </summary>
		public string ProjectID { get; set; }
	
		/// <summary>
		/// The ID of the user who has or will have access to the project.
		/// </summary>
		public string AccountID { get; set; }
	
		/// <summary>
		/// What the user can do with the project.
		/// </summary>
		public ProjectPermission Permission { get; set; }
	
		/// <summary>
		/// The ID of the user permission. Only used when editing the
		/// property.
		/// </summary>
		public string PermissionID { get; set; }
	
	}
}
