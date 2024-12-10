using ChessWithBot.Utils;
using System.Collections.Generic;
using System.Windows.Media;

namespace ChessWithBot.Game.Pieces;

public abstract class Piece
{
    public PieceType Type { get; init; }
    public Brush Color { get; init; }
    public (int X, int Y) Position { get; set; }
    public Board Board { get; init; }
    public HashSet<Move> PossibleMoves { get; set; } = new HashSet<Move>();
    protected abstract double[,] PiecePositionMatrix { get; }
    public abstract int Weight { get; }
    public double[,] PositionMatrix
    {
        get
        {
            return Color == Brushes.White ? PiecePositionMatrix : PiecePositionMatrix.RotateRows();
        }
    }
    public Piece(Brush color, PieceType type, (int, int) position, Board board)
    {
        Color = color;
        Type = type;
        Position = position;
        Board = board;
    }

    public virtual bool Move(Move move, HashSet<Move> possibleMoves)
    {
        if (!possibleMoves.Contains(move)) return false;
        var oldPosition = Position;
        Position = move.Coordinates;
        if (Board.Squares[move.Coordinates.X, move.Coordinates.Y].Piece != null) Take(move.Coordinates);
        Board.Squares[oldPosition.X, oldPosition.Y].Piece = null;
        Board.Squares[Position.X, Position.Y].Piece = this;
        if (this is IMoveDependent moveDependent)
        {
            moveDependent.MovesCount++;
        }
        return true;
    }

    protected void Take((int X, int Y) position)
    {
        Piece enemy = Board.Squares[position.X, position.Y].Piece!;
        bool isPlayer = Color == Brushes.White;
        if (enemy != null)
        {
            if (isPlayer) Board.BotPeaces.Remove(enemy);
            else Board.PlayerPeaces.Remove(enemy);
        }
        Board.Squares[position.X, position.Y].Piece = null;
    }

    public virtual void UndoMove((int X, int Y) oldPosition, Move newPositon, Piece? oldPiece)
    {
        Board.Squares[oldPosition.X, oldPosition.Y].Piece = this;
        Position = oldPosition;
        if (oldPiece != null)
        {
            bool isPlayer = Color == Brushes.White;
            if (isPlayer) Board.BotPeaces.Add(oldPiece);
            else Board.PlayerPeaces.Add(oldPiece);
            oldPiece.Position = newPositon.Coordinates;
        }
        Board.Squares[newPositon.Coordinates.X, newPositon.Coordinates.Y].Piece = oldPiece;
        if (this is IMoveDependent moveDependent)
        {
            moveDependent.MovesCount--;
        }
    }

    public virtual void ValidateMoves(HashSet<Move> moves)
    {
        bool isPlayer = Color == Brushes.White;
        King king = isPlayer ? Board.PlayerKing : Board.BotKing;
        King enemyKing = isPlayer ? Board.BotKing : Board.PlayerKing;
        List<Piece> enemies = isPlayer ? Board.BotPeaces : Board.PlayerPeaces;
        Square currentSquare = Board.Squares[Position.X, Position.Y];
        foreach (var move in moves)
        {
            Piece? newSquarePiece = Board.Squares[move.Coordinates.X, move.Coordinates.Y].Piece;
            Move(move, moves);
            foreach (var p in enemies)
            {
                var enemyMoves = p.GeneratePossibleMoves();
                if (enemyMoves.Any(m => m.Coordinates == king.Position))
                {
                    moves.Remove(move);
                    break;
                }
            }
            UndoMove(currentSquare.Coordinates, move, newSquarePiece);
        }
    }

    public abstract HashSet<Move> GeneratePossibleMoves();

    protected void RookTypeMovement(HashSet<Move> res, bool lockDistance)
    {
        int xLine = Position.X;
        int yLine = Position.Y;
        int xPointer = xLine;
        int yPointer = yLine;

        Move p;
        Square[,] squares = Board.Squares;
        while (xPointer > 0)
        {
            Square sq = squares[--xPointer, yPointer];
            p = new Move(this, (xPointer, yPointer));
            if (sq.Piece == null)
            {
                res.Add(p);
                if (lockDistance) break;
                continue;
            }
            // Take enemy's piece
            if (sq.Piece != null && sq.Piece.Color != Color)
            {
                p.TakedPiece = sq.Piece;
                res.Add(p);
                if (lockDistance) break;
            }
            if (sq.Piece != null || lockDistance) break;
        }
        xPointer = xLine;
        // Right horizontal line
        while (xPointer < Board.BOARD_HEIGHT - 1)
        {
            Square sq = squares[++xPointer, yPointer];
            p = new Move(this, (xPointer, yPointer));
            if (sq.Piece == null)
            {
                res.Add(p);
                if (lockDistance) break;
                continue;
            }
            // Take enemy's piece
            if (sq.Piece != null && sq.Piece.Color != Color)
            {
                p.TakedPiece = sq.Piece;
                res.Add(p);
                if (lockDistance) break;
            }
            if (sq.Piece != null || lockDistance)
            {
                break;
            }
        }
        xPointer = xLine;
        // Top vertical line
        while (yPointer > 0)
        {
            Square sq = squares[xPointer, --yPointer];
            p = new Move(this, (xPointer, yPointer));
            if (sq.Piece == null)
            {
                res.Add(p);
                if (lockDistance) break;
                continue;
            }
            // Take enemy's piece
            if (sq.Piece != null && sq.Piece.Color != Color)
            {
                p.TakedPiece = sq.Piece;
                res.Add(p);
                if (lockDistance) break;
            }
            if (sq.Piece != null || lockDistance)
            {
                break;
            }
        }
        yPointer = yLine;
        // Bottom vertical line
        while (yPointer < Board.BOARD_WIDTH - 1)
        {
            Square sq = squares[xPointer, ++yPointer];
            p = new Move(this, (xPointer, yPointer));
            if (sq.Piece == null)
            {
                res.Add(p);
                if (lockDistance) break;
                continue;
            }
            // Take enemy's piece
            if (sq.Piece != null && sq.Piece.Color != Color)
            {
                p.TakedPiece = sq.Piece;
                res.Add(p);
                if (lockDistance) break;
            }
            if (sq.Piece != null || lockDistance) break;
        }
    }

    protected void BishopTypeMovements(HashSet<Move> res, bool lockDistance)
    {
        int xLine = Position.X;
        int yLine = Position.Y;
        int xPointer = xLine;
        int yPointer = yLine;
        Square[,] squares = Board.Squares;
        Move p;

        // Top left diagonal
        while (yPointer > 0 && xPointer > 0)
        {
            Square sq = squares[--xPointer, --yPointer];
            p = new Move(this, (xPointer, yPointer));
            if (sq.Piece == null)
            {
                res.Add(p);
                if (lockDistance) break;
                continue;
            }
            // Take enemy's piece
            if (sq.Piece != null && sq.Piece.Color != Color)
            {
                p.TakedPiece = sq.Piece;
                res.Add(p);
                if (lockDistance) break;
            }
            if (sq.Piece != null || lockDistance)
            {
                break;
            }
        }
        xPointer = xLine;
        yPointer = yLine;

        // Top right diagonal
        while (xPointer > 0 && yPointer < Board.BOARD_WIDTH - 1)
        {
            Square sq = squares[--xPointer, ++yPointer];
            p = new Move(this, (xPointer, yPointer));
            if (sq.Piece == null)
            {
                res.Add(p);
                if (lockDistance) break;
                continue;
            }
            // Take enemy's piece
            if (sq.Piece != null && sq.Piece.Color != Color)
            {
                p.TakedPiece = sq.Piece;
                res.Add(p);
                if (lockDistance) break;
            }
            if (sq.Piece != null || lockDistance)
            {
                break;
            }
        }

        xPointer = xLine;
        yPointer = yLine;

        // Bottom left diagonal
        while (xPointer < Board.BOARD_HEIGHT - 1 && yPointer > 0)
        {
            Square sq = squares[++xPointer, --yPointer];
            p = new Move(this, (xPointer, yPointer));
            if (sq.Piece == null)
            {
                res.Add(p);
                if (lockDistance) break;
                continue;
            }
            // Take enemy's piece
            if (sq.Piece != null && sq.Piece.Color != Color)
            {
                p.TakedPiece = sq.Piece;
                res.Add(p);
                if (lockDistance) break;
            }
            if (sq.Piece != null || lockDistance)
            {
                break;
            }
        }

        xPointer = xLine;
        yPointer = yLine;

        // Bottom right diagonal
        while (xPointer < Board.BOARD_HEIGHT - 1 && yPointer < Board.BOARD_WIDTH - 1)
        {
            Square sq = squares[++xPointer, ++yPointer];
            p = new Move(this, (xPointer, yPointer));
            if (sq.Piece == null)
            {
                res.Add(p);
                if (lockDistance) break;
                continue;
            }
            // Take enemy's piece
            if (sq.Piece != null && sq.Piece.Color != Color)
            {
                p.TakedPiece = sq.Piece;
                res.Add(p);
                if (lockDistance) break;
            }
            if (sq.Piece != null || lockDistance)
            {
                break;
            }
        }
    }
}

public enum PieceType
{
    Pawn, Knight, Bishop, Rook, Queen, King
}
