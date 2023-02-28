namespace TalkBack.Logic.Checkers;

/// <summary>
/// A class represents a single player.
/// </summary>
public class Player
{
    /// <summary>
    /// The UserId.
    /// </summary>
    public string? Id { get; set; }
    /// <summary>
    /// The username.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// The color of the player (black | white).
    /// </summary>
    public Color Color { get; set; }
    /// <summary>
    /// The player pieces.
    /// </summary>
    public Piece[] Pieces { get; set; }
    /// <summary>
    /// The amount of player's pieces.
    /// </summary>
    public int TotalPieces { get; set; }
    /// <summary>
    /// The amount of captured pieces of the player.
    /// </summary>
    public int CapturedPieces { get; set; }
    /// <summary>
    /// Is the player's turn.
    /// </summary>
    public bool IsTurn { get; set; }
    /// <summary>
    /// The time the player was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// The time the player last moved.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
