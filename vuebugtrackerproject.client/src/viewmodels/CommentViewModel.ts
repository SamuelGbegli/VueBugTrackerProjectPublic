export default class CommentViewModel{

	// Unique identifier for the comment.
	id: string = "";
	// The comment's content.
	text: string = "";
	// The date the comment was created.
	datePosted: Date = new Date();
	// The ID of the user that made the comment.
	ownerID: string = "";
	// The name of the user that made the comment.
	ownerName: string = "";
	// The icon of the user that made the comment.
	ownerIcon: string = "";
	// If true, means the comment is an automated message about a
	// change to the bug.
	isStatusUpdate: boolean = false;
	// If true, means the comment has been edited by the user.
	edited: boolean = false;
	// Indicates whether a comment that was replied to has been
	// deleted.
	isCommentReplyDeleted: boolean = false;
  // The ID of the comment being replied to.
  replyCommentID: string = "";
  // A sample of the text of the comment being replied to.
  replyCommentText: string = "";
  // The ID of the account that made the comment being replied to.
  replyCommentOwnerID: string = "";
  // The username of the account that made the comment being replied to.
  replyCommentOwnerName: string = "";
  // The icon of the account that made the comment being replied to.
  replyCommentOwnerIcon: string = "";
}
