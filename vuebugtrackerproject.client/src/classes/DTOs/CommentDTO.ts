export default class CommentDTO{

	// The comment's contents.
	text: string = "";
	// The ID of the user making the comment.
	ownerID: string = "";
	// The ID of the bug the user is commenting on.
	bugID: string = "";
	// The ID of the comment the user is replying to, if any.
	replyID: string = "";
	// The ID of the comment. Only used when editing an existing
	// comment.
	commentID: string = "";
}
