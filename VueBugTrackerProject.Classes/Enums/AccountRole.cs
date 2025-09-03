using System.ComponentModel.DataAnnotations;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes the role a user has in the app, and what they can do
    /// </summary>
    public enum AccountRole
    {
        /// <summary>
        /// An account with no elevated privileges.
        /// </summary>
        [Display(Description = "Normal")]
        Normal,
        /// <summary>
        /// Has the same abilities as a Normal user, with the added ability to view a list of all users
        /// and suspend any of them.
        /// </summary>
        [Display(Description = "Administrator")]
        Admin,
        /// <summary>
        /// Has the same privileges as an Admin, and cannot be removed or suspended. A single SuperUser
        /// account is created upon first boot of the application, and the role cannot be given to any
        /// other user.
        /// </summary>
        [Display(Description = "Super user")]
        SuperUser
    }
}
