using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes a project's bug.
    /// </summary>
    [Table("Bugs")]
    public class Bug
	{
        /// <summary>
        /// Unique identifier for the class.
        /// </summary>
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

        /// <summary>
        /// A short description of the bug.
        /// </summary>
        [Required]
        public string Summary { get; set; }

        /// <summary>
        /// The severity of the bug.
        /// </summary>
        [Required]
        public Severity Severity { get; set; }

        /// <summary>
        /// A longer, more detailed description of the bug.
        /// </summary>
		[Required]
        public string Description { get; set; }

        /// <summary>
        /// The user that created the bug.
        /// </summary>
        [Required]
        public Account Creator { get; set; }

        /// <summary>
        /// A list of the bug’s comments and status updates.
        /// </summary>
        [Required]
        public List<Comment> Comments { get; set; } = new List<Comment>();

        /// <summary>
        /// The date the bug was created.
        /// </summary>
        [Required]
        [Column(TypeName ="datetime2")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The date the bug was last updated.
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime DateModified { get; set; }

        /// <summary>
        /// Determines whether the bug is open or closed.
        /// </summary>
        [Required]
        public Status Status { get; set; }

        /// <summary>
        /// The project the bug belongs to.
        /// </summary>
        [Required]
        public Project Project { get; set; }

    }
}
