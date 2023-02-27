using Microsoft.AspNetCore.Identity;
using TalkBack.Models;

namespace TalkBack.DataService
{
    public interface IIdentityService
    {
        public Task<SignInResult> Login(LoginRequest loginModel);
        public Task Logout();
        public Task<bool> Register(RegisterRequest registerModel);
        public Task<bool> IsUsernameInUse(string username);
        public Task<bool> ChangePassword(string username, string newPassword);
        public Task<User> GetUserByUsername(string username);
        public Task<IEnumerable<User>> GetAllUsers();
        public Task ChangeConnectionStatus(User user);
        public bool IsAuthenticated();
    }
}
