using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Stores a list of bug comments, as well as the total number
    /// of comments and its current page.
    /// </summary>
    public class CommentContainer
    {
        /// <summary>
        /// The total number of comments the bug has.
        /// </summary>
        public int TotalComments { get; set; }

        /// <summary>
        /// The current page of commennts the user is on.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Stores the comments that will be seen by the clients.
        /// </summary>
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
    }
}
