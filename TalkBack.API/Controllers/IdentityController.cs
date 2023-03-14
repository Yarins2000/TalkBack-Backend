using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO.Pipes;
using TalkBack.API.JwtServices;
using TalkBack.IdentityServices;
using TalkBack.Models;

namespace TalkBack.API.Controllers
{
    /// <summary>
    /// Represents a controller for identity usage.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _service;
        private readonly UserManager<User> _userManager;
        private readonly IJWTTokenGenerator _jwtToken;

        public IdentityController(IIdentityService service, UserManager<User> userManager, IJWTTokenGenerator jwtToken)
        {
            _service = service;
            _userManager = userManager;
            _jwtToken = jwtToken;
        }

        /// <summary>
        /// Handles HTTP POST requests to the "login" endpoint.
        /// </summary>
        /// <param name="loginModel">The "LoginRequest" object containing the username, password and remember me.</param>
        /// <returns>OK if the authentication was successful, with an object containing the authentication result, the user and the access token.
        /// BadRequest If the authentication failed, returns a message indicating that the username or password are incorrect.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginModel)
        {
            var result = await _service.Login(loginModel);
            if (result.Succeeded)
            {
                var user = await _service.GetUserByUsername(loginModel.Username!);
                user.IsConnected = true;
                //await _service.ChangeConnectionStatus(user);
                await _userManager.UpdateAsync(user);
                return Ok(new
                {
                    result,
                    user,
                    token = _jwtToken.GenerateToken(user)
                });
            }
            else
                return BadRequest("username or/and password are incorrect");
        }

        /// <summary>
        /// Handles HTTP POST requests to the "logout" endpoint.
        /// </summary>
        /// <param name="logoutModel">The "LogoutRequest" object containing the username.</param>
        /// <returns>Ok status.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequest logoutModel)
        {
            var user = await _service.GetUserByUsername(logoutModel.Username!);
            await _service.Logout();
            //await _service.ChangeConnectionStatus(user);
            user.IsConnected = false;
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        /// <summary>
        /// Handles HTTP POST requests to the "register" endpoint.
        /// </summary>
        /// <param name="registerModel">The "RegisterRequest" object containing the username, password and confirm password.</param>
        /// <returns>OK if the registration was successful, with a true value.
        /// BadRequest If the registration failed.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerModel)
        {
            var isUsernameExist = await _service.IsUsernameInUse(registerModel.Username!);
            if (isUsernameExist is true)
                return BadRequest("Username is already exist");

            var result = await _service.Register(registerModel);
            return result ? Ok(result) : BadRequest();
        }

        /// <summary>
        /// Handles HTTP PUT requests to the "changepassword" endpoint.
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="newPassword">The user's new password</param>
        /// <returns>Ok if the change was successful, otherwise BadRequest.</returns>
        [HttpPut("changepassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            var result = await _service.ChangePassword(changePasswordRequest);
            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Handles HTTP GET requests to the "users" endpoint.
        /// </summary>
        /// <returns>Ok with an IEnumerable of users.</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        /// Handles HTTP GET requests to the "isAuthenticated" endpoint.
        /// </summary>
        /// <returns>Ok with a boolean value indicating the authentication success.</returns>
        [HttpGet("isAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok(_service.IsAuthenticated());
        }
    }
}
