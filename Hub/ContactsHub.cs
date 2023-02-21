using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models;

namespace Hubs
{
    public class ContactsHub : Hub
    {
        public async void UserLoggedIn(string username)
        {
            await Clients.All.SendAsync("newLogin", username);
        }
        public async void UserLoggedOut(string username)
        {
            await Clients.All.SendAsync("newLogout", username);
        }
    }
}