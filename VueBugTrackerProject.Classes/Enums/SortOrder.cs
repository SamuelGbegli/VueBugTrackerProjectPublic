using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Enum to determine the order projects are sorted.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Sorts projects by their sort condition from lowest to highest.
        /// </summary>
        Ascending,
        /// <summary>
        /// Sorts projects by their sort condition from higwest to lowest.
        /// </summary>
        Descending
    }
}
