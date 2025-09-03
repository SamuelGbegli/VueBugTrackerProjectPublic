using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject
{
	public class FilterDTO
	{
		/// <summary>
		/// The sting that is used to search for projects.
		/// </summary>
		public string Query { get; set; }
	
		/// <summary>
		/// Filters projects if they have any open bugs or not.
		/// </summary>
		public ProjectType ProjectType { get; set; }
	
		/// <summary>
		/// Determines to search for a project based on the date it was
		/// created or last updated.
		/// </summary>
		public DateSearch DateSearch { get; set; }
	
		/// <summary>
		/// The starting date search range.
		/// </summary>
		public DateTime? DateFrom { get; set; }
	
		/// <summary>
		/// The end date search range.
		/// </summary>
		public DateTime? DateEnd { get; set; }
	
		/// <summary>
		/// Determines how the items will be sorted.
		/// </summary>
		public SortType SortType { get; set; }
	
		/// <summary>
		/// Determines whether the items are sorted in ascending or
		/// descending order.
		/// </summary>
		public SortOrder SortOrder { get; set; }

		/// <summary>
		/// The page of projects that will be returned.
		/// </summary>
        public int PageNumber { get; set; }
    }
}
