using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Data transfer object for resetting a user's password.
    /// </summary>
    public class PasswordResetDTO
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// The user's new password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The token used to authorise the password reset.
        /// </summary>
        public string PasswordResetToken { get; set; }
    }
}
