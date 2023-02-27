using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TalkBack.Logic.Checkers;

namespace TalkBack.Hubs
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

        public async Task<bool> JoinGroup(string groupName)
        {
            if (!_groups.ContainsKey(groupName))
            {
                _groups.Add(groupName, new List<string>());
            }
            if (_groups[groupName].Count >= 2)
            {
                // Group is full, return error message to client
                await Clients.Client(Context.ConnectionId).SendAsync("groupFull", groupName);
                return false;
            }
            _groups[groupName].Add(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Notify the clients in the group about the new user
            await Clients.Group(groupName).SendAsync("userJoined", Context.ConnectionId);
            return true;
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            RemoveUserAndGroup(groupName, Context.ConnectionId);
            RemoveGame(groupName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Get the group that the user was a part of
            var groupName = _groups.FirstOrDefault(g => g.Value.Contains(Context.ConnectionId)).Key;

            if (!string.IsNullOrEmpty(groupName))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                RemoveUserAndGroup(groupName, Context.ConnectionId);
                RemoveGame(groupName);

                // Notify the remaining users in the group that the user has disconnected
                await Clients.Group(groupName).SendAsync("userDisconnected", Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartGame(string groupName)
        {
            if (!_games.ContainsKey(groupName))
                _games.Add(groupName, new GameLogic());

            await Clients.Group(groupName).SendAsync("startGame");
        }

        public async Task MakeMove(string groupName, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            // Get the game associated with the group
            var game = _games[groupName];

            // Make the move
            var moveResult = game.MakeMove(fromRow, fromColumn, toRow, toColumn);

            // Notify the current player that his move is invalid
            if (!moveResult)
                await Clients.Caller.SendAsync("invalidMove");

            // Notify the group about the move
            await Clients.Group(groupName).SendAsync("moveMade", moveResult, fromRow, fromColumn, toRow, toColumn);

            // Notify the players the game is over
            if (game.Game.Status != GameStatus.InProgress)
                await Clients.Group(groupName).SendAsync("gameOver");
        }

        private void RemoveUserAndGroup(string groupName, string connectionId)
        {
            _groups[groupName].Remove(connectionId);
            if (_groups[groupName].Count == 0)
            {
                _groups.Remove(groupName);
            }
        }

        private void RemoveGame(string groupName)
        {
            if(_games.ContainsKey(groupName))
                _games.Remove(groupName);
        }
    }
}
