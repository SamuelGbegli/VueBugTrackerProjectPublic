namespace VueBugTrackerProject.Classes
{
	public class ProjectDTO
	{
		/// <summary>
		/// The project's name.
		/// </summary>
		public string Name { get; set; }
	
		/// <summary>
		/// The short description of the project.
		/// </summary>
		public string Summary { get; set; }
	
		/// <summary>
		/// A link to the project's site.
		/// </summary>
		public string Link { get; set; }
	
		/// <summary>
		/// Determines who can see the project.
		/// </summary>
		public Visibility Visibility { get; set; }
	
		/// <summary>
		/// The raw, unformatted long description of the project.
		/// </summary>
		public string Description { get; set; }
	
		/// <summary>
		/// The long description of the project, with HTML tags to style
		/// the text.
		/// </summary>
		public string FormattedDescription { get; set; }
	
		/// <summary>
		/// The project's tags.
		/// </summary>
		public List<string> Tags { get; set; }
	
		/// <summary>
		/// The project's ID. Only used when making changes to the project.
		/// 
		/// </summary>
		public string ProjectID { get; set; }
	
	}
}
