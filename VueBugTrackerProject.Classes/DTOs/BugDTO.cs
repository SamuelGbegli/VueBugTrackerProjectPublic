namespace VueBugTrackerProject.Classes
{
	public class BugDTO
	{
		/// <summary>
		/// A short description of the bug.
		/// </summary>
		public string Summary { get; set; }
	
		/// <summary>
		/// The severity of the bug.
		/// </summary>
		public Severity Severity { get; set; }
	
		/// <summary>
		/// A longer, more detailed description of the bug.
		/// </summary>
		public string Description { get; set; }
	
		/// <summary>
		/// The bug�s ID. Only used when making changes to the project.
		/// </summary>
		public string BugID { get; set; }

		/// <summary>
		/// Determines whether the bug is open or closed.
		/// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// The ID of the project the bug belongs to.
        /// </summary>
        public string ProjectID { get; set; }
	
	}
}
