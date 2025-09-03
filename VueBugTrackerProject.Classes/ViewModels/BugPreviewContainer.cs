using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Stores a list of bug previews for a project, as well as the number of bugs
    /// that match a filter query and the page of filtered bugs being accessed.
    /// </summary>
    public class BugPreviewContainer
    {
        /// <summary>
        /// The total number of bugs based on the filter given.
        /// </summary>
        public int NumberOfBugs { get; set; }
        /// <summary>
        /// The current page of filtered bugs being viewed.
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// The list of bug previews that will be shown to the user.
        /// </summary>
        public List<BugPreviewViewModel> BugPreviews { get; set; } = new List<BugPreviewViewModel>();
    }
}
