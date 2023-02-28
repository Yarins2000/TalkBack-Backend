using Microsoft.AspNetCore.Identity;
using TalkBack.Models;

namespace TalkBack.IdentityServices
{
    public interface IIdentityService
    {
        /// <summary>
        /// Gets a <see cref="LoginRequest"/> parameter and trying to login the user.
        /// </summary>
        /// <param name="loginRequest">The login request that contains the username and password of the user.</param>
        /// <returns>True if the login was successful, otherwise false.</returns>
        public Task<SignInResult> Login(LoginRequest loginRequest);
        /// <summary>
        /// Logs out the authenticated user (based on the current session).
        /// </summary>
        public Task Logout();
        /// <summary>
        /// Gets a <see cref="RegisterRequest"/> parameter and trying to register the user.
        /// </summary>
        /// <param name="registerModel">The register request which contains the user's information for the registration.</param>
        /// <returns>True if the user was registered successfully, otherwise false.</returns>
        public Task<bool> Register(RegisterRequest registerModel);
        /// <summary>
        /// Checks wether the username is already in use.
        /// </summary>
        /// <param name="username">the received username</param>
        /// <returns>true if the username is already in use, otherwise false.</returns>
        public Task<bool> IsUsernameInUse(string username);
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="username">the user's username</param>
        /// <param name="newPassword">the user's new password</param>
        /// <returns>true if the change was seccessful, otherwise false.</returns>
        public Task<bool> ChangePassword(string username, string newPassword);
        /// <summary>
        /// Gets a user by its username from the user table in the db.
        /// </summary>
        /// <param name="username">the user's username</param>
        /// <returns>The user from the db.</returns>
        public Task<User> GetUserByUsername(string username);
        /// <summary>
        /// Gets all the users from the user table in the db.
        /// </summary>
        /// <returns>An IEnumerable of the users.</returns>
        public Task<IEnumerable<User>> GetAllUsers();
        /// <summary>
        /// Changes the user connection status.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ChangeConnectionStatus(User user);
        /// <summary>
        /// Checks wether the current user (from current session) is authenticated.
        /// </summary>
        /// <returns>true if the user is authenticated, otherwise false.</returns>
        public bool IsAuthenticated();
    }
}
