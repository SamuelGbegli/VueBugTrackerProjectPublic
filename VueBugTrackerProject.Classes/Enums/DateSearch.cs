using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Enum to determine whether to sort a project by when it was created
    /// or last updated.
    /// </summary>
    public enum DateSearch
    {
        /// <summary>
        /// Filters projects by when they were created.
        /// </summary>
        CreatedDate,
        /// <summary>
        /// Filters projects by when they were last updated.
        /// </summary>
        LastUpdated

    }
}
