using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// View models for a bug's comments.
    /// </summary>
    public class CommentViewModel
    {

        /// <summary>
        /// The ID of the comment.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The comment's text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The date the comment was posted.
        /// </summary>
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// The ID of the account that made the comment.
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// The username of the account that made the comment.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The icon of the account that made the comment.
        /// </summary>
        public string OwnerIcon { get; set; }

        /// <summary>
        /// If true, the comment is an automated status update and cannot be replied to.
        /// </summary>
        public bool IsStatusUpdate { get; set; }

        /// <summary>
        /// If true, the comment has been edited.
        /// </summary>
        public bool Edited { get; set; }

        /// <summary>
        /// If true, the comment that is being replied to has been deleted.
        /// </summary>
        public bool IsCommentReplyDeleted { get; set; }

        /// <summary>
        /// The ID of the comment being replied to.
        /// </summary>
        public string ReplyCommentID { get; set; }

        /// <summary>
        /// A sample of the text of the comment being replied to.
        /// </summary>
        public string ReplyCommentText { get; set; }

        /// <summary>
        /// The ID of the account that made the comment being replied to.
        /// </summary>
        public string ReplyCommentOwnerID { get; set; }

        /// <summary>
        /// The username of the account that made the comment being replied to.
        /// </summary>
        public string ReplyCommentOwnerName { get; set; }

        /// <summary>
        /// The icon of the account that made the comment being replied to.
        /// </summary>
        public string ReplyCommentOwnerIcon { get; set; }

        public CommentViewModel(Comment comment, Comment reply = null)
        {
            ID = comment.ID;
            Text = comment.Text;
            DatePosted = comment.DatePosted;
            OwnerID = comment.Owner.Id;
            OwnerName = comment.Owner.UserName;
            OwnerIcon = comment.Owner.Icon;
            IsStatusUpdate = comment.IsStatusUpdate;
            Edited = comment.Edited;
            IsCommentReplyDeleted = comment.IsCommentReplyDeleted;

            //Assigns reply if comment has one
            if (reply != null)
            {
                ReplyCommentID = reply.ID;

                //Truncates reply text if it's longer than 103 characters
                if (reply.Text.Length > 103)
                    ReplyCommentText = reply.Text.Substring(0, 100) + "...";
                else ReplyCommentText = reply.Text;
                    ReplyCommentOwnerID = reply.Owner.Id;
                ReplyCommentOwnerName = reply.Owner.UserName;
                ReplyCommentOwnerIcon = reply.Owner.Icon;
            }
        }
    }
}
