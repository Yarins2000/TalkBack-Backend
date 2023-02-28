using TalkBack.CheckersGameLogic.CheckersModels;

namespace TalkBack.CheckersGameLogic
{
    /// <summary>
    /// The class which responsible for the game logic methods.
    /// </summary>
    public class GameLogic
    {
        private readonly Game _game;
        private readonly Board _board;

        public Game Game { get; }

        public GameLogic()
        {
            _game = new Game();
            _board = new Board() { BoardLength = 8 };
            InitializeGame();
            InitializeBoard();
            Game = _game;
        }

        public GameLogic(Game game, Board board)
        {
            _game = game;
            _board = board;
        }

        /// <summary>
        /// Initializes the board with white/black/empty squares and the pieces of each player.
        /// </summary>
        public void InitializeBoard()
        {
            var boardGame = new CheckerState[_board.BoardLength, _board.BoardLength];
            var blackPieces = new List<Piece>();
            var whitePieces = new List<Piece>();

            for (int i = 0; i < _board.BoardLength; i++)
            {
                for (int j = 0; j < _board.BoardLength; j++)
                {
                    if (i < 3)
                    {
                        if (i % 2 == 0)
                        {
                            if (j % 2 != 0)
                            {
                                boardGame[i, j] = CheckerState.Black;
                                blackPieces.Add(CreatePiece(Color.Black, i, j));
                            }
                        }
                        else
                        {
                            if (j % 2 == 0)
                            {
                                boardGame[i, j] = CheckerState.Black;
                                blackPieces.Add(CreatePiece(Color.Black, i, j));
                            }
                        }
                    }
                    else if (i > 4)
                    {
                        if (i % 2 == 0)
                        {
                            if (j % 2 != 0)
                            {
                                boardGame[i, j] = CheckerState.White;
                                whitePieces.Add(CreatePiece(Color.White, i, j));
                            }
                        }
                        else
                        {
                            if (j % 2 == 0)
                            {
                                boardGame[i, j] = CheckerState.White;
                                whitePieces.Add(CreatePiece(Color.White, i, j));
                            }
                        }
                    }
                }
            }
            _game.Players[0].Pieces = whitePieces.ToArray();
            _game.Players[1].Pieces = blackPieces.ToArray();
            _board.BoardGame = boardGame;
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        public void InitializeGame(/*string firstUserConnectionId, string secondUserConnectionId*/)
        {
            _game.Players = new Player[2]
            {
                new Player
                {
                    //Id = firstUserConnectionId,
                    //Name = u1.UserName,
                    Color = Color.White,
                    Pieces = new Piece[12],
                    TotalPieces = 12,
                    CapturedPieces = 0,
                    IsTurn = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Player
                {
                    //Id = secondUserConnectionId,
                    //Name = u2.UserName,
                    Color = Color.Black,
                    Pieces = new Piece[12],
                    TotalPieces = 12,
                    CapturedPieces = 0,
                    IsTurn = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };
            _game.CurrentPlayer = _game.Players[0];
            _game.CreatedAt = DateTime.Now;
            _game.UpdatedAt = DateTime.Now;
            _game.Status = GameStatus.InProgress;
        }

        /// <summary>
        /// Creates a single piece.
        /// </summary>
        /// <param name="color">the piece's color</param>
        /// <param name="row">the piece's row position</param>
        /// <param name="column">the piece's column position</param>
        /// <returns>The new created piece.</returns>
        public Piece CreatePiece(Color color, int row, int column)
        {
            return new()
            {
                Color = color,
                Position = (row, column),
                IsAlive = true,
                IsKing = false
            };
        }

        /// <summary>
        /// Makes a move made by the user.
        /// </summary>
        /// <param name="fromRow">The current playing piece starting row</param>
        /// <param name="fromColumn">The current playing piece starting column</param>
        /// <param name="toRow">The row which the current playing piece wants to move to</param>
        /// <param name="toColumn">The column which the current playing piece wants to move to</param>
        /// <returns>true if the move was invalid, otherwise false.</returns>
        public bool MakeMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (Math.Abs(toRow - fromRow) == 1 && Math.Abs(toColumn - fromColumn) == 1)
                return MakeRegularMove(fromRow, fromColumn, toRow, toColumn);
            else if (Math.Abs(toRow - fromRow) == 2 && Math.Abs(toColumn - fromColumn) == 2)
                return MakeCaptureMove(fromRow, fromColumn, toRow, toColumn);
            IsGameOver();
            return false;
        }

        /// <summary>
        /// Makes a single step move.
        /// </summary>
        /// <param name="fromRow">The current playing piece starting row</param>
        /// <param name="fromColumn">The current playing piece starting column</param>
        /// <param name="toRow">The row which the current playing piece wants to move to</param>
        /// <param name="toColumn">The column which the current playing piece wants to move to</param>
        /// <returns>true if the move is valid, otherwise false.</returns>
        public bool MakeRegularMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (!IsValidMove(fromRow, fromColumn, toRow, toColumn))
                return false;

            var currentPiece = _game.CurrentPlayer!.Pieces.First(p => p.Position == (fromRow, fromColumn));

            if (IsBecomingKing(toRow, _game.CurrentPlayer!.Color))
                currentPiece.IsKing = true;

            // Update the board state
            _board.BoardGame[toRow, toColumn] = _board.BoardGame[fromRow, fromColumn];
            currentPiece.Position = (toRow, toColumn);
            _board.BoardGame[fromRow, fromColumn] = CheckerState.Empty;

            // Switch the active player
            SwitchTurn(_game.Players[0], _game.Players[1]);

            _game.UpdatedAt = DateTime.Now;

            return true;
        }

        /// <summary>
        /// Checks for the validity of the move.
        /// </summary>
        /// <param name="fromRow">The current playing piece starting row</param>
        /// <param name="fromColumn">The current playing piece starting column</param>
        /// <param name="toRow">The row which the current playing piece wants to move to</param>
        /// <param name="toColumn">The column which the current playing piece wants to move to</param>
        /// <returns>true if the move is valid, otherwise false.</returns>
        public bool IsValidMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            // Check if the move is within the board bounds
            if (fromRow < 0 || fromRow > 7 || fromColumn < 0 || fromColumn > 7 ||
                toRow < 0 || toRow > 7 || toColumn < 0 || toColumn > 7)
                return false;

            // Check if the user made a single step
            if (Math.Abs(toRow - fromRow) != 1 || Math.Abs(toColumn - fromColumn) != 1)
                return false;

            // Check if the piece belongs to the active player
            var activePlayer = _game.CurrentPlayer!;
            var pieceState = _board.BoardGame[fromRow, fromColumn];
            if (activePlayer.Color == Color.White && pieceState != CheckerState.White ||
                activePlayer.Color == Color.Black && pieceState != CheckerState.Black)
                return false;

            var currentPiece = activePlayer.Pieces.First(p => p.Position == (fromRow, fromColumn));
            //  Check if the direction is correct
            if (!currentPiece.IsKing && !IsValidDirection(fromRow, toRow, activePlayer.Color))
                return false;

            // Check if the destination is not occupied
            if (_board.BoardGame[toRow, toColumn] != CheckerState.Empty)
                return false;

            return true;
        }

        /// <summary>
        /// Checks for the validity of the movement direction.
        /// </summary>
        /// <param name="fromRow">The current playing piece starting row</param>
        /// <param name="toRow">The row which the current playing piece wants to move to</param>am>
        /// <param name="playerColor">The color of the current piece.</param>
        /// <returns>true if the direction is valid, otherwise false.</returns>
        public bool IsValidDirection(int fromRow, int toRow, Color playerColor)
        {
            if (playerColor == Color.Black)
                return toRow - fromRow > 0;
            else
                return toRow - fromRow < 0;
        }

        /// <summary>
        /// Checks if the piece has become a king.
        /// </summary>
        /// <param name="toRow">The row which the current playing piece wants to move to</param>
        /// <param name="playerColor"></param>
        /// <returns>true if the piece has become a king, otherwise false.</returns>
        public bool IsBecomingKing(int toRow, Color playerColor)
        {
            if (playerColor == Color.Black)
                return toRow == 7;
            else
                return toRow == 0;
        }

        /// <summary>
        /// Switch the turns and save the current player.
        /// </summary>
        /// <param name="p1">player 1</param>
        /// <param name="p2">player 2</param>
        public void SwitchTurn(Player p1, Player p2)
        {
            p1.IsTurn = !p1.IsTurn;
            p2.IsTurn = !p2.IsTurn;
            _game.CurrentPlayer = p1.IsTurn ? p1 : p2;
        }

        // ===== Capturing ========
        /// <summary>
        /// Checks if the capture is valid.
        /// </summary>
        /// <param name="fromRow">The current playing piece starting row</param>
        /// <param name="fromColumn">The current playing piece starting column</param>
        /// <param name="toRow">The row which the current playing piece wants to move to</param>
        /// <param name="toColumn">The column which the current playing piece wants to move to</param>
        /// <returns>true if the capture move was valid, otherwise false.</returns>
        public bool IsCapturePossible(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            // Check if the move is within the board bounds
            if (fromRow < 0 || fromRow > 7 || fromColumn < 0 || fromColumn > 7 ||
                toRow < 0 || toRow > 7 || toColumn < 0 || toColumn > 7)
                return false;

            // Check if the move is two steps diagonally
            if (Math.Abs(toRow - fromRow) != 2 || Math.Abs(toColumn - fromColumn) != 2)
                return false;

            // Check if the destination square is empty
            var destinationState = _board.BoardGame[toRow, toColumn];
            if (destinationState != CheckerState.Empty)
                return false;

            // Check if the piece belongs to the active player
            var activePlayer = _game.CurrentPlayer!;
            var pieceState = _board.BoardGame[fromRow, fromColumn];
            if (activePlayer.Color == Color.White && pieceState != CheckerState.White ||
                activePlayer.Color == Color.Black && pieceState != CheckerState.Black)
                return false;

            // Check if the middle square is occupied by an opponent's piece
            var middleRow = (fromRow + toRow) / 2;
            var middleColumn = (fromColumn + toColumn) / 2;
            var middleState = _board.BoardGame[middleRow, middleColumn];
            if (activePlayer.Color == Color.White && middleState != CheckerState.Black ||
                activePlayer.Color == Color.Black && middleState != CheckerState.White)
                return false;

            return true;
        }

        /// <summary>
        /// Checks for the possible capture move depending on the starting position.
        /// </summary>
        /// <param name="fromRow">The current playing piece starting row</param>
        /// <param name="fromColumn">The current playing piece starting column</param>
        /// <returns>A list of tuples that represent the starting position (item1, item2) and the possible position after a capture(item3, item4).</returns>
        public List<(int, int, int, int)> GetCaptureMoves(int fromRow, int fromColumn)
        {
            var captureMoves = new List<(int, int, int, int)>();
            var currentPiece = _game.CurrentPlayer!.Pieces.First(p => p.Position == (fromRow, fromColumn));
            var directions = currentPiece.IsKing ? new[] { (1, 1), (1, -1), (-1, 1), (-1, -1) } :
                                     (currentPiece.Color == Color.Black ? new[] { (1, 1), (1, -1) } : new[] { (-1, 1), (-1, -1) });
            foreach (var (dRow, dCol) in directions)
            {
                var toRow = fromRow + 2 * dRow;
                var toCol = fromColumn + 2 * dCol;
                if (IsCapturePossible(fromRow, fromColumn, toRow, toCol))
                    captureMoves.Add((fromRow, fromColumn, toRow, toCol));
            }
            return captureMoves;
        }

        public List<(int, int, int, int)> GetCaptureSequence(int fromRow, int fromColumn)//maybe delete
        {
            var sequence = new List<(int, int, int, int)> { (fromRow, fromColumn, fromRow, fromColumn) };
            var current = (fromRow, fromColumn, fromRow, fromColumn);
            while (true)
            {
                var nextMoves = GetCaptureMoves(current.Item1, current.Item2);
                if (nextMoves.Count == 0)
                    break;
                current = nextMoves[0];
                sequence.Add(current);

                // Check if the current position has already been added to the sequence
                if (sequence.Count(x => x.Item1 == current.Item1 && x.Item2 == current.Item2) > 1)
                    break;
            }
            return sequence;
        }

        /// <summary>
        /// Determines whether a capture move from the starting position (fromRow, fromColumn) to the end position (toRow, toColumn)
        /// is valid for the current player. A capture move is valid if it is possible and results in capturing at least one opponent's piece.
        /// </summary>
        /// <param name="fromRow">The row of the starting position.</param>
        /// <param name="fromColumn">The column of the starting position.</param>
        /// <param name="toRow">The row of the end position.</param>
        /// <param name="toColumn">The column of the end position.</param>
        /// <returns>True if the capture move is valid, otherwise false.</returns>
        public bool IsValidCaptureMove(int fromRow, int fromColumn, int toRow, int toColumn)//maybe change to private
        {
            if (!IsCapturePossible(fromRow, fromColumn, toRow, toColumn))
                return false;

            var currentPlayer = _game.CurrentPlayer!;
            var currentPiece = currentPlayer.Pieces.First(p => p.Position == (fromRow, fromColumn));
            var captureMoves = GetCaptureMoves(fromRow, fromColumn);

            return captureMoves.Any(c => c.Item3 == toRow && c.Item4 == toColumn);
        }

        /// <summary>
        /// Makes a capture move - move the piece from the starting position to the end one and exclude the captured piece from the game.
        /// </summary>
        /// <param name="fromRow">The row of the starting position.</param>
        /// <param name="fromColumn">The column of the starting position.</param>
        /// <param name="toRow">The row of the end position.</param>
        /// <param name="toColumn">The column of the end position.</param>
        /// <returns>True if the capture was successful, otherwise false.</returns>
        public bool MakeCaptureMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (!IsValidCaptureMove(fromRow, fromColumn, toRow, toColumn))
                return false;

            var activePlayer = _game.CurrentPlayer!;
            var currentPiece = activePlayer.Pieces.First(p => p.Position == (fromRow, fromColumn));

            if (!currentPiece.IsKing)
            {
                var direction = activePlayer.Color == Color.White ? -1 : 1;
                if (toRow - fromRow != 2 * direction)
                    return false;
            }

            var middleRow = (fromRow + toRow) / 2;
            var middleColumn = (fromColumn + toColumn) / 2;
            var capturedPiece = _game.Players.First(p => p.Color != activePlayer.Color).Pieces.First(p => p.Position == (middleRow, middleColumn));
            var opponentPlayer = _game.Players.FirstOrDefault(p => p != activePlayer);
            capturedPiece.IsAlive = false;
            capturedPiece.Position = (-1, -1);
            opponentPlayer!.CapturedPieces++;

            if (IsBecomingKing(toRow, activePlayer.Color))
                currentPiece.IsKing = true;

            // Update the board state
            _board.BoardGame[toRow, toColumn] = _board.BoardGame[fromRow, fromColumn];
            currentPiece.Position = (toRow, toColumn);
            _board.BoardGame[fromRow, fromColumn] = CheckerState.Empty;
            _board.BoardGame[middleRow, middleColumn] = CheckerState.Empty;

            var moreCaptures = GetCaptureMoves(toRow, toColumn);
            if (moreCaptures.Count == 0)
            {
                // Switch the active player
                SwitchTurn(_game.Players[0], _game.Players[1]);
            }

            _game.UpdatedAt = DateTime.Now;

            return true;
        }

        /// <summary>
        /// Checks wether the game is over or not.
        /// </summary>
        /// <returns>True if the game is over, otherwise false.</returns>
        public bool IsGameOver()
        {
            // Check if one player has won
            var whitePlayer = _game.Players.First(p => p.Color == Color.White);
            var blackPlayer = _game.Players.First(p => p.Color == Color.Black);
            if (!whitePlayer.Pieces.Any(p => p.IsAlive) || !blackPlayer.Pieces.Any(p => p.IsAlive))
            {
                _game.Status = GameStatus.Finished;
                return true;
            }

            // Check if there is a draw
            if (!whitePlayer.Pieces.Any(p => p.IsAlive || GetCaptureMoves(p.Position.Item1, p.Position.Item2).Count > 0) &&
                !blackPlayer.Pieces.Any(p => p.IsAlive || GetCaptureMoves(p.Position.Item1, p.Position.Item2).Count > 0))
            {
                _game.Status = GameStatus.Draw;
                return true;
            }

            return false;
        }
    }
}