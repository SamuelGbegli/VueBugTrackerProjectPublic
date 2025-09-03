using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes.DTOs
{
    /// <summary>
    /// DTO for changing an account's password.
    /// </summary>
    public class PasswordDTO
    {
        /// <summary>
        /// The user's current password.
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// The user's new password.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
