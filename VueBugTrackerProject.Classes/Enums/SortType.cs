using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Enum for deciding what to sort projects on.
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// Sorts projects by their name.
        /// </summary>
        Name,
        /// <summary>
        /// Sorts projects by when they were created.
        /// </summary>
        CreatedDate,
        /// <summary>
        /// Sorts projects by when they were last updated.
        /// </summary>
        LastUpdated
    }
}
