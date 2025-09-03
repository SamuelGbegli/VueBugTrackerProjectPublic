using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes a comment or status update left on a project.
    /// </summary>
    [Table("Comments")]
    public class Comment
	{
		/// <summary>
		/// Unique identifier for the class.
		/// </summary>
		[Key]
		[Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

		/// <summary>
		/// The comment’s contents.
		/// </summary>
		[Required]
		public string Text { get; set; }

        /// <summary>
        /// The date the comment was created.
        /// </summary>
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime DatePosted { get; set; }

		/// <summary>
		/// A past comment that is being replied to by the comment.
		/// </summary>
		public Comment? CommentReply { get; set; }

		/// <summary>
		/// The user that made the comment.
		/// </summary>
		[Required]
		public Account Owner { get; set; }

        /// <summary>
        /// If true, means the comment is an automated message about a
		/// change to the bug.
        /// </summary>
        public bool IsStatusUpdate { get; set; }

        /// <summary>
        /// If true, means the comment has been edited by the user.
        /// </summary>
        public bool Edited { get; set; }
	
		/// <summary>
		/// Indicates whether a comment that was replied to has been
		/// deleted.
		/// </summary>
		public bool IsCommentReplyDeleted { get; set; }

		/// <summary>
		/// The bug the comment belongs to.
		/// </summary>
        public Bug Bug { get; set; }
    }
}
