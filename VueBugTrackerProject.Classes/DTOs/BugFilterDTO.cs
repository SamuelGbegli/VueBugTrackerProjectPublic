using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// DTO for filtering a project's bug.
    /// </summary>
    public class BugFilterDTO
    {
        /// <summary>
        /// Search query for the summary of the bug.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Decides whether to filter bugs on date created or last updated.
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
        /// The username of whoever created the bug.
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// Filters bugs based on severity. These are a set of 3 true/false values
        /// in order of "low", "medium" and "high".
        /// </summary>
        public List<Severity> SeverityValues { get; set; }

        /// <summary>
        /// Filters bugs based on whether they are open or closed. These are a set of 2
        /// true/false values in order of "open" and "closed".
        /// </summary>
        public List<Status> StatusValues { get; set; }

        /// <summary>
        /// Determines how the items will be sorted.
        /// </summary>
        public SortType SortType { get; set; }

        /// <summary>
        /// Determines whether the items are sorted in ascending or descending order.
        /// </summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// The page of bugs that will be returned.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The ID of the project the bugs belong to.
        /// </summary>
        public string ProjectID { get; set; }
    }
}
