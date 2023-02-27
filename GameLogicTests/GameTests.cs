using TalkBack.Logic.Checkers;
namespace TalkBack.GameLogicTests
{
    public class GameTests
    {
        //private GameLogic _gameLogic = new(new Game(), new Board());
        private GameLogic _gameLogic;

        private void InitializeBoard(Board board, Game game) //maybe change to private
        {
            var boardGame = new CheckerState[board.BoardLength, board.BoardLength];
            var blackPieces = new List<Piece>();
            var whitePieces = new List<Piece>();

            for (int i = 0; i < board.BoardLength; i++)
            {
                for (int j = 0; j < board.BoardLength; j++)
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
            game.Players[0].Pieces = whitePieces.ToArray();
            game.Players[1].Pieces = blackPieces.ToArray();
            board.BoardGame = boardGame;
        }

        private Piece CreatePiece(Color color, int i, int j)
        {
            return new()
            {
                Color = color,
                Position = (i, j),
                IsAlive = true,
                IsKing = false
            };
        }

        private void StartGame(Game game)
        {
            game.Players = new Player[2]
            {
                new Player
                {
                    Id = "1",
                    Name = "first",
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
                    Id = "2",
                    Name = "second",
                    Color = Color.Black,
                    Pieces = new Piece[12],
                    TotalPieces = 12,
                    CapturedPieces = 0,
                    IsTurn = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };
            game.CurrentPlayer = game.Players[0];
            game.CreatedAt = DateTime.Now;
            game.UpdatedAt = DateTime.Now;
            game.Status = GameStatus.InProgress;
        }

        private void InitializeGame(out Game game, out Board board)
        {
            game = new Game();
            board = new Board() { BoardLength = 8 };
            StartGame(game);
            InitializeBoard(board, game);
        }

        private void SwitchTurn(Player p1, Player p2, Game game)
        {
            p1.IsTurn = !p1.IsTurn;
            p2.IsTurn = !p2.IsTurn;
            game.CurrentPlayer = p1.IsTurn ? p1 : p2;
        }
        //===============================================
        //InitializeGame(out Game game, out Board board, out GameLogic gameLogic);

        //[Fact]
        [Theory]
        [InlineData(5, 2, 4, 3)]
        [InlineData(5, 6, 4, 7)]
        //[InlineData(6,1,5,0)]
        //[InlineData(5,2,3,4)]
        public void IsValidMove_ReturnsTrueForValidMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            // Arrange

            InitializeGame(out Game game, out Board board);
            _gameLogic = new(game, board);

            //var method = new PrivateObject(game);
            //int fromRow = 5, fromColumn = 2, toRow = 4, toColumn = 3;

            // Act
            var result = _gameLogic.IsValidMove(fromRow, fromColumn, toRow, toColumn);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsBecomingKing_ReturnsTrueForKing()
        {
            InitializeGame(out Game game, out Board board);
            _gameLogic = new(game, board);
            board.BoardGame[1, 6] = CheckerState.White;
            board.BoardGame[0, 7] = CheckerState.Empty;
            var result = _gameLogic.IsBecomingKing(0, Color.White);
            Assert.True(result);
        }

        [Fact]
        public void IsCapturePossible_ValidMove_ReturnsTrue()
        {
            // Arrange
            InitializeGame(out Game game, out Board board);

            SwitchTurn(game.Players[0], game.Players[1], game); //now black is the current-player

            board.BoardGame[3, 2] = CheckerState.Black;
            board.BoardGame[4, 3] = CheckerState.White;
            board.BoardGame[5, 4] = CheckerState.Empty;

            _gameLogic = new(game, board);

            // Act
            var result = _gameLogic.IsCapturePossible(3, 2, 5, 4);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCapturePossible_InvalidMove_ReturnsFalse()
        {
            InitializeGame(out Game game, out Board board);

            board.BoardGame[3, 2] = CheckerState.White;
            board.BoardGame[4, 3] = CheckerState.Black;
            board.BoardGame[5, 4] = CheckerState.White;

            _gameLogic = new(game, board);

            // Act
            var result = _gameLogic.IsCapturePossible(5, 4, 3, 2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetCaptureMoves_ReturnsCorrectMoves()
        {
            // Arrange
            InitializeGame(out Game game, out Board board);

            board.BoardGame[3, 2] = CheckerState.Black;
            board.BoardGame[4, 3] = CheckerState.White;
            board.BoardGame[4, 1] = CheckerState.White;
            board.BoardGame[5, 4] = CheckerState.Empty;
            board.BoardGame[5, 0] = CheckerState.Empty;
            board.BoardGame[6, 5] = CheckerState.White;
            board.BoardGame[7, 6] = CheckerState.Empty;
            game.CurrentPlayer = game.Players[1];

            _gameLogic = new GameLogic(game, board);
            // Act
            var captureMoves = _gameLogic.GetCaptureMoves(3, 2);

            // Assert
            var expectedCaptureMoves = new List<(int, int, int, int)>
            {
                (3, 2, 5, 4), (3, 2, 5, 0)
            };
            Assert.Equal(expectedCaptureMoves, captureMoves);
        }


        [Fact]
        public void GetCaptureSequence_ReturnsCorrectSequence()
        {
            InitializeGame(out Game game, out Board board);
            board.BoardGame = new CheckerState[,]
            {
                { CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty },
                { CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty },
                { CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Black, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty },
                { CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.White, CheckerState.Black, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty },
                { CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty },
                { CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty },
                { CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty },
                { CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty, CheckerState.Empty },
            };

            board.BoardGame[3, 2] = CheckerState.Black;
            board.BoardGame[4, 3] = CheckerState.White;
            board.BoardGame[5, 4] = CheckerState.Empty;
            board.BoardGame[6, 5] = CheckerState.White;
            board.BoardGame[7, 6] = CheckerState.Empty;

            game.CurrentPlayer = game.Players[1];

            _gameLogic = new GameLogic(game, board);

            // Act
            var captureSequences = _gameLogic.GetCaptureSequence(3, 2);

            // Assert
            var expectedCaptureSequences = new List<(int, int, int, int)>
                {
                     (3, 2, 3, 2), (3, 2, 5, 4)
                };
            Assert.Equal(expectedCaptureSequences, captureSequences);
        }

        [Fact]
        public void IsValidCaptureMove_ValidMove_ReturnsTrue()
        {
            // Arrange
            InitializeGame(out Game game, out Board board);

            SwitchTurn(game.Players[0], game.Players[1], game);

            board.BoardGame[3, 2] = CheckerState.Black;
            board.BoardGame[4, 3] = CheckerState.White;
            board.BoardGame[5, 4] = CheckerState.Empty;

            game.Players[0].Pieces[0] = new Piece { Color = Color.White, IsAlive = true, IsKing = false, Position = (4, 3) };
            game.Players[1].Pieces[0] = new Piece { Color = Color.Black, IsAlive = true, IsKing = false, Position = (3, 2) };

            _gameLogic = new GameLogic(game, board);

            // Act
            var result = _gameLogic.IsValidCaptureMove(3, 2, 5, 4);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidCaptureMove_InvalidMove_ReturnsFalse()
        {
            // Arrange
            InitializeGame(out Game game, out Board board);

            SwitchTurn(game.Players[0], game.Players[1], game);

            board.BoardGame[3, 2] = CheckerState.Black;
            board.BoardGame[4, 3] = CheckerState.Empty;
            board.BoardGame[5, 4] = CheckerState.White;

            game.Players[0].Pieces[0] = new Piece { Color = Color.White, IsAlive = true, IsKing = false, Position = (5, 4) };
            game.Players[1].Pieces[0] = new Piece { Color = Color.Black, IsAlive = true, IsKing = false, Position = (3, 2) };

            _gameLogic = new GameLogic(game, board);

            // Act
            var result = _gameLogic.IsValidCaptureMove(3, 2, 5, 4);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void MakeCaptureMove_ValidCaptureMove_UpdatesBoardState()
        {
            // Arrange
            InitializeGame(out Game game, out Board board);

            board.BoardGame[3, 2] = CheckerState.White;
            board.BoardGame[4, 3] = CheckerState.Black;
            board.BoardGame[5, 4] = CheckerState.Empty;

            game.Players[0].Pieces[0] = new Piece { Color = Color.White, IsAlive = true, IsKing = true, Position = (3, 2) };
            game.Players[1].Pieces[0] = new Piece { Color = Color.Black, IsAlive = true, IsKing = false, Position = (4, 3) };

            _gameLogic = new(game, board);
            // Act
            var result = _gameLogic.MakeCaptureMove(3, 2, 5, 4);

            // Assert
            Assert.True(result);
            Assert.Equal(CheckerState.White, board.BoardGame[5, 4]);
            Assert.Equal(CheckerState.Empty, board.BoardGame[3, 2]);
            Assert.Equal(CheckerState.Empty, board.BoardGame[4, 3]);
            Assert.False(game.Players[1].Pieces[0].IsAlive);
            Assert.Equal((5, 4), game.Players[0].Pieces[0].Position);
            Assert.Equal(game.Players[1], game.CurrentPlayer);
        }

        [Fact]
        public void IsGameOver_GameNotOver_ReturnsFalse()
        {
            // Arrange
            InitializeGame(out Game game, out Board board);
            var whitePlayer = new Player { Color = Color.White };
            var blackPlayer = new Player { Color = Color.Black };
            whitePlayer.Pieces = new List<Piece>
                {
                    new Piece { IsAlive = true, Position = (0, 0) },
                    new Piece { IsAlive = true, Position = (0, 2) }
                }.ToArray();
            blackPlayer.Pieces = new List<Piece>
                {
                    new Piece { IsAlive = true, Position = (7, 1) },
                    new Piece { IsAlive = true, Position = (7, 3) }
                }.ToArray();
            game.Players = new List<Player> { whitePlayer, blackPlayer }.ToArray();
            game.CurrentPlayer = whitePlayer;

            _gameLogic = new GameLogic(game, board);

            // Act
            var result = _gameLogic.IsGameOver();

            // Assert
            Assert.False(result);
        }


    }
}