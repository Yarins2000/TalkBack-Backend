namespace TalkBack.CheckersGameLogic.CheckersModels
{
    /// <summary>
    /// The class represents a game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Game's Id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The game's players (an array of 2 players).
        /// The first element would be the white player, the second would be the black one.
        /// </summary>
        public Player[] Players { get; set; }

        //public CheckerState[,]? Board { get; set; }
        /// <summary>
        /// The current player who playing right now.
        /// </summary>
        public Player? CurrentPlayer { get; set; }
        /// <summary>
        /// The winner of the game.
        /// </summary>
        public Player? Winner { get; set; }
        /// <summary>
        /// The status of the game (in progress | finished | draw).
        /// </summary>
        public GameStatus Status { get; set; }
        /// <summary>
        /// The created time of the game.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// The updated time(the time the current player has ended his turn).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
