namespace VueBugTrackerProject.Classes
{
	public class BugPreviewViewModel
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
		public string Severity { get; set; }
	
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
		/// The status of the bug, determining whether it is open or closed.
		/// </summary>
		public string Status { get; set; }
	
		/// <summary>
		/// The number of comments and status updates the bug has.
		/// </summary>
		public int NumberOfComments { get; set; }
	
		/// <summary>
		/// The date the bug was last updated.
		/// </summary>
		public DateTime DateModified { get; set; }

        public BugPreviewViewModel(Bug bug)
        {
            ID = bug.ID;
			Summary = bug.Summary;
			CreatorID = bug.Creator.Id;
			CreatorName = bug.Creator.UserName;
			CreatorIcon = bug.Creator.Icon;
			NumberOfComments = bug.Comments.Count;
			DateModified = bug.DateModified;

			switch (bug.Severity)
			{
				case Classes.Severity.Low:
					Severity = "Low";
					break;
				case Classes.Severity.Medium:
					Severity = "Medium";
					break;
				case Classes.Severity.High:
					Severity = "High";
					break;
			}

			if (bug.Status == Classes.Status.Closed)
				Status = "Closed";
			else Status = "Open";
        }

    }
}
