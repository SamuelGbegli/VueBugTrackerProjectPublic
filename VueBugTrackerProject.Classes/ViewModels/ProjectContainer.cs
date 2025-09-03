using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Container for returning a set of projects from the server.
    /// </summary>
    public class ProjectContainer
    {
        /// <summary>
        /// The number of projects the user can see.
        /// </summary>
        public int TotalProjects { get; set; }

        /// <summary>
        /// The projects the user can view.
        /// </summary>
        public List<ProjectPreviewViewModel> Projects { get; set; } = new List<ProjectPreviewViewModel>();
    }
}
