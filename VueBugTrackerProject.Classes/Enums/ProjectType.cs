using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Descibes the types of projects being search for.
    /// </summary>
    public enum ProjectType
    {
        /// <summary>
        /// Gets all projects.
        /// </summary>
        All,
        /// <summary>
        /// Only gets projects with no bugs that are open.
        /// </summary>
        NoOpenBugs,
        /// <summary>
        /// Only gets projects with at least one open bug.
        /// </summary>
        OpenBugsOnly
    }
}
