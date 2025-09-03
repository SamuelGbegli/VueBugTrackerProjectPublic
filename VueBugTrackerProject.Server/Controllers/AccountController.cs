using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sodium;
using System.Diagnostics;
using VueBugTrackerProject.Classes;
using VueBugTrackerProject.Classes.DTOs;
using Microsoft.Extensions.Hosting.Internal;

namespace VueBugTrackerProject.Server.Controllers
{
    [ApiController]
    [Route("/accounts")]
    public class AccountController : ControllerBase
    {
        private DatabaseContext _context;
        private UserManager<Account> _userManager;
        private SignInManager<Account> _signInManager;

        public AccountController(DatabaseContext context,
            UserManager<Account> userManager,
            SignInManager<Account> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Function to check if a user in the app has a given username. 
        /// </summary>
        /// <param name="username">The username to be checked.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("verifyusername/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUsername(string username)
        {
            try
            {
                var result = await (_context.Accounts.AnyAsync(a => a.UserName.ToLower() == username.ToLower()));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of accounts in the application.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Authorize(Roles = "Administrator, Super user")]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber)
        {
            try
            {
                //Gers all accounts
                var accounts = await _context.Accounts
                    .OrderBy(a => a.UserName)
                    .ToListAsync();

                //Creates a container to store account information
                var accountContainer = new AccountContainer
                {
                    Pages = (int)Math.Ceiling(accounts.Count / (double)20),
                    CurrentPage = pageNumber,
                    TotalAccounts = accounts.Count
                };

                //Creates a view model for each account to be viewed
                foreach (var account in accounts.Skip((pageNumber - 1) * 20).Take(20))
                    accountContainer.Accounts.Add(new AccountViewModel(account));

                return Ok(accountContainer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Suspends or unsuspends a user's account.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("suspend")]
        [Authorize(Roles = "Administrator, Super user")]
        public async Task<IActionResult> SuspendAccount([FromBody] string accountId)
        {
            try
            {
                //Looks for the account by ID
                var account = await _context.Accounts
                    .FirstOrDefaultAsync( a => a.Id == accountId);
                if(account == null) return NotFound();

                //Toggles the user's suspend status (e.g., suspends the
                //account if it is not suspended)
                account.Suspended = !account.Suspended;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates a user's role.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="newRole"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("updaterole")]
        [Authorize(Roles = "Administrator, Super user")]
        public async Task<IActionResult> UpdateAccountRole([FromBody] RoleDTO roleDTO)
        {
            try
            {
                //Looks for the account by ID
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Id == roleDTO.AccountID);
                if (account == null) return NotFound();

                //Removes account's existing role
                var existingRoles = await _userManager.GetRolesAsync(account);
                await _userManager.RemoveFromRolesAsync(account, existingRoles);
                //Creates string based on role
                string roleString = "";
                switch (roleDTO.Role)
                {
                    case AccountRole.Normal:
                        roleString = "Normal";
                        break;
                    case AccountRole.Admin:
                        roleString = "Administrator";
                        break;
                    //Failsafe in case someone attempts to assign the super user role
                    default:
                        return BadRequest();
                }

                //Assigns new roles to account
                await _userManager.AddToRoleAsync(account, roleString);
                account.Role = roleDTO.Role;
                
                //Saves changes
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates an account's username.
        /// </summary>
        /// <param name="newUserName"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("updateusername")]
        [Authorize]
        public async Task<IActionResult> UpdateUsername([FromBody] string newUserName)
        {
            try
            {
                //Gets user account
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Id == _userManager.GetUserId(User));

                //Assigns new user naame and saves changes
                account.UserName = newUserName;
                await _context.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates an account's email.
        /// </summary>
        /// <param name="newUserEmail"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("updateemail")]
        [Authorize]
        public async Task<IActionResult> UpdateUserEmail([FromBody] string newUserEmail)
        {
            try
            {
                //Gets user account
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Id == _userManager.GetUserId(User));

                //Assigns new user email and saves changes
                account.Email = newUserEmail;
                await _context.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates a user's password.
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="oldPassword"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("updatepassword")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPassword([FromBody] PasswordDTO passwordDTO)
        {
            try
            {
                var account = await _userManager.GetUserAsync(User);
                var result =  await _userManager.ChangePasswordAsync(account, passwordDTO.OldPassword, passwordDTO.NewPassword);
                if (result.Succeeded)
                    return NoContent();
                //Section if user inputted the wrong password credentials
                return Unauthorized();
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Function to save and update a user's icon.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadicon")]
        [Authorize]
        public async Task<IActionResult> UploadIcon()
        {
            try
            {
                //Relative path of avatar folder
                var path = Path.GetRelativePath("C:\\Users\\samue\\source\\repos\\VueBugTrackerProject\\VueBugTrackerProject.Server\\", "C:\\Users\\samue\\source\\repos\\VueBugTrackerProject\\vuebugtrackerproject.client\\Avatars\\");

                //Ensures avatar folder exists
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //Gets user
                var account = await _userManager.GetUserAsync(User);

                //Gets uploaded icon
                var icon = Request.Form.Files.First();

                //Creates icon filename, {account id}.gif
                var fileName = Path.Combine(path, $"{account.Id}.gif");

                //Copies file to 
               using (Stream fileStream = new FileStream(fileName, FileMode.Create))
                {
                    await icon.CopyToAsync(fileStream);
                }

                //Sets account icon path
                account.Icon = fileName;

                //Saves changes
                await _context.SaveChangesAsync();

                //TODO: return icon link to client
                return Ok(fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Removes an account by an ID provided.
        /// </summary>
        /// <param name="accountId">The ID of the user to be deleted.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteaccount")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromBody] string accountId)
         {
            //TODO: possibly allow admins and super user to delete accounts
            //TODO: delete user avatar if not null
            try
            {
                //Cancels the request if the ID supplied does not match the logged in user
                var currentUser = _userManager.GetUserId(User);
                if (currentUser != accountId) return Unauthorized("Invalid delete request.");

                var accountToRemove = await _context.Accounts.FindAsync(accountId);

                if (accountToRemove != null)
                {                    
                    //Prevents super user account from being deleted
                    if(accountToRemove.Role == AccountRole.SuperUser) return Forbid();

                    var userRoles = await _userManager.GetRolesAsync(accountToRemove);

                    //Signs out user before deleting account
                    await _signInManager.SignOutAsync();

                    //Removes account's role
                    await _userManager.RemoveFromRolesAsync(accountToRemove, userRoles);

                    //Deletes the account
                    await _userManager.DeleteAsync(accountToRemove);
                    return NoContent();
                }
                return NotFound("No account exists with the ID provided.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
