using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VueBugTrackerProject.Classes;
using Microsoft.EntityFrameworkCore;

namespace VueBugTrackerProject.Server.Controllers
{
    /// <summary>
    /// Controller for viewing and managing projects.
    /// </summary>
    [ApiController]
    [Route("/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<Account> _userManager;

        public ProjectController(DatabaseContext context, UserManager<Account> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Adds a new project.
        /// </summary>
        /// <param name="projectDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDTO projectDTO)
        {
            try
            {
                //Ensures user is logged in
                var account = await _userManager.GetUserAsync(User);
                if (account == null) return BadRequest("User does not exist");

                //Creates new project
                var project = new Project
                {
                    Name = projectDTO.Name,
                    Summary = projectDTO.Summary,
                    Link = projectDTO.Link,
                    Visibility = projectDTO.Visibility,
                    Description = projectDTO.Description,
                    FormattedDescription = projectDTO.FormattedDescription,
                    Tags = projectDTO.Tags,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Owner = account
                };

                //Adds user permission for project owner
                project.UserPermissions.Add(new UserPermission
                {
                    Account = account,
                    Permission = ProjectPermission.Owner
                });

                //Adds project to database
                await _dbContext.Projects.AddAsync(project);
                await _dbContext.SaveChangesAsync();

                return Created($"project/{project.ID}", projectDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of projects in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjects([FromQuery] int pageNumber = 1)
        {
            try
            {

                var projectPreviews = new List<ProjectPreviewViewModel>();

                //Gets all projects, with their owners and lists of bugs
                var projects = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .ToListAsync();

                //If user is not logged in, only return projects that can be seen by all
                if (!User.Identity.IsAuthenticated)
                    projects = projects.Where(p => p.Visibility == Visibility.Public).ToList();
                else
                {
                    //If user is logged in, return projects that are public, available to logged in users only
                    //or are hidden and the user is allowed to view or edit them
                    var account = await _userManager.GetUserAsync(User);
                    projects = projects.Where(p => p.Visibility != Visibility.Restricted || p.UserPermissions.Any(up => up.Account == account)).ToList();
                }

                var projectContainer = new ProjectContainer { TotalProjects = projects.Count };

                //Creates view models based on projects found
                foreach (var project in projects.OrderByDescending(p => p.DateModified).Skip(30 * (pageNumber - 1)).Take(30))
                {
                    projectContainer.Projects.Add(new ProjectPreviewViewModel(project));
                }

                //Returns the projects
                return Ok(projectContainer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Returns a list of sorted and filtered projects from the database.
        /// </summary>
        /// <param name="filterDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProjects([FromBody] FilterDTO filterDTO)
        {
            try
            {
                var projectPreviews = new List<ProjectPreviewViewModel>();

                //Gets all projects, with their owners and lists of bugs
                var projects = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .ToListAsync();

                //If user is not logged in, only return projects that can be seen by all
                if (!User.Identity.IsAuthenticated)
                    projects = projects.Where(p => p.Visibility == Visibility.Public).ToList();
                else
                {
                    //If user is logged in, return projects that are public, available to logged in users only
                    //or are hidden and the user is allowed to view or edit them
                    var account = await _userManager.GetUserAsync(User);
                    projects = projects.Where(p => p.Visibility != Visibility.Restricted || p.UserPermissions.Any(up => up.Account == account)).ToList();
                }

                //Filtering starts here
                //1. Searches for projects by name, summary or tag
                if (!string.IsNullOrWhiteSpace(filterDTO.Query))
                {
                    //Splits query by spaces
                    var queries = filterDTO.Query.ToLower().Split(' ');
                    //Checks if project name, summary or tags matches with query text
                    //Values are converted to lower case
                    projects = projects.Where(p =>
                    queries.Any(q => p.Name.ToLower().Contains(q)
                        || p.Summary.ToLower().Contains(q)
                        || p.Tags.Any(t => t.ToLower() == q))).ToList();

                }
                //2. Checks if projects have any open bugs or none
                switch (filterDTO.ProjectType)
                {
                    //Returns all projects with at least one open bug
                    case ProjectType.OpenBugsOnly:
                        projects = projects.Where(p => p.Bugs.Any(b => b.Status == Status.Open)).ToList();
                        break;
                    //Return all projects where all bugs are closed
                    //Maty not return projects with 0 bugs
                    case ProjectType.NoOpenBugs:
                        projects = projects.Where(p => p.Bugs.All(b => b.Status == Status.Closed)).ToList();
                        break;
                }

                //3. Filters projects by date range
                //Skips if both date values are null
                if (filterDTO.DateFrom != null || filterDTO.DateEnd != null)
                {
                    //All filter options check whether to search for the date the 
                    //project was created or modified

                    //Filters projects from a certain date if DateEnd value is null
                    if (filterDTO.DateEnd == null)
                        projects = projects.Where(p =>
                            filterDTO.DateSearch == DateSearch.CreatedDate ?
                            p.DateCreated >= filterDTO.DateFrom :
                            p.DateModified >= filterDTO.DateFrom).ToList();

                    //Filters projects up to a certain date if DateFrom value is null
                    else if (filterDTO.DateFrom == null)
                        projects = projects.Where(p =>
                            filterDTO.DateSearch == DateSearch.CreatedDate ?
                            p.DateCreated <= filterDTO.DateEnd :
                            p.DateModified <= filterDTO.DateEnd).ToList();

                    //Filters projects on range between the earliest and latest dates
                    else
                    {
                        //Stores date ranges as a list
                        var dates = new List<DateTime> { filterDTO.DateFrom.Value, filterDTO.DateEnd.Value };

                        projects = projects.Where(p =>
                            filterDTO.DateSearch == DateSearch.CreatedDate ?
                            p.DateCreated >= dates.Min() && p.DateCreated <= dates.Max() :
                            p.DateModified >= dates.Min() && p.DateModified <= dates.Max()).ToList();
                    }
                }

                //4. Sorts projects by order specified
                switch (filterDTO.SortType)
                {
                    case SortType.Name:
                        if (filterDTO.SortOrder == SortOrder.Ascending)
                            projects = projects.OrderBy(p => p.Name).ToList();
                        else projects = projects.OrderByDescending(p => p.Name).ToList();
                        break;

                    case SortType.CreatedDate:
                        if (filterDTO.SortOrder == SortOrder.Ascending)
                            projects = projects.OrderBy(p => p.DateCreated).ToList();
                        else projects = projects.OrderByDescending(p => p.DateCreated).ToList();
                        break;

                    case SortType.LastUpdated:
                        if (filterDTO.SortOrder == SortOrder.Ascending)
                            projects = projects.OrderBy(p => p.DateModified).ToList();
                        else projects = projects.OrderByDescending(p => p.DateModified).ToList();
                        break;
                }

                var projectContainer = new ProjectContainer { TotalProjects = projects.Count };

                //Creates view models based on projects found
                foreach (var project in projects.Skip(30 * (filterDTO.PageNumber - 1)).Take(30))
                {
                    projectContainer.Projects.Add(new ProjectPreviewViewModel(project));
                }

                //Returns the projects
                return Ok(projectContainer);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Returns the number of projects the user can see.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectcount")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNumberOfProjects()
        {
            try
            {
                //Gets all projects
                var projects = await _dbContext.Projects.ToListAsync();

                //If user is not logged in, only return projects that can be seen by all
                if (!User.Identity.IsAuthenticated)
                    projects = projects.Where(p => p.Visibility == Visibility.Public).ToList();
                else
                {
                    //If user is logged in, return projects that are public, available to logged in users only
                    //or are hidden and the user is allowed to view or edit them
                    var account = await _userManager.GetUserAsync(User);
                    projects = projects.Where(p => p.Visibility != Visibility.Restricted || p.UserPermissions.Any(up => up.Account == account)).ToList();
                }

                return Ok(projects.Count);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Returns the 5 most recently edited projects the user can vies.
        /// </summary>
        /// <param name="getUserProjects">If true, only gets projects the user made</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getrecentprojects")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentProjects([FromQuery] bool getUserProjects)
        {
            try
            {
                var projectPreviews = new List<ProjectPreviewViewModel>();

                //Gets all projects, in order of when they were created with their owners and lists of bugs
                var projects = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .OrderByDescending(p => p.DateModified)
                    .ToListAsync();

                if (getUserProjects)
                {
                    //Bounces the user if they are not logged in
                    if (!User.Identity.IsAuthenticated) return Unauthorized();

                    //Gets thw user's account
                    var account = await _userManager.GetUserAsync(User);

                    //Gets projects that the user owns
                    projects = projects.Where(p => p.Owner == account).ToList();
                }
                else
                {
                    //If user is not logged in, only return projects that can be seen by all
                    if (!User.Identity.IsAuthenticated)
                        projects = projects.Where(p => p.Visibility == Visibility.Public).ToList();
                    else
                    {
                        //If user is logged in, return projects that are public, available to logged in users only
                        //or are hidden and the user is allowed to view or edit them
                        var account = await _userManager.GetUserAsync(User);
                        projects = projects.Where(p => p.Visibility != Visibility.Restricted || p.UserPermissions.Any(up => up.Account == account)).ToList();
                    }
                }

                //Creates view models based on the first 5 projects found
                foreach (var project in projects.Take(5))
                {
                    projectPreviews.Add(new ProjectPreviewViewModel(project));
                }
                //Returns the projects
                return Ok(projectPreviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets a project from the database.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{projectId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProject(string projectId)
        {
            try
            {
                //Looks for project by ID
                var project = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .FirstOrDefaultAsync(p => p.ID == projectId);

                //Returns error if project does not exist
                if (project == null) return NotFound("Project does not exist");

                //For restricted projects, checks if user has been granted permission to view it
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

                //Returns project view model
                return Ok(new ProjectViewModel(project));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="projectDTO"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDTO projectDTO)
        {
            try
            {
                //Looks for project by ID
                var project = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == projectDTO.ProjectID);

                //Returns error if project does not exist
                if (project == null) return NotFound("Project does not exist");

                //For restricted projects, checks if user has been granted permission to view it
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

                //Updates project and saves changes
                project.Name = projectDTO.Name;
                project.Summary = projectDTO.Summary;
                project.Visibility = projectDTO.Visibility;
                project.Link = projectDTO.Link;
                project.Description = projectDTO.Description;
                project.FormattedDescription = projectDTO.FormattedDescription;
                project.Tags = projectDTO.Tags;
                project.DateModified = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteProject([FromBody] string projectID)
        {
            try
            {
                //Ensures user is logged in
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return BadRequest();

                //Bounces if project is the one used to record bugs in this application
                if (projectID == "bff592a4-c7f6-43d1-9640-469ee5d4da1e") return Forbid();

                //Looks for project by ID
                var projectToDelete = await _dbContext.Projects.FindAsync(projectID);

                //Bounces if project does not exist with ID given
                if (projectToDelete == null) return NotFound("Project does not exist");

                //Bounces if user does not own project
                if (projectToDelete.Owner != currentUser) return Forbid();

                //Removes project from database
                _dbContext.Projects.Remove(projectToDelete);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Checks if the user has editing priviliges for a project.
        /// </summary>
        /// <param name="projectId">The project's ID.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("canedit/{projectId}")]
        [Authorize]
        public async Task<IActionResult> CanUserEditProject(string projectId)
        {
            try
            {
                //Checks if user is logged in
                if (!User.Identity.IsAuthenticated) return Unauthorized();
                var account = await _userManager.GetUserAsync(User);

                //Looks for project
                var project = await _dbContext.Projects
                    .FirstOrDefaultAsync(p => p.ID == projectId);
                if (project == null) return NotFound();

                //Gets user permissions
                var userPermissions = await _dbContext.UserPermissions
                    .Include(up => up.Account).ToListAsync();

                //Denies request if user did not create project or does not have permission to edit the project
                if (project.Owner != account)
                {
                    var permission = userPermissions.FirstOrDefault(up => up.Account == account);
                    if (permission == null || permission.Permission != ProjectPermission.Editor) return Unauthorized();
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

        /// <summary>
        /// Checks if the user can view or edit a project.
        /// </summary>
        /// <param name="projectId">The project's ID.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("canview/{projectId}")]
        [Authorize]
        public async Task<IActionResult> CanUserViewProject(string projectId)
        {
            try
            {
                //Checks if user is logged in
                if (!User.Identity.IsAuthenticated) return Unauthorized();
                var account = await _userManager.GetUserAsync(User);

                //Looks for project
                var project = await _dbContext.Projects
                    .FirstOrDefaultAsync(p => p.ID == projectId);
                if (project == null) return NotFound();

                //Gets user permissions
                var userPermissions = await _dbContext.UserPermissions
                    .Include(up => up.Account).ToListAsync();

                //Denies request if user did not create project or does not have permission to view a restricted project
                if (project.Owner != account)
                {
                    var permission = userPermissions.FirstOrDefault(up => up.Account == account);
                    if (permission == null) return Unauthorized();
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

        /// <summary>
        /// Function to create dummy projects for testing.  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("createtestprojects")]
        [Authorize]
        public async Task<IActionResult> CreateTestProjects()
        {
            try
            {
                var account = await _userManager.GetUserAsync(User);
                for (int i = 1; i <= 4; i++)
                {
                    //Creates new project
                    var project = new Project
                    {
                        Name = $"Test project {i}",
                        Summary = $"Test project {i}",
                        Visibility = Visibility.Public,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                        Owner = account
                    };

                    for (int j = 1; j <= 25 * i; j++)
                    {
                        //Adds dummy bug to project for testing
                        project.Bugs.Add(new Bug
                        {
                            Summary = $"Test bug {j}",
                            Severity = Severity.Low,
                            Status = Status.Open,
                            Description = $"This is dummy bug {j} for test job {i}",
                            Creator = account,
                            DateCreated = DateTime.UtcNow,
                            DateModified = DateTime.UtcNow,
                        });
                    }
                    await _dbContext.Projects.AddAsync(project);
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

