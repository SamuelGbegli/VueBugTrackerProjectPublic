using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes an account that can access the application.
    /// </summary>
    [Table("Accounts")]
	public class Account : IdentityUser
	{

        /// <summary>
        /// The icon that will be shown with the account’s username.
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// The role and privileges the account has in the application.
        /// </summary>
        [Required]
        public AccountRole Role { get; set; }

        /// <summary>
        /// "If true, the user cannot log into the application.
        /// </summary>
        [Required]
        public bool Suspended { get; set; }

        /// <summary>
        /// The date and time the account was created.
        /// </summary> 
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }
	
		/// <summary>
		/// A JWT used to verify the user when performing various actions.
		/// </summary>
		public string? LoginToken { get; set; }
	
	}
}
