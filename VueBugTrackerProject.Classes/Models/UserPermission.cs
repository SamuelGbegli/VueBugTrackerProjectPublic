using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes what permissions a user have with a project they
    /// didn't create.
    /// </summary>

    [Table("UserPermissions")]
    public class UserPermission
	{
		/// <summary>
		/// Unique identifier for the class.
		/// </summary>
		[Key]
		[Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

		/// <summary>
		/// The target project.
		/// </summary>
		[Required]
		public Project Project { get; set; }

		/// <summary>
		/// The user who is granted access to the project.
		/// </summary>
		[Required]
		public Account Account { get; set; }

		/// <summary>
		/// What the user can do with the project.
		/// </summary>
		[Required]
		public ProjectPermission Permission { get; set; }
	
	}
}
