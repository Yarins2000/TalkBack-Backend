namespace TalkBack.CheckersGameLogic.CheckersModels
{
    /// <summary>
    /// The class represents a checkers board.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// The board's size (for length and height).
        /// </summary>
        public int BoardLength { get; set; }
        /// <summary>
        /// The board, a 2 dimensional array of <see cref="CheckerState"/>.
        /// </summary>
        public CheckerState[,] BoardGame { get; set; }
    }
}
