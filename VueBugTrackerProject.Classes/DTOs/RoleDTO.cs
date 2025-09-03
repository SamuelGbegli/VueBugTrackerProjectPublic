using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// DTO to update an account's role.
    /// </summary>
    public class RoleDTO
    {
        /// <summary>
        /// The ID of the account whose role will be updated.
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// The role the user will have.
        /// </summary>
        public AccountRole Role { get; set; }
    }
}
