using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {
        public async Task SendMessageToUser(string senderId, string recipientId, string message, bool isRecipientConnected)
        {
            if (isRecipientConnected)
            {
                await Clients.User(recipientId).SendAsync("receiveMessage", senderId, message, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            else
                await Clients.User(senderId).SendAsync("messageNotRecieved", "the user is offline, message ain't received");
        }
    }
}
