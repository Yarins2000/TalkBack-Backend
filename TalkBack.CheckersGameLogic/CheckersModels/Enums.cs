namespace TalkBack.CheckersGameLogic.CheckersModels
{
    /// <summary>
    /// Represents the possible states of a checker in a checkers game.
    /// </summary>
    public enum CheckerState
    {
        /// <summary>
        /// Represents an empty square on the board.
        /// </summary>
        Empty,

        /// <summary>
        /// Represents a white checker on the board.
        /// </summary>
        White,

        /// <summary>
        /// Represents a black checker on the board.
        /// </summary>
        Black
    };

    /// <summary>
    /// Represents the possible color of a checker.
    /// </summary>
    public enum Color
    {
        White = 1,
        Black
    }

    /// <summary>
    /// Represent the possible status of the game.
    /// </summary>
    public enum GameStatus
    {
        InProgress,
        Finished,
        Draw
    }
}
