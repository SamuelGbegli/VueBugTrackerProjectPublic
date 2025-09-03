using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server.Controllers
{
    /// <summary>
    /// Controller for adding, viewing and editing user permmissions
    /// for a project.
    /// </summary>
    [ApiController]
    [Route("userpermissions")]
    public class UserPermissionController : ControllerBase
    {
       private readonly UserManager<Account> _userManager;
       private readonly DatabaseContext _databaseContext;

        public UserPermissionController(UserManager<Account> userManager, DatabaseContext databaseContext)
        {
            _userManager = userManager;
            _databaseContext = databaseContext;
        }

        /// <summary>
        /// Returns a list of user permission data.
        /// </summary>
        /// <param name="projectID">THe ID of the project that will be selected.\</param>
        /// <param name="pageNumber">The page of prermissions that will be returned.validate</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Authorize]
        public async Task<IActionResult> GetUserPermissions([FromQuery] string projectID, [FromQuery] int pageNumber)
        {
            try
            {
                //Checks if project exists
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == projectID);
                if (project == null) return NotFound();

                //Checks if user owns project
                var account = await _userManager.GetUserAsync(User);
                if (project.Owner != account) return Forbid();

                //Gets user permissions
                var userPermissions = await _databaseContext.UserPermissions
                    .Include(up => up.Project)
                    .Include(up => up.Account)
                    .Where(up => up.Project.ID == projectID).ToListAsync();                

                //Creates container
                var userPermissionContainer = new UserPermissionContainer
                {
                    TotalPermissions = userPermissions.Count,
                    Pages = (int)Math.Ceiling(userPermissions.Count / (double)20),
                    CurrentPage = pageNumber
                };

                foreach (var userPermission in userPermissions.OrderBy(up => up.Account.UserName).Skip((pageNumber - 1) * 20).Take(20))
                    userPermissionContainer.UserPermissions.Add(new UserPermissionViewModel(userPermission));

                return Ok(userPermissionContainer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Adds a new user permission to the project.
        /// </summary>
        /// <param name="permissionDTO">A DTO for handling user permissions</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        [Authorize]
        public async Task<IActionResult> AddUserPermission([FromBody] UserPermissionDTO permissionDTO)
        {
            try
            {
                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.UserPermissions)
                    .FirstOrDefaultAsync(p => p.ID == permissionDTO.ProjectID);

                //Checks if user owns the project
                var account = await _userManager.GetUserAsync(User);
                if (project.Owner != account) return Forbid();

                //Looks for user
                var userToAdd = await _databaseContext.Accounts.FirstOrDefaultAsync(u => u.UserName.ToLower() == permissionDTO.AccountID.ToLower());
                if (userToAdd == null) return NotFound();

                //Checks if user already has a permission for the project or owns the project
                if(project.UserPermissions.Any(up => up.Account ==  userToAdd) || userToAdd == project.Owner) return BadRequest();

                //Adds and saves permission
                project.UserPermissions.Add(new UserPermission
                {
                    Account = userToAdd,
                    Permission = permissionDTO.Permission
                });
                await _databaseContext.SaveChangesAsync();

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates a user permission.
        /// </summary>
        /// <param name="permissionDTO">A DTO for handling user permissions</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPermission([FromBody] UserPermissionDTO permissionDTO)
        {
            try
            {
                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.UserPermissions)
                    .FirstOrDefaultAsync(p => p.ID == permissionDTO.ProjectID);

                //Returns error if project does not exist
                if (project == null) return NotFound("Project does not exist");

                //Checks if user owns the project
                var account = await _userManager.GetUserAsync(User);
                if (project.Owner != account) return Forbid();

                //Looks for permission
                var permission = await _databaseContext.UserPermissions
                    .Include(up => up.Account)
                    .FirstOrDefaultAsync(up => up.ID == permissionDTO.PermissionID);
                if (permission == null) return NotFound();

                //Applies changes
                permission.Permission = permissionDTO.Permission;
                await _databaseContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a user permission.
        /// </summary>
        /// <param name="permissionDTO">A DTO for handling user permissions</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteUserPermission([FromBody] string permissionId)
        {
            try
            {
                //Looks for permission
                var permission = await _databaseContext.UserPermissions
                    .Include (up => up.Project)
                    .FirstOrDefaultAsync(up => up.ID == permissionId);
                if (permission == null) return NotFound();

                //Checks if project owner is making the request
                var account = await _userManager.GetUserAsync(User);
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync (p => p.ID == permission.Project.ID);
                if (project.Owner != account) return Forbid();

                //Removes user
                _databaseContext.UserPermissions.Remove(permission);
                await _databaseContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Checks if the user can create a user permission for the project requested.
        /// </summary>
        /// <param name="username">The username to be checked.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("isnamevalid")]
        [Authorize]
        public async Task<IActionResult> IsUsernameValid([FromQuery]string username, [FromQuery] string projectId)
        {
            try
            {
                //Gets project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == projectId);
                if (project == null) return NotFound("Project does not exist");

                //Checks if user exists
                if (await _databaseContext.Accounts.FirstOrDefaultAsync(a => a.UserName == username) == null)
                    return NotFound("User does not exist.");

                //Checks if user supplied is project owner
                if (username.ToLower() == project.Owner.UserName.ToLower()) return BadRequest("You cannot assign yourself a permission because you are its owner.");

                //Checks if user has a permission in the project
                var userPermissions = await _databaseContext.UserPermissions
                    .Include(up => up.Account)
                    .ToListAsync();
                if (project.UserPermissions.Any(up => up.Account.UserName == username)) return BadRequest("User already has a permission for this project.");

                //User does not have a permission and can be added
                return Ok();
            }
            //Something went wrong, show an error
            catch (Exception ex)
            {
                return StatusCode(500, "Cannot validate username. Please try again later.");
            }
        }

        /// <summary>
        /// Checks if the user has permission to view a restricted project
        /// </summary>
        /// <param name="permissionDTO">A DTO for handling user permissions.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("validate")]
        [Authorize]
        public async Task<IActionResult> ValidateUserPermission([FromBody] UserPermissionDTO permissionDTO)
        {
            try
            {

                //Gets user account
                var account = await _userManager.GetUserAsync(User);

                //Looks for project
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == permissionDTO.ProjectID);
                if (project == null) return NotFound();

                //Gets account's user permission, if any
                var userPermission = await _databaseContext.UserPermissions
                    .Include(up => up.Account)
                    .Include(up => up.Project)
                    .FirstOrDefaultAsync(up => up.Account.Id == account.Id);

                //Returns forbid if user is not project owner, they have no project permission or their permission does not match
                if (account != project.Owner && (userPermission == null || userPermission.Permission != permissionDTO.Permission))
                    return Forbid();

                //Permission is valid
                return Ok();
            }
            //Something went wrong, show an error
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
