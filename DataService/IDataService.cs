using Microsoft.AspNetCore.Identity;
using Models;

namespace DataService
{
    public interface IDataService
    {
        public Task<SignInResult> Login(Login loginModel);
        public Task Logout();
        public Task<bool> Register(Register registerModel);
        public Task<bool> IsUsernameInUse(string username);
        public Task<bool> ChangePassword(string username, string newPassword);
        public Task<User> GetUserByUsername(string username);
        public Task<IEnumerable<User>> GetAllUsers();
        public Task ChangeConnectionStatus(User user);
        public bool IsAuthenticated();
    }
}
