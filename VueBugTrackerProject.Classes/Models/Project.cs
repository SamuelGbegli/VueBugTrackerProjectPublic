using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes a project in the application.
    /// </summary>
    [Table("Projects")]
    public class Project
	{
		/// <summary>
		/// Unique identifier for the class.
		/// </summary>
		[Key]
		[Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

		/// <summary>
		/// The name of the project.
		/// </summary>
		[Required]
		public string Name { get; set; }

        /// <summary>
        /// The short description of the project.
        /// </summary>
        [Required]
        public string Summary { get; set; }
	
		/// <summary>
		/// A link to the project's main page or where it's hosted.
		/// </summary>
		public string? Link { get; set; }

        /// <summary>
        /// Determines who can see the project.
        /// </summary>
        [Required]
        public Visibility Visibility { get; set; }

        /// <summary>
        /// "The raw, unformatted long description of the project.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The long description of the project, with HTML tags to style the text.
        /// </summary>
        public string? FormattedDescription { get; set; }

		/// <summary>
		/// The account that created and owns the project.
		/// </summary>
		[Required]
		public Account Owner { get; set; }

		/// <summary>
		/// The date the project was created.
		/// </summary>
		[Required]
		[Column(TypeName = "datetime2")]
		public DateTime DateCreated { get; set; }

        /// <summary>
        /// The date the project was last updated.
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime DateModified { get; set; }
	
		/// <summary>
		/// The project's bugs.
		/// </summary>
		public List<Bug> Bugs { get; set; } = new List<Bug>();
	
		/// <summary>
		/// The project's tags.
		/// </summary>
		public List<string> Tags { get; set; } = new List<string>();
	
		/// <summary>
		/// A list of users that can view or edit the project.
		/// </summary>
		public List<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
	
	}
}
