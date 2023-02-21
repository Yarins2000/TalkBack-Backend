using Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class GameHub : Hub
    {
        /// <summary>
        /// key - groupName, value - List of users connectionId.
        /// </summary>
        private static readonly Dictionary<string, List<string>> _groups = new();
        /// <summary>
        /// key - gameId(would be the groupName), value - an instance of a <see cref="GameLogic"/>
        /// </summary>
        private static readonly Dictionary<string, GameLogic> _games = new();

        public async Task SendGameRequest(string recipientId)
        {
            await Clients.User(recipientId).SendAsync("gameRequestReceived");
        }

        public async Task JoinGroup(string groupName)
        {
            if (!_groups.ContainsKey(groupName))
            {
                _groups.Add(groupName, new List<string>());
            }
            if (_groups[groupName].Count >= 2)
            {
                // Group is full, return error message to client
                await Clients.Client(Context.ConnectionId).SendAsync("groupFull", groupName);
                return;
            }
            _groups[groupName].Add(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Notify the clients in the group about the new user
            await Clients.Group(groupName).SendAsync("userJoined", Context.ConnectionId);

        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _groups[groupName].Remove(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Get the group that the user was a part of
            var group = _groups.FirstOrDefault(g => g.Value.Contains(Context.ConnectionId)).Key;

            if (!string.IsNullOrEmpty(group))
            {
                // Remove the user from the group
                _groups[group].Remove(Context.ConnectionId);

                // If the group is now empty, remove it
                if (_groups[group].Count == 0)
                {
                    _groups.Remove(group);
                }

                // Notify the remaining users in the group that the user has disconnected
                await Clients.Group(group).SendAsync("userDisconnected", Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartGame(string groupName)
        {
            var game = new GameLogic();
            if (!_games.ContainsKey(groupName))
                _games.Add(groupName, game);
            else
                _games[groupName] = game;

            await Clients.Group(groupName).SendAsync("startGame");
        }

        public async Task MakeMove(string groupName, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            // Get the game associated with the group
            var game = _games[groupName];

            // Make the move
            var moveResult = game.MakeMove(fromRow, fromColumn, toRow, toColumn);

            // Notify the group about the move
            await Clients.Group(groupName).SendAsync("moveMade", moveResult);
        }
    }
}
