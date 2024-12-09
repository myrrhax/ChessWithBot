using ChessWithBot.Game.Pieces;
using System.Windows;
using System.Windows.Media;

namespace ChessWithBot.Game;

public class Game
{
    public Board Board { get; set; }
    public bool IsPlayerMove { get; set; } = true;
    public event Action NotifyUpdate;
    public event Action<(int X, int Y)> OnPieceTake;
    private ChessBot bot;
    public object Locker { get; set; } = new object();
    public event Action OnGameEnd;
    public Game(Action action, Action endGame)
    {
        NotifyUpdate += action;
        OnGameEnd += endGame;
    }

    public void StartGame()
    {
        Board = new Board();
        bot = new ChessBot(this);
    }

    public bool TryMakeMove(Move move)
    {
        Piece p = move.Piece;
        if (p.PossibleMoves.Contains(move))
        {
            p.Move(move.Coordinates, p.PossibleMoves);
            Board.UpdatePiecesPossibleMoves();
            return true;
        }
        return false;
    }

    public void UndoMove((int, int) oldPosition, Move move)
    {
        var piece = move.Piece;
        piece.UndoMove(oldPosition, move.Coordinates, move.TakedPiece);
        Board.UpdatePiecesPossibleMoves();
    }

    public void MakeBotMove()
    {
        bot.MakeMove();
    }

    public void UpdateBoard()
    {
        NotifyUpdate?.Invoke();
    }

    public void TakePieceByBot((int, int) position)
    {
        OnPieceTake?.Invoke(position);
    }

    internal void NoPossibleMoves(SolidColorBrush color)
    {
        var king = color == Brushes.Brown ? Board.BotKing : Board.PlayerKing;
        var enemies = color == Brushes.Brown ? Board.PlayerPeaces : Board.BotPeaces;
        if (enemies.Any(piece =>
            piece.PossibleMoves.Select(m => m.Coordinates)
            .Contains(king.Position)))
        {
            MessageBox.Show("Победил " + (color == Brushes.Brown ? "Игрок" : "Бот"));
        } 
        else
        {
            MessageBox.Show("Пат!");

        }
        OnGameEnd?.Invoke();
    }

    public void CancelCastling(Move move)
    {
        int line = move.Piece.Color == Brushes.White ? Board.BOARD_WIDTH - 1 : 0;
        if (move.IsShortCastling)
        {
            var rook = Board.Squares[line, Board.BOARD_WIDTH - 3].Piece!;

            rook.UndoMove((line, Board.BOARD_WIDTH - 1), (line, Board.BOARD_WIDTH - 3), null);
        }
        else if (move.IsLongCastling)
        {
            var rook = Board.Squares[line, 3].Piece!;
            rook.UndoMove((line, 0), (line, 3), null);
        }
    }
}
