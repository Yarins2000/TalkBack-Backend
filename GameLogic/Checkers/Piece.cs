namespace TalkBack.Logic.Checkers;

/// <summary>
/// A class represents a single piece.
/// </summary>
public class Piece
{
    /// <summary>
    /// The color of the piece (black | white).
    /// </summary>
    public Color Color { get; set; }
    /// <summary>
    /// The position (row, column) of the piece.
    /// </summary>
    public (int, int) Position { get; set; }
    /// <summary>
    /// Is the player a king (has reached the edge of the board).
    /// </summary>
    public bool IsKing { get; set; }
    /// <summary>
    /// Is the piece alive (still playing) or got captured.
    /// </summary>
    public bool IsAlive { get; set; }
}
