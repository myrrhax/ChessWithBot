using ChessWithBot.Game;
using ChessWithBot.Game.Pieces;
using ChessWithBot.Utils;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChessWithBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game.Game Game { get; }
        private Border[,] Borders { get; }
        private Border? selectedCell = null;
        private HashSet<Move> cachedMoves = new HashSet<Move>();
        public void ReDrawBoard()
        {
            for (int i = 0; i < Board.BOARD_WIDTH; i++)
            {
                for (int j = 0; j < Board.BOARD_HEIGHT; j++)
                {
                    Square s = Game.Board.Squares[i,j];
                    Border cell = Borders[i,j];
                    if (s.Piece == null && cell.Child != null)
                    {
                        cell.Child = null;
                    }
                    else if (s.Piece != null && cell.Child == null)
                    {
                        PieceType pieceType = s.Piece.Type;
                        string imagePath = PieceImageNameHelper.GetPieceImagePath(pieceType, s.Piece.Color);
                        var bmp = new BitmapImage(new Uri(imagePath));
                        Image img = new Image()
                        {
                            Source = bmp,
                            Stretch = Stretch.Uniform,
                        };
                        cell.Child = img;
                    }
                }
            }
            foreach (var move in cachedMoves)
            {
                Borders[move.Coordinates.X, move.Coordinates.Y].Background = Brushes.LimeGreen;
            }
        }

        public void OnPieceTake((int X, int Y) pressedPosition)
        {
            Borders[pressedPosition.X, pressedPosition.Y].Child = null;
        }

        private (int X, int Y) GetBorderPosition(Border border)
        {
            for (int i = 0; i < Board.BOARD_WIDTH; i++)
            {
                for (int j = 0; j < Board.BOARD_HEIGHT; j++)
                {
                    Border cell = Borders[i,j];
                    if (cell == border) return (i, j);
                }
            }

            return (-1, -1);
        }

        private void MouseClick(object? sender, MouseEventArgs e)
        {
            if (!Game.IsPlayerMove) return;
            if (!Game.GetAllPossibleMoves(Brushes.White).Any())
            {
                Game.NoPossibleMoves(Brushes.White);
            }

            if (selectedCell == null)
            {
                selectedCell = (Border)sender;
                var position = GetBorderPosition(selectedCell);
                var piece = Game.Board.Squares[position.X, position.Y].Piece;
                if (piece == null || piece.Color != Brushes.White)
                {
                    selectedCell = null;
                    ReDrawBoard();
                    return;
                }

                cachedMoves = piece.PossibleMoves;
                ReDrawBoard();
                return;
            }
            var selectedCellPosition = GetBorderPosition(selectedCell);
            Piece p = Game.Board.Squares[selectedCellPosition.X, selectedCellPosition.Y].Piece!;
            var currentCell = (Border)sender!;
            var pressedPosition = GetBorderPosition(currentCell);
            if (Game.Board.Squares[pressedPosition.X, pressedPosition.Y].Piece?.Color == Brushes.White)
            {
                foreach (var m in cachedMoves)
                {
                    Brush c = m.Coordinates.X % 2 == m.Coordinates.Y % 2 ? Brushes.White : Brushes.Brown;
                    Borders[m.Coordinates.X, m.Coordinates.Y].Background = c;
                }
                selectedCell = (Border)sender;
                cachedMoves = Game.Board.Squares[pressedPosition.X, pressedPosition.Y].Piece!.PossibleMoves;
                ReDrawBoard();
                return;
            }

            if (Game.Board.Squares[pressedPosition.X, pressedPosition.Y].Piece?.Color == Brushes.Brown &&
                cachedMoves.Contains(new Move(p, pressedPosition)))
            {
                OnPieceTake(pressedPosition);
            }

            if (Game.TryMakeMove(new Move(p, pressedPosition)))
            {
                selectedCell.Child = null;
                foreach (var m in cachedMoves)
                {
                    Brush c = m.Coordinates.X % 2 == m.Coordinates.Y % 2 ? Brushes.White : Brushes.Brown;
                    Borders[m.Coordinates.X, m.Coordinates.Y].Background = c;
                }
                selectedCell = null;
                cachedMoves = new HashSet<Move>();
                Game.IsPlayerMove = false;
                ReDrawBoard();
                Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
                Game.MakeBotMove();
            }
            
        }

        private void OnGameEnd()
        {
            Game.IsPlayerMove = true;
            Game.StartGame();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int i = 0; i < Board.BOARD_WIDTH; i++)
            {
                ChessBoardGrid.RowDefinitions.Add(new RowDefinition());
                ChessBoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < Board.BOARD_WIDTH; i++)
            {
                for (int j = 0; j < Board.BOARD_HEIGHT; j++)
                {
                    var cell = new Border
                    {
                        Background = Game.Board.Squares[i, j].Color
                    };
                    cell.Width = Square.WIDTH;
                    cell.Height = Square.HEIGHT;

                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    ChessBoardGrid.Children.Add(cell);
                    Borders[i, j] = cell;
                    cell.MouseDown += MouseClick;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Width = Square.WIDTH * Board.BOARD_WIDTH;
            Height = Square.HEIGHT * Board.BOARD_HEIGHT;
            ResizeMode = ResizeMode.NoResize;
            Borders = new Border[Board.BOARD_WIDTH, Board.BOARD_HEIGHT];
            Game = new Game.Game(ReDrawBoard, OnGameEnd);
            Game.StartGame();
            InitializeGrid();
            Game.OnPieceTake += OnPieceTake;
            ReDrawBoard();
        }
    }
}