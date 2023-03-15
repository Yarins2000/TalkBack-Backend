using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TalkBack.Models;

namespace TalkBack.Hubs
{
    /// <summary>
    /// A hub class which responsible for chat methods.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, List<Message>> _messages = new();

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

            if (!_messages.ContainsKey(groupName))
                _messages.Add(groupName, new());

            if (_messages[groupName].Count > 0)
                await Clients.Group(groupName).SendAsync("receiveAllMessages", _messages[groupName]);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(string senderId, string groupName, string message)
        {
            _messages[groupName].Add(new Message(senderId, message, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")));

            var ids = groupName.Split('_');
            var recipientId = ids.First(id => id != senderId);

            await Clients.Group(groupName).SendAsync("receiveMessage", senderId, message, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            await Clients.User(recipientId).SendAsync("notifyUser");
        }

        public void DeleteMessagesForGroup(string groupName)
        {
            _messages.Remove(groupName);
        }
    }
}
