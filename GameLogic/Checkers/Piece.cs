namespace TalkBack.Logic.Checkers;

public class Piece
{
    /// <summary>
    /// The color of the piece (black | white).
    /// </summary>
    public Color Color { get; set; }
    /// <summary>
    /// The position (x, y) of the piece.
    /// </summary>
    public (int, int) Position { get; set; }
    /// <summary>
    /// Is the player a king (reached the edge of the board).
    /// </summary>
    public bool IsKing { get; set; }
    /// <summary>
    /// Is the piece alive(still playing) or got captured.
    /// </summary>
    public bool IsAlive { get; set; }
}
