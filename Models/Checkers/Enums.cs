namespace Models.Checkers
{
    public enum CheckerState
    {
        Empty,
        White,
        Black
    };

    public enum Color
    {
        White = 1,
        Black
    }

    public enum GameStatus
    {
        InProgress,
        Finished,
        Draw
    }
}
