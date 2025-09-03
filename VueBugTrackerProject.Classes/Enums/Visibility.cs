using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes who is allowed to view a project in the app.
    /// </summary>
    public enum Visibility
    {
        /// <summary>
        /// Anyone can view the project, regardless of whether they are logged in.
        /// </summary>
        Public,
        /// <summary>
        /// The project can only be seen by users that have logged in.
        /// </summary>
        LoggedInOnly,
        /// <summary>
        /// Only the project owner and any users they have granted permission can view
        /// the project.
        /// </summary>
        Restricted,

    }
}
