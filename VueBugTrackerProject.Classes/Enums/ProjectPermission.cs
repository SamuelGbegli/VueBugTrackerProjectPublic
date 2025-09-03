using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes a role a user can have in a project.
    /// </summary>
    public enum ProjectPermission
    {
        /// <summary>
        /// Grants the user permission to view a project. Only used if the project is restricted.
        /// </summary>
        Viewer,
        /// <summary>
        /// Allows the user to add and modify bugs in a project.
        /// </summary>
        Editor,
        /// <summary>
        /// Indicates the user owns the project. This is only assigned when the user creates the project.
        /// </summary>
        Owner

    }
}
