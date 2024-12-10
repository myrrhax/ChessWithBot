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

    public Game()
    {

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
            p.Move(move, p.PossibleMoves);
            Board.UpdatePiecesPossibleMoves();
            return true;
        }
        return false;
    }

    public void UndoMove((int, int) oldPosition, Move move)
    {
        var piece = move.Piece;
        piece.UndoMove(oldPosition, move, move.TakedPiece);
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

    public List<Move> GetAllPossibleMoves(Brush color)
    {
        var pieces = color == Brushes.White ? Board.PlayerPeaces : Board.BotPeaces;
        var moveSets = pieces.Select(x => x.PossibleMoves).Select(move => move).ToList();

        List<Move> moves = new List<Move>();

        foreach (var m in moveSets)
        {
            foreach (var movesInSet in m)
            {
                moves.Add(movesInSet);
            }
        }

        return moves;
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

}
