using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server.Controllers
{
    /// <summary>
    /// Controller for managing bugs.
    /// </summary>
    [ApiController]
    [Route("/bugs")]
    public class BugController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly UserManager<Account> _userManager;

        public BugController(DatabaseContext databaseContext, UserManager<Account> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets a preview of a project's bugs.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbugpreviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectBugPreviews([FromQuery] string projectId, [FromQuery] int page)
        {
            try
            {
                var project = await _databaseContext.Projects
                    .Include(p => p.Bugs)
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == projectId);
                if (project == null) return NotFound();

                // For restricted projects, checks if user has been granted permission to view it
                if (project.Visibility == Visibility.Restricted)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null) return Unauthorized("Restricted project");

                    //Bounces user if they do not own the project or have permission to view it
                    if (project.Owner != currentUser && !project.UserPermissions.Any(up => up.Account == currentUser))
                        return Forbid();
                }

                //For projects only visible to logged in users, checks if user is logged in
                if (project.Visibility == Visibility.LoggedInOnly)
                {
                    if (!User.Identity.IsAuthenticated) return Unauthorized("Login reqired");
                }

                //Creates and returns a list of bug summaries
                var bugPreviews = new List<BugPreviewViewModel>();
                foreach (var bug in project.Bugs.OrderByDescending(b => b.DateModified).Skip((page - 1) * 20).Take(20))
                {
                    bugPreviews.Add(new BugPreviewViewModel(bug));
                }
                return Ok(bugPreviews);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets a preview of a project's bugs, filtered and sorted by the user.
        /// </summary>
        /// <param name="bugFilterDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getbugpreviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectBugPreviews([FromBody] BugFilterDTO bugFilterDTO)
        {
            try
            {
                //Gets project to ensure it exists
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == bugFilterDTO.ProjectID);
                if (project == null) return NotFound();

                // For restricted projects, checks if user has been granted permission to view it
                if (project.Visibility == Visibility.Restricted)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null) return Unauthorized("Restricted project");

                    //Bounces user if they do not own the project or have permission to view it
                    if (project.Owner != currentUser && !project.UserPermissions.Any(up => up.Account == currentUser))
                        return Forbid();
                }

                //For projects only visible to logged in users, checks if user is logged in
                if (project.Visibility == Visibility.LoggedInOnly)
                {
                    if (!User.Identity.IsAuthenticated) return Unauthorized("Login reqired");
                }

                //Gets bugs
                var bugs = await _databaseContext.Bugs
                    .Include(b => b.Creator)
                    .Where(b => b.Project == project)
                    .ToListAsync();

                //Filtering starts here
                //1. Filters bugs based on summary
                if (!string.IsNullOrWhiteSpace(bugFilterDTO.Summary))
                {
                    //Query is split by spacing and made lower case
                    var queries = bugFilterDTO.Summary.ToLower().Split(' ');
                    //Searches for matches in summary and description
                    bugs = bugs.Where(b => queries.Any(q =>
                        b.Summary.ToLower().Contains(q) ||
                        b.Description.ToLower().Contains(q)))
                        .ToList();
                }

                //2. Filters bugs on creator name
                if (!string.IsNullOrWhiteSpace(bugFilterDTO.CreatorName))
                {
                    //Splits creator name query by space and make lower case
                    var queries = bugFilterDTO.CreatorName.ToLower().Split(' ');

                    bugs = bugs.Where(b => queries.Any(q =>
                        b.Creator.UserName.ToLower().Contains(q)))
                        .ToList();
                }

                //3. Filters on severity and status 
                if(bugFilterDTO.SeverityValues.Count > 0)
                    bugs = bugs.Where(b => bugFilterDTO.SeverityValues.Contains(b.Severity)).ToList();
    
                if(bugFilterDTO.StatusValues.Count > 0)
                    bugs = bugs.Where(b => bugFilterDTO.StatusValues.Contains(b.Status)).ToList();

                //4. Filters bug by date range
                //Skips if both date values are null
                if (bugFilterDTO.DateFrom != null || bugFilterDTO.DateEnd != null)
                {
                    //All filter options check whether to search for the date the 
                    //project was created or modified

                    //Filters projects from a certain date if DateEnd value is null
                    if (bugFilterDTO.DateEnd == null)
                        bugs = bugs.Where(b =>
                            bugFilterDTO.DateSearch == DateSearch.CreatedDate ?
                            b.DateCreated >= bugFilterDTO.DateFrom :
                            b.DateModified >= bugFilterDTO.DateFrom).ToList();

                    //Filters projects up to a certain date if DateFrom value is null
                    else if (bugFilterDTO.DateFrom == null)
                        bugs = bugs.Where(b =>
                            bugFilterDTO.DateSearch == DateSearch.CreatedDate ?
                            b.DateCreated <= bugFilterDTO.DateEnd :
                            b.DateModified <= bugFilterDTO.DateEnd).ToList();

                    //Filters projects on range between the earliest and latest dates
                    else
                    {
                        //Stores date ranges as a list
                        var dates = new List<DateTime> { bugFilterDTO.DateFrom.Value, bugFilterDTO.DateEnd.Value };

                        bugs = bugs.Where(b =>
                            bugFilterDTO.DateSearch == DateSearch.CreatedDate ?
                            b.DateCreated >= dates.Min() && b.DateCreated <= dates.Max() :
                            b.DateModified >= dates.Min() && b.DateModified <= dates.Max()).ToList();
                    }
                }

                //5. Sorts bugs by order specified
                switch (bugFilterDTO.SortType)
                {
                    case SortType.Name:
                        if (bugFilterDTO.SortOrder == SortOrder.Ascending)
                            bugs = bugs.OrderBy(b => b.Summary).ToList();
                        else bugs = bugs.OrderByDescending(b => b.Summary).ToList();
                        break;

                    case SortType.CreatedDate:
                        if (bugFilterDTO.SortOrder == SortOrder.Ascending)
                            bugs = bugs.OrderBy(b => b.DateCreated).ToList();
                        else bugs = bugs.OrderByDescending(b => b.DateCreated).ToList();
                        break;

                    case SortType.LastUpdated:
                        if (bugFilterDTO.SortOrder == SortOrder.Ascending)
                            bugs = bugs.OrderBy(b => b.DateModified).ToList();
                        else bugs = bugs.OrderByDescending(b => b.DateModified).ToList();
                        break;
                }

                //Creates and returns a container with bug summaries
                var bugPreviewContainer = new BugPreviewContainer();
                bugPreviewContainer.CurrentPage = bugFilterDTO.PageNumber;
                bugPreviewContainer.NumberOfBugs = bugs.Count;

                foreach (var bug in bugs.OrderByDescending(b => b.DateModified).Skip((bugFilterDTO.PageNumber - 1) * 20).Take(20))
                {
                    bugPreviewContainer.BugPreviews.Add(new BugPreviewViewModel(bug));
                }

                return Ok(bugPreviewContainer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("get/{bugId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBug(string bugId)
        {
            try
            {
                //Searches for bug based on ID
                var bug = await _databaseContext.Bugs
                    .Include(b => b.Project)
                    .Include(b => b.Creator)
                    .FirstOrDefaultAsync(b => b.ID == bugId);

                //Throws not found if bug does not exist
                if (bug == null) return NotFound();

                //For restricted projects, checks if user has been granted permission to view it
                if (bug.Project.Visibility == Visibility.Restricted)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null) return Unauthorized("Restricted project");

                    //Bounces user if they do not own the project or have permission to view it
                    if (bug.Project.Owner != currentUser && !bug.Project.UserPermissions.Any(up => up.Account == currentUser))
                        return Forbid();
                }

                //For projects only visible to logged in users, checks if user is logged in
                if (bug.Project.Visibility == Visibility.LoggedInOnly)
                {
                    if (!User.Identity.IsAuthenticated) return Unauthorized("Login reqired");
                }

                return Ok(new BugViewModel(bug));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets the number of bugs in a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getnumberofbugs/{projectId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectBugCount(string projectId)
        {
            try
            {
                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .FirstOrDefaultAsync(p => p.ID == projectId);
                if (project == null) return NotFound();

                //Ensures users not logged in can only see public projects
                if (!User.Identity.IsAuthenticated)
                {
                    if (project.Visibility != Visibility.Public) return Unauthorized();
                }

                //If project is restricted, checks if user owns project or can view it
                if (project.Visibility == Visibility.Restricted)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (project.Owner != user && project.UserPermissions.Any(u => u.Account == user))
                        return Forbid();
                }

                //Returns nubmer of bugs in project
                return Ok(project.Bugs.Count());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Adds a bug to a project.
        /// </summary>
        /// <param name="bugDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addbug")]
        [Authorize]
        public async Task<IActionResult> AddBug([FromBody] BugDTO bugDTO)
        {
            try
            {
                //Gets user
                var account = await _userManager.GetUserAsync(User);

                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == bugDTO.ProjectID);
                if (project == null) return NotFound();

                //Checks if user owns the project or has editing privilages

                //Gets account's user permission, if any
                var userPermission = await _databaseContext.UserPermissions
                    .Include(up => up.Account)
                    .Include(up => up.Project)
                    .FirstOrDefaultAsync(up => up.Account.Id == account.Id);

                //Ensures user either owns the project or has the editor role in the project
                if (account != project.Owner && (userPermission == null || userPermission.Permission != ProjectPermission.Editor))
                    return Forbid();

                //Creates and adds bug
                var bug = new Bug
                {
                    Summary = bugDTO.Summary,
                    Description = bugDTO.Description,
                    Severity = bugDTO.Severity,
                    Status = Status.Open,
                    Creator = account,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };

                bug.Comments.Add(new Comment
                {
                    Owner = account,
                    Text = $"Created bug on {DateTime.UtcNow}",
                   IsStatusUpdate = true,
                   DatePosted = DateTime.UtcNow                   
                });

                project.Bugs.Add(bug);
                project.DateModified = DateTime.UtcNow;
                await _databaseContext.SaveChangesAsync();

                return Created();


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates a bug in the database.
        /// </summary>
        /// <param name="bugDTO"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("updatebug")]
        [Authorize]
        public async Task<IActionResult> UpdateBug([FromBody] BugDTO bugDTO)
        {
            try
            {

                //Gets user account
                var account = await _userManager.GetUserAsync(User);

                //Gets project
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .FirstOrDefaultAsync(b => b.ID == bugDTO.ProjectID);
                if (project == null) return NotFound();

                //Checks if bug exists in project
                var bug = await _databaseContext.Bugs
                    .Include(b => b.Comments)
                    .FirstOrDefaultAsync(b => b.ID == bugDTO.BugID);
                if (bug == null) return NotFound();

                //Checks if user owns the project or created the bug
                if (project.Owner != account && bug.Creator != account) return Forbid();

                //Adds status update if bug severity changes
                if(bug.Severity != bugDTO.Severity)
                    bug.Comments.Add(new Comment {
                        Owner = account,
                        Text = $"Changed bug severity to {(bugDTO.Severity == Severity.Low ? "Low" : bugDTO.Severity == Severity.Medium ? "Medium" : "High")}",
                        IsStatusUpdate = true,
                        DatePosted = DateTime.UtcNow
                    });

                //Updates bug
                bug.Summary = bugDTO.Summary;
                bug.Description = bugDTO.Description;
                bug.Status = bugDTO.IsOpen ? Status.Open : Status.Closed;
                bug.DateModified = DateTime.UtcNow;
                project.DateModified = DateTime.UtcNow;

                //Saves changes
                await _databaseContext.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Opens or closes a bug.
        /// </summary>
        /// <param name="bugDTO"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("togglebugstatus")]
        [Authorize]
        public async Task<IActionResult> ToggleBugStatus([FromBody] string bugId)
        {
            try
            {
                //Gets user account
                var account = await _userManager.GetUserAsync(User);


                //Gets bug
                var bug = await _databaseContext.Bugs
                    .Include(b => b.Creator)
                    .Include(b => b.Project)
                    .FirstOrDefaultAsync(b => b.ID == bugId);
                if (bug == null) return NotFound();

                //Gets project
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == bug.Project.ID);
                if (project == null) return NotFound();

                //Checks if user owns the project or created the bug
                if (project.Owner != account && bug.Creator != account) return Forbid();

                //Updates bug
                bug.Status = bug.Status == Status.Closed ? Status.Open : Status.Closed;

                bug.Comments.Add(new Comment
                {
                    Owner = account,
                    Text = $"{(bug.Status == Status.Open ? "Reopened" : "Closed")} the bug.",
                    IsStatusUpdate = true,
                    DatePosted = DateTime.UtcNow
                });

                bug.DateModified = DateTime.UtcNow;
                project.DateModified = DateTime.UtcNow;

                //Saves changes
                await _databaseContext.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a bug from a project.
        /// </summary>
        /// <param name="bugDTO"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletebug")]
        [Authorize]
        public async Task<IActionResult> DeleteBug([FromBody] BugDTO bugDTO)
        {
            try
            {
                //Bounces user if user is not logged in
                if (!User.Identity.IsAuthenticated) return Unauthorized();

                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .FirstOrDefaultAsync(p => p.ID == bugDTO.ProjectID);
                if (project == null) return NotFound();

                //Gets bug in project
                var bug = await _databaseContext.Bugs
                    .Include(b => b.Comments)
                    .FirstOrDefaultAsync(b => b.ID == bugDTO.BugID);
                if (bug == null) return NotFound();

                //Gets user account
                var account = await _userManager.GetUserAsync(User);

                //Bounces user if they did not create the bug or are not the project owner
                if (project.Owner != account && bug.Creator != account) return Forbid();

                //Removes bug and saves changes
                bug.Comments.Clear();
                project.Bugs.Remove(bug);
                project.DateModified = DateTime.UtcNow;
                await _databaseContext.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
