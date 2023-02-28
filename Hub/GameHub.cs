using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TalkBack.Logic.Checkers;

namespace TalkBack.Hubs
{
    /// <summary>
    /// Hub class which responsible for game methods.
    /// </summary>
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

        /// <summary>
        /// Invokes a method for the recipient when the sender wants to play with him.
        /// </summary>
        /// <param name="recipientId">The recipient's id</param>
        public async Task SendGameRequest(string recipientId)
        {
            await Clients.User(recipientId).SendAsync("gameRequestReceived");
        }

        /// <summary>
        /// Gets the group's name and join the user to this group.
        /// </summary>
        /// <param name="groupName">The group's name</param>
        /// <returns>True if the joining was successful, otherwise false.</returns>
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

        /// <summary>
        /// Remove the user from the group and game.
        /// </summary>
        /// <param name="groupName">The group's name</param>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            RemoveUserAndGroup(groupName, Context.ConnectionId);
            RemoveGame(groupName);
        }

        /// <summary>
        /// Occurs when a user has disconnected. Removes him from the group and game.
        /// </summary>
        /// <param name="exception">The exception occurs while disconnecting</param>
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

        /// <summary>
        /// Starting the game by creating a new <see cref="GameLogic"/> instance, and informs the  group's users.
        /// </summary>
        /// <param name="groupName">The group name</param>
        public async Task StartGame(string groupName)
        {
            if (!_games.ContainsKey(groupName))
                _games.Add(groupName, new GameLogic());

            await Clients.Group(groupName).SendAsync("startGame");
        }

        /// <summary>
        /// Inokes the MakeMove function (from <see cref="GameLogic"/>).
        /// </summary>
        /// <param name="groupName">The group's name</param>
        /// <param name="fromRow">The current playing piece starting row</param>
        /// <param name="fromColumn">The current playing piece starting column</param>
        /// <param name="toRow">The row which the current playing piece wants to move to</param>
        /// <param name="toColumn">The column which the current playing piece wants to move to</param>
        /// <returns>Invokes a method for the caller if its an invalid move or for the group if its a valid one. also might inform the group that the game is over.</returns>
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

        /// <summary>
        /// Remove a user from a group. If the group is empty, removes it also.
        /// </summary>
        /// <param name="groupName">the group's name</param>
        /// <param name="connectionId">the user's connection id</param>
        private void RemoveUserAndGroup(string groupName, string connectionId)
        {
            _groups[groupName].Remove(connectionId);
            if (_groups[groupName].Count == 0)
            {
                _groups.Remove(groupName);
            }
        }

        /// <summary>
        /// Removes the game from its dictionary.
        /// </summary>
        /// <param name="groupName">the group's name</param>
        private void RemoveGame(string groupName)
        {
            if(_games.ContainsKey(groupName))
                _games.Remove(groupName);
        }
    }
}
