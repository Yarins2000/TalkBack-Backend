using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TalkBack.Hubs
{
    /// <summary>
    /// A hub class which responsible for chat methods.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// Invokes a method to the message's sender/recipient.
        /// </summary>
        /// <param name="senderId">The message's sender id(his user id)</param>
        /// <param name="recipientId">The message's recipient id(his user id)</param>
        /// <param name="message">The message that sent from sender to recippient</param>
        /// <param name="isRecipientConnected">Recipient connection status</param>
        public async Task SendMessageToUser(string senderId, string recipientId, string message, bool isRecipientConnected)
        {
            if (isRecipientConnected)
                await Clients.User(recipientId).SendAsync("receiveMessage", senderId, message, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            else
                await Clients.User(senderId).SendAsync("messageNotRecieved", "the user is offline, message hasn't received");
        }
    }
}
