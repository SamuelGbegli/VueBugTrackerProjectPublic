namespace VueBugTrackerProject.Classes
{
	public class UserPermissionViewModel
	{
		/// <summary>
		/// Unique identifier for the class.
		/// </summary>
		public string ID { get; set; }
	
		/// <summary>
		/// The ID of the user who is granted access to the project.
		/// </summary>
		public string AccountID { get; set; }
	
		/// <summary>
		/// The name of the user who is granted access to the project.
		/// </summary>
		public string AccountName { get; set; }
	
		/// <summary>
		/// The icon of the user who is granted access to the project.
		/// </summary>
		public string AccountIcon { get; set; }
	
		/// <summary>
		/// What the user can do with the project.
		/// </summary>
		public ProjectPermission Permission { get; set; }

        public UserPermissionViewModel()
        {
            
        }

        public UserPermissionViewModel(UserPermission userPermission)
        {
            ID = userPermission.ID;
			AccountID = userPermission.Account.Id;
			AccountName = userPermission.Account.UserName;
			AccountIcon = userPermission.Account.Icon;
			Permission = userPermission.Permission;
        }

    }
}
