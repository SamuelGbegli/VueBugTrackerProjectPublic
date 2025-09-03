namespace VueBugTrackerProject.Classes
{
	/// <summary>
	/// View model for a project.
	/// </summary>
	public class ProjectViewModel
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
		/// The project's external link.
		/// </summary>
        public string Link { get; set; }

		/// <summary>
		/// The visibility of the project.
		/// </summary>
        public Visibility Visibility { get; set; }

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
        /// The date the project was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The date the project was last updated.
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// The formatted project description.
        /// </summary>
        public string Description { get; set; }
	
		/// <summary>
		/// The project's tags.
		/// </summary>
		public List<string> Tags { get; set; }


        public ProjectViewModel(Project project)
        {
            ID = project.ID;
			Name = project.Name;
			Summary = project.Summary;
			Link = project.Link;
			Visibility = project.Visibility;
			OwnerID = project.Owner.Id;
			OwnerName = project.Owner.UserName;
			OwnerIcon = project.Owner.Icon;
			OpenBugs = project.Bugs.Count(b => b.Status == Status.Open);
			TotalBugs = project.Bugs.Count;
			DateCreated = project.DateCreated;
			DateModified = project.DateModified;
			Description = project.FormattedDescription;
			Tags = project.Tags;
        }
    }
}
