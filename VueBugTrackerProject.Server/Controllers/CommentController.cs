using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server.Controllers
{
    /// <summary>
    /// Controller for handling a bug's comments and status updates.
    /// </summary>
    [ApiController]
    [Route("/comments")]
    public class CommentController : ControllerBase
    {

        private readonly DatabaseContext _dbContext;
        private readonly UserManager<Account> _userManager;

        public CommentController(DatabaseContext databaseContext, UserManager<Account> userManager)
        {
            _dbContext = databaseContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets a bug's comments, in groups of 20.
        /// </summary>
        /// <param name="bugId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{bugId}/{page}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(string bugId, int page)
        {
            try
            {
                //Gets comments
                var comments = await _dbContext.Comments
                    .Include(c => c.Owner)
                    .Include(c => c.CommentReply)
                    .OrderBy(c => c.DatePosted)
                    .Where(c => c.Bug.ID == bugId).ToListAsync();

                //Gets bug
                var bug = await _dbContext.Bugs
                    .Include(b => b.Project)
                    .FirstOrDefaultAsync(b => b.ID == bugId);
                if (bug == null) return NotFound();

                //Gets project
                var project = await _dbContext.Projects
                    .Include(p => p.UserPermissions)
                    .FirstOrDefaultAsync(p => p == bug.Project);

                //Shows unauthorised if project is not public and user is not logged in
                if (project.Visibility != Visibility.Public && !User.Identity.IsAuthenticated)
                    return Unauthorized();

                //Forbids user if project is restricted and they cannot view it
                if (project.Visibility == Visibility.Restricted)
                {
                    //Gets user
                    var user = await _userManager.GetUserAsync(User);

                    if (project.Owner != user && !project.UserPermissions.Any(p => p.Account == user))
                        return Forbid();

                }

                //Creates container to store the number of comments, the current page
                //and the comments the user will see
                var commentContainer = new CommentContainer
                {
                    CurrentPage = page,
                    TotalComments = comments.Count
                };

                //Adds up to 20 comments to be sent to the backend
                foreach (var comment in comments.Skip(20 * (page - 1)).Take(20)) {
                    if (comment.CommentReply == null)
                        commentContainer.Comments.Add(new CommentViewModel(comment));
                    else
                    {
                        var reply = comments.Find(c => c == comment.CommentReply);
                        commentContainer.Comments.Add(new CommentViewModel(comment, reply));
                    }
                }

                return Ok(commentContainer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Adds a comment to the database.
        /// </summary>
        /// <param name="commentDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentDTO commentDTO)
        {
            try
            {
                //Checks if bug exists
                var bug = await _dbContext.Bugs
                    .Include(b => b.Comments)
                    .Include(b => b.Project)
                    .FirstOrDefaultAsync(b => b.ID == commentDTO.BugID);
                if (bug == null) return NotFound();

                //Gets project
                var project = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.UserPermissions)
                    .FirstOrDefaultAsync(p => p == bug.Project);

                //Gets user
                var account = await _userManager.GetUserAsync(User);

                //Checks if user can add comments to bug
                if (account != project.Owner && !project.UserPermissions.Any(up => up.Account == account && up.Permission == ProjectPermission.Editor))
                    return Forbid();

                //Adds comment
                var comment = new Comment
                {
                    Text = commentDTO.Text,
                    Owner = account,
                    IsStatusUpdate = false,
                    Edited = false,
                    DatePosted = DateTime.UtcNow,
                };

                //Checks if comment is a reply
                if (!string.IsNullOrWhiteSpace(commentDTO.ReplyID))
                    comment.CommentReply = bug.Comments.Find(b => b.ID == commentDTO.ReplyID);

                //Adds comment and saves changes
                bug.Comments.Add(comment);
                project.DateModified = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates a comment.
        /// </summary>
        /// <param name="commentDTO"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> UpdateComment([FromBody] CommentDTO commentDTO)
        {
            try
            {
                //Looks for bug
                var comment = await _dbContext.Comments
                    .Include(c => c.Owner)
                    .FirstOrDefaultAsync(c => c.ID == commentDTO.CommentID);
                if (comment == null) return NotFound();

                //Checks if comment is status update
                if (comment.IsStatusUpdate) return Forbid();

                //Checks if user made the comment
                var account = await _userManager.GetUserAsync(User);
                if (comment.Owner != account) return Forbid();

                //TODO: Possibly block users who no longer access the bug from
                //editing comments

                //Applies edit to comment and saves changes
                comment.Text = commentDTO.Text;
                comment.Edited = true;
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Removes a comment.
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteComment([FromBody] string commentId)
        {
            try
            {
                //Looks for comment
                var comment = await _dbContext.Comments
                    .Include(c => c.Owner)
                    .FirstOrDefaultAsync (c => c.ID == commentId);

                if (comment == null) return NotFound();

                //Checks if owner made the comment
                var account = await _userManager.GetUserAsync(User);
                if(account != comment.Owner) return Forbid();

                //Goes through all the comment's replies and indicates the source comment
                //has been removed
                var replies = await _dbContext.Comments
                    .Include(c => c.CommentReply)
                    .Where(c => c.CommentReply == comment).ToListAsync();
                foreach (var reply in replies) 
                    reply.IsCommentReplyDeleted = true;

                //Removes comment and saves changes
                _dbContext.Comments.Remove(comment);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Checks if the user can add a comment to a bug
        /// </summary>
        /// <param name="projectId">The bug's ID.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("canadd/{bugid}")]
        [Authorize]
        public async Task<IActionResult> CanUserAddBug(string bugId)
        {
            try
            {
                //Checks if user is logged in
                if (!User.Identity.IsAuthenticated) return Unauthorized();
                var account = await _userManager.GetUserAsync(User);

                //Lookks for bug
                var bug = await _dbContext.Bugs
                    .Include(b => b.Project)
                    .FirstOrDefaultAsync(b => b.ID == bugId);
                if(bug == null) return NotFound();

                //Looks for project
                var project = await _dbContext.Projects
                    .FirstOrDefaultAsync(p => p.ID == bug.Project.ID);
                if (project == null) return NotFound();

                //Gets user permissions
                var userPermissions = await _dbContext.UserPermissions
                    .Include(up => up.Account)
                    .Where(up => up.Project == project)
                    .ToListAsync();

                //Denies request if user did not create project or does not have permission to view a restricted project
                if (project.Owner != account)
                {
                    var permission = userPermissions.FirstOrDefault(up => up.Account == account);
                    if (permission == null || ((int)permission.Permission == -1) ) return Unauthorized();
                }

                //Sends true back to the client
                return Ok(true);
            }
            catch (Exception ex)
            {
                //Something went wrong, send error message
                return StatusCode(500, ex.Message);
            }
        }

    }
}
