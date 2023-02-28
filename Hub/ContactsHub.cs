using Microsoft.AspNetCore.SignalR;

namespace TalkBack.Hubs
{
    /// <summary>
    /// Hub class which responsible for contacts methods.
    /// </summary>
    public class ContactsHub : Hub
    {
        /// <summary>
        /// Invokes a method to all the users informs them that a new user has logged in.
        /// </summary>
        /// <param name="username">The logged in user's username</param>
        public async void UserLoggedIn(string username)
        {
            await Clients.All.SendAsync("newLogin", username);
        }
        /// <summary>
        /// Invokes a method to all the users informs them that a new user has logged out.
        /// </summary>
        /// <param name="username">The logged out user's username</param>
        public async void UserLoggedOut(string username)
        {
            await Clients.All.SendAsync("newLogout", username);
        }
    }
}