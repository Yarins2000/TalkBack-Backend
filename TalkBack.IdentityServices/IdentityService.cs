using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalkBack.Models;

namespace TalkBack.IdentityServices
{
    /// <summary>
    /// The service class that implements <see cref="IIdentityService"/> interface. Responsible for the methods implementations.
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task ChangeConnectionStatus(User user)
        {
            if (IsAuthenticated())
                user.IsConnected = false;
            else
                user.IsConnected = true;
            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> ChangePassword(string username, string newPassword)
        {
            var user = await GetUserByUsername(username);
            if (user is null)
                return false;

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, user.PasswordHash, newPassword);
            return changePasswordResult.Succeeded;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public bool IsAuthenticated()
        {
            if (_signInManager.Context.User.Identity is null)
                return false;
            return _signInManager.Context.User.Identity.IsAuthenticated;
        }

        public async Task<bool> IsUsernameInUse(string username)
        {
            var existUsername = await GetUserByUsername(username);
            return existUsername != null;
        }

        public async Task<SignInResult> Login(LoginRequest loginRequest)
        {
            var result = await _signInManager.PasswordSignInAsync(loginRequest.Username, loginRequest.Password, loginRequest.RememberMe, false);
            return result;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> Register(RegisterRequest registerModel)
        {
            var newUser = new User { UserName = registerModel.Username };
            var result = await _userManager.CreateAsync(newUser, registerModel.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, false);
                return true;
            }
            return false;
        }
    }
}