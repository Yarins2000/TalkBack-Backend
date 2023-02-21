namespace Models.Checkers
{
    public class Board
    {
        /// <summary>
        /// The board's size (size, size).
        /// </summary>
        public int BoardLength { get; set; }
        /// <summary>
        /// The board, a 2 dimensional array of checker state.
        /// </summary>
        public CheckerState[,] BoardGame { get; set; }
    }
}
