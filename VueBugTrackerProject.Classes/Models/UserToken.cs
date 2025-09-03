using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Class for storing user access tokens.
    /// </summary>
    public class UserToken
    {

        /// <summary>
		/// Unique identifier for the class.
		/// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Key { get; set; }

        /// <summary>
        /// The account the tokens belong to.
        /// </summary>
        [Required]
        public Account Account { get; set; }

        /// <summary>
        /// Token for a user to log into the application.
        /// </summary>
        public string? LoginToken { get; set; }
    }
}
