using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes how severe a bug is in a project.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Severity is of low importance.
        /// </summary>
        Low,
        /// <summary>
        /// Severity is of medium importance.
        /// </summary>
        Medium,
        /// <summary>
        /// Severity is of high importance.
        /// </summary>
        High,
    }
}
