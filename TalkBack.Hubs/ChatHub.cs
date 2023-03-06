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
        private string GenerateGroupName(string user1Id, string user2Id)
        {
            var ids = new string[] { user1Id, user2Id };
            Array.Sort(ids);
            return string.Join("_", ids);
        }

        public async Task JoinGroup(string senderId, string recipientId)
        {
            string groupName = GenerateGroupName(senderId, recipientId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("groupNameReceived", groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(string senderId, string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("receiveMessage", senderId, message, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }
}
