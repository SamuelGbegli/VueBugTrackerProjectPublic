namespace VueBugTrackerProject.Classes
{
	public class BugViewModel
	{
		/// <summary>
		/// Unique identifier for the bug.
		/// </summary>
		public string ID { get; set; }
	
		/// <summary>
		/// A short description of the bug.
		/// </summary>
		public string Summary { get; set; }
	
		/// <summary>
		/// The severity of the bug.
		/// </summary>
		public Severity Severity { get; set; }

		/// <summary>
		/// If true, means the bug is yet to be resolved.
		/// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// A longer, more detailed description of the bug.
        /// </summary>
        public string Description { get; set; }
	
		/// <summary>
		/// The ID of the user that created the bug.
		/// </summary>
		public string CreatorID { get; set; }
	
		/// <summary>
		/// The name of the user that created the bug.
		/// </summary>
		public string CreatorName { get; set; }
	
		/// <summary>
		/// The icon of the user that created the bug.
		/// </summary>
		public string CreatorIcon { get; set; }
	
		/// <summary>
		/// The number of comments and status updates the bug has.
		/// </summary>
		public int NumberOfComments { get; set; }
	
		/// <summary>
		/// The date the bug was created.
		/// </summary>
		public DateTime DateCreated { get; set; }
	
		/// <summary>
		/// The date the bug was last updated.
		/// </summary>
		public DateTime DateModified { get; set; }

		/// <summary>
		/// The ID of the project the bug belongs to.
		/// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// The name of the project the bug belongs to.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The ID of the owner of the project the bug belongs to.
        /// </summary>
        public string ProjectOwnerID { get; set; }


        public BugViewModel(Bug bug)
        {
            ID = bug.ID;
			Summary = bug.Summary;
			Severity = bug.Severity;
			IsOpen = bug.Status == Status.Open;
			Description = bug.Description;
			CreatorID = bug.Creator.Id;
			CreatorName = bug.Creator.UserName;
			CreatorIcon = bug.Creator.Icon;
			NumberOfComments = bug.Comments.Count;
			DateCreated = bug.DateCreated;
			DateModified = bug.DateModified;
			ProjectID = bug.Project.ID;
			ProjectName = bug.Project.Name;
        }
    }
}
