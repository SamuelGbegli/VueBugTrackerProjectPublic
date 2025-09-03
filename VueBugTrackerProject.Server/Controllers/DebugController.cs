using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server.Controllers
{
    /// <summary>
    /// A controller used for testing parts of the application with the Swagger UI
    /// </summary>
    [ApiController]
    [Route("/debug")]
    public class DebugController : ControllerBase
    {

        private readonly DatabaseContext _dbContext;
        private readonly UserManager<Account> _userManager;

        public DebugController(DatabaseContext databaseContext, UserManager<Account> userManager)
        {
            _dbContext = databaseContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Adds comments to a selected bug.
        /// </summary>
        /// <param name="bugId">The bug that will have comments added to it.</param>
        /// <param name="numberOfComments">The number of comments that will be added to the bug.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("addcomments")]
        [Authorize]
        public async Task<IActionResult> AddCommentsToBug([FromQuery] string bugId, [FromQuery] int numberOfComments)
        {
            try
            {
                //Gets bug and comments
                var bug = await _dbContext.Bugs
                    .Include(b => b.Comments)
                    .FirstOrDefaultAsync(b => b.ID == bugId);
                if (bug == null) return NotFound();

                //Adds comments
                for (int i = 0; i < numberOfComments; i++)
                {
                    bug.Comments.Add(new Comment
                    {
                        Owner = await _userManager.GetUserAsync(User),
                        DatePosted = DateTime.UtcNow,
                        Text = $"Test comment {i + 1} of {numberOfComments} created on {DateTime.UtcNow} UTC"
                    });
                }

                //Saves changes and exits
                await _dbContext.SaveChangesAsync();
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Adds projects to the application database.
        /// </summary>
        /// <param name="numberOfProjects">The number of projects that will be created</param>
        /// <returns></returns>
        [HttpGet]
        [Route("addprojects")]
        [AllowAnonymous]
        public async Task<IActionResult> AddProjects([FromQuery] int numberOfProjects)
        {
            try
            {
                //Generates projects, each with a random project owner
                for (int i = 0; i < numberOfProjects; i++)
                {
                    await _dbContext.Projects.AddAsync(new Project
                    {
                        Name = $"New project {i+1}/{numberOfProjects}",
                        Summary = $"Created on {DateTime.UtcNow}",
                        Owner = _dbContext.Accounts.ElementAt(Random.Shared.Next(_dbContext.Accounts.Count())),
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                        Visibility = Visibility.Public
                    });
                }
                await _dbContext.SaveChangesAsync();
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
