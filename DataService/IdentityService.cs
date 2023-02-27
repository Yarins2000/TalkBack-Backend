using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalkBack.Models;

namespace TalkBack.DataService
{
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
            if(IsAuthenticated())
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

        /// <summary>
        /// Checks wether the username is already in use.
        /// </summary>
        /// <param name="username">the received username</param>
        /// <returns>true if the username is already in use, otherwise false.</returns>
        public async Task<bool> IsUsernameInUse(string username)
        {
            var existUsername = await GetUserByUsername(username);
            return existUsername != null;
        }

        public async Task<SignInResult> Login(LoginRequest loginModel)
        {
            var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, loginModel.RememberMe, false);
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