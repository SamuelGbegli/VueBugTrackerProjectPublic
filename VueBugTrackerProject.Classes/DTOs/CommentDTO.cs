namespace VueBugTrackerProject
{
	public class CommentDTO
	{
		/// <summary>
		/// The comment's contents.
		/// </summary>
		public string Text { get; set; }
	
		/// <summary>
		/// The ID of the user making the comment.
		/// </summary>
		public string OwnerID { get; set; }
	
		/// <summary>
		/// The ID of the bug the user is commenting on.
		/// </summary>
		public string BugID { get; set; }
	
		/// <summary>
		/// The ID of the comment the user is replying to, if any.
		/// </summary>
		public string ReplyID { get; set; }
	
		/// <summary>
		/// The ID of the comment. Only used when editing an existing
		/// comment.
		/// </summary>
		public string CommentID { get; set; }
	
	}
}
