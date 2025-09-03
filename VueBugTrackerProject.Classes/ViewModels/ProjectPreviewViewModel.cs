namespace VueBugTrackerProject.Classes
{
	/// <summary>
	/// View model for a preview of a project.
	/// </summary>
	public class ProjectPreviewViewModel
	{
		/// <summary>
		/// Unique identifier for the project.
		/// </summary>
		public string ID { get; set; }
	
		/// <summary>
		/// The name of the project.
		/// </summary>
		public string Name { get; set; }
	
		/// <summary>
		/// The short description of the project.
		/// </summary>
		public string Summary { get; set; }

        /// <summary>
        /// The ID of the user that created the project.
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// The name of the user that created the project.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The icon of the user that created the project.
        /// </summary>
        public string OwnerIcon { get; set; }

        /// <summary>
        /// The number of bugs in the project are open.
        /// </summary>
        public int OpenBugs { get; set; }
	
		/// <summary>
		/// The total number of bugs in the project.
		/// </summary>
		public int TotalBugs { get; set; }
	
		/// <summary>
		/// The date the project was last updated.
		/// </summary>
		public DateTime DateModified { get; set; }

		/// <summary>
		/// Creates a view model summary of a project.
		/// </summary>
		/// <param name="project">The project the view model is being made for.</param>
        public ProjectPreviewViewModel(Project project)
        {
            ID = project.ID;
			Name = project.Name;
			Summary = project.Summary;
			OwnerID = project.Owner.Id;
			OwnerName = project.Owner.UserName;
			OwnerIcon = project.Owner.Icon;
			OpenBugs = project.Bugs.Count(b => b.Status == Status.Open);
			TotalBugs = project.Bugs.Count();
			DateModified = project.DateModified;
        }

    }
}
