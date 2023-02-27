using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TalkBack.DataService;
using TalkBack.Server.Services.Token;
using TalkBack.Models;

namespace TalkBack.Server.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerModel)
        {
            var isUsernameExist = await _service.IsUsernameInUse(registerModel.Username!);
            if (isUsernameExist is true)
                return BadRequest("Username is already exist");

            var result = await _service.Register(registerModel);
            return result ? Ok(result) : BadRequest();
        }

        [HttpPut("changepassword")]
        public async Task<ActionResult> ChangePassword(string username, string newPassword)
        {
            var result = await _service.ChangePassword(username, newPassword);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("isAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok(_service.IsAuthenticated());
        }
    }
}
