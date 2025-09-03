using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VueBugTrackerProject.Classes;

using Microsoft.IdentityModel.JsonWebTokens;

using Sodium;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using VueBugTrackerProject.Server.Services;
using System.Security.Claims;

namespace VueBugTrackerProject.Server.Controllers
{
    //TODO: add list of function returns
    /// <summary>
    /// Controller for processing auth requests.
    /// </summary>
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// The app's database context.
        /// </summary>
        private readonly DatabaseContext _context;

        private readonly SignInManager<Account> _signInManager;

        private readonly UserManager<Account> _userManager;
        private readonly EmailService _emailService;

        private AuthService _authService;

        public AuthController(DatabaseContext context, AuthService authService, SignInManager<Account> signInManager, UserManager<Account> userManager, EmailService emailService)
        {
            _context = context;
            _authService = authService;
            _emailService = emailService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Adds a new account to the database.
        /// </summary>
        /// <param name="userDTO">The user detatils that will be added to the database.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createaccount")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] UserDTO userDTO)
        {
            //Bounces user if already logged in
            if (User.Identity.IsAuthenticated) return Unauthorized("User is already logged in.");
            try
            {
                var account = new Account
                {
                    Email = userDTO.EmailAddress,
                    UserName = userDTO.Username,
                    Role = AccountRole.Normal,
                    DateCreated = DateTime.Now
                };

                await _userManager.CreateAsync(account, userDTO.Password);
                await _userManager.AddToRoleAsync(account, "Normal");
                Trace.WriteLine($"Successfully created user {userDTO.Username}");

                return Created();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Function to log in the user to the application.
        /// </summary>
        /// <param name="userDTO">A DTO containing a username and password.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            //Bounces user if already logged in
            if (User.Identity.IsAuthenticated) return Unauthorized("User is already logged in.");

            try
            {
                //Searches for a user by the username input
                var account = await _userManager.FindByNameAsync(userDTO.Username);
                if (account == null)
                {
                    Trace.WriteLine("User does not exist.");
                    return Unauthorized("Incorrect username or password.");
                }

                //Checks if the user is suspended
                if (account.Suspended)
                {
                    Trace.WriteLine("Account has been suspended.");
                    return Unauthorized("Account has been suspended. Please contact an administrator if you need to regain access.");
                }

                //Checks if the inputted password matches the hash stored
                if (await _userManager.CheckPasswordAsync(account, userDTO.Password))
                {

                    await _signInManager.SignInAsync(account, false);

                    //TODO: possibly return more detailed user information
                    //Returns account ID if username and password match
                    return Ok(new { account.Id, username = account.UserName, role = account.Role, account.Icon });
                }

                Trace.WriteLine("Password does not match.");
                return Unauthorized("Incorrect username or password.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Logs the user out of the application.
        /// </summary>
        /// <param name="empty"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] object empty)
        {
            try
            {
                if (empty != null)
                {
                    await _signInManager.SignOutAsync();
                    return Ok();
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Checks if the user is logged into the application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("isloggedin")]
        [AllowAnonymous]
        public IActionResult IsLoggedIn()
        {
            try
            {
                var result = User.Identity.IsAuthenticated;
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Generates a token for the user to reset their account's password.
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("passwordresetrequest")]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePasswordResetToken([FromBody] UserDTO userDTO)
        {
            if (User.Identity.IsAuthenticated) return Forbid("User is already logged in.");
            try
            {
                var account = await _userManager.FindByNameAsync(userDTO.Username);
                if (account == null) return NotFound("User does not exist.");
                if (account.Email != userDTO.EmailAddress) return Unauthorized("Account's email does not match the one supplied.");

                var token = await _userManager.GeneratePasswordResetTokenAsync(account);
                var encodedToken = _authService.ConvertToBase64(token);

                Trace.WriteLine(encodedToken);

                var emailBody = _emailService.GetPasswordResetEmailText(account.Id, encodedToken);

                await _emailService.SendEmail(account, "Reset password", emailBody);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Checks if a password reset token is valid.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("validateresettoken")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidatePasswordResetToken([FromQuery] string id, [FromQuery] string token)
        {
            if (User.Identity.IsAuthenticated || token == null) return BadRequest();

            var account = await _userManager.FindByIdAsync(id);
            if (account == null) return NotFound("User does not exist.");

            var decodedToken = _authService.ConvertFromBase64(token);

            var result = await _userManager.VerifyUserTokenAsync(account, TokenOptions.DefaultProvider, UserManager<Account>.ResetPasswordTokenPurpose, decodedToken);

            return Ok(result);
        }

        /// <summary>
        /// Resets the password of a user.
        /// </summary>
        /// <param name="passwordResetDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDTO passwordResetDTO)
        {
            if (User.Identity.IsAuthenticated) return BadRequest();
            try
            {
                var account = await _userManager.FindByIdAsync(passwordResetDTO.AccountID);
                if (account == null) return BadRequest();
                await _userManager.ResetPasswordAsync(account, passwordResetDTO.PasswordResetToken, passwordResetDTO.Password);
                return Ok(account.UserName);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        /// <summary>
        /// Test function to validate the email service.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("testemail")]
        public async Task<IActionResult> TestEmailService([FromQuery] string username)
        {
            try
            {
                var account = await _userManager.FindByNameAsync(username);
                if (account == null) return NotFound("User does not exist");
                await _emailService.SendEmail(account, "Test email", "This is a email to test the email service of this application.");
                return Ok("Email sent successfully");
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets the logged in user's role.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getrole")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRole()
        {
            try
            {
                //Returns the normal role if not logged in
                if (!User.Identity.IsAuthenticated) return Ok(AccountRole.Normal);
                
                //Gets and returns role from logged in user
                var account = await _userManager.GetUserAsync(User);
                return Ok(account.Role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Function to validate a user's token.
        /// </summary>
        /// <param name="token">The token to be validated.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("validatetoken")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            try
            {
                if (await _authService.ValidateToken(token)) return Ok();
                else return Unauthorized("Token is invalid or expired.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
