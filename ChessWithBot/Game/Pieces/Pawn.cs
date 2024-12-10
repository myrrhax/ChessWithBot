using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ChessWithBot.Game.Pieces;

public class Pawn : Piece, IMoveDependent
{
    public int MovesCount { get; set; } = 0;

    protected override double[,] PiecePositionMatrix => new double[,] {
        { 0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0 },
        { 5.0,  5.0,  5.0,  5.0,  5.0,  5.0,  5.0,  5.0 },
        { 1.0,  1.0,  2.0,  3.0,  3.0,  2.0,  1.0,  1.0 },
        { 0.5,  0.5,  1.0,  2.5,  2.5,  1.0,  0.5,  0.5 },
        { 0.0,  0.0,  0.0,  2.0,  2.0,  0.0,  0.0,  0.0 },
        { 0.5, -0.5, -1.0,  0.0,  0.0, -1.0, -0.5,  0.5 },
        { 0.5,  1.0, 1.0,  -2.0, -2.0,  1.0,  1.0,  0.5 },
        { 0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0 }
    };

    public override int Weight => 10;

    public Pawn(Brush color, (int, int) position, Board board) : base(color, PieceType.Pawn, position, board)
    {
        MovesCount = 0;
    }

    public override bool Move(Move move, HashSet<Move> possibleMoves)
    {
        if (move.IsQueening)
        {
            var collection = Color == Brushes.White ? Board.PlayerPeaces : Board.BotPeaces;
            collection.Remove(this);
            Queen newQueen = new Queen(Color, move.Coordinates, Board);
            collection.Add(newQueen);
            if (move.IsAttacking)
            {
                Take(move.Coordinates);
            }
            Board.Squares[Position.X, Position.Y].Piece = null;
            Board.Squares[move.Coordinates.X, move.Coordinates.Y].Piece = newQueen;
            return true;
        }
        return base.Move(move, possibleMoves);
    }

    public override void UndoMove((int X, int Y) oldPosition, Move move, Piece? oldPiece)
    {
        if (move.IsQueening)
        {
            var collection = Color == Brushes.White ? Board.PlayerPeaces : Board.BotPeaces;
            var queen = Board.Squares[move.Coordinates.X, move.Coordinates.Y].Piece!;
            collection.Remove(queen);
            collection.Add(this);
            Board.Squares[queen.Position.X, queen.Position.Y].Piece = null;
            Board.Squares[Position.X, Position.Y].Piece = this;
            if (move.IsAttacking)
            {
                Board.Squares[queen.Position.X, queen.Position.Y].Piece = move.TakedPiece;
                move.TakedPiece.Position = queen.Position;
                if (Color == Brushes.White) Board.BotPeaces.Add(move.TakedPiece!);
                else Board.PlayerPeaces.Add(move.TakedPiece!);
            }
            return;
        }
        base.UndoMove(oldPosition, move, oldPiece);
    }


    public override HashSet<Move> GeneratePossibleMoves()
    {
        var set = new HashSet<Move>();
        int direction = Color == Brushes.Brown ? 1 : -1;
        if (Position.X > 0 && Position.X < Board.BOARD_HEIGHT - 1)
        {
            if (Board.Squares[Position.X + direction, Position.Y].Piece == null)
            {
                set.Add(new Move(this, (Position.X + direction, Position.Y))
                {
                    IsQueening = Position.X + direction == Board.BOARD_HEIGHT - 1 || Position.X + direction == 0
                });
            }

            if (Position.Y > 0 && Board.Squares[Position.X + direction, Position.Y - 1].Piece != null 
                && Board.Squares[Position.X + direction, Position.Y - 1].Piece!.Color != Color)
            {
                set.Add(new Move(this, (Position.X + direction, Position.Y - 1))
                {
                    TakedPiece = Board.Squares[Position.X + direction, Position.Y - 1].Piece!,
                    IsQueening = Position.X + direction == Board.BOARD_HEIGHT - 1 || Position.X + direction == 0
                });
            }
            if (Position.Y < Board.BOARD_WIDTH - 1 && Board.Squares[Position.X + direction, Position.Y + 1].Piece != null
                && Board.Squares[Position.X + direction, Position.Y + 1].Piece!.Color != Color)
            {
                set.Add(new Move(this, (Position.X + direction, Position.Y + 1))
                {
                    TakedPiece = Board.Squares[Position.X + direction, Position.Y + 1].Piece!,
                    IsQueening = Position.X + direction == Board.BOARD_HEIGHT - 1 || Position.X + direction == 0
                });
            }
        }
    
        if (MovesCount == 0)
        {
            if (Board.Squares[Position.X + direction * 2, Position.Y].Piece == null
                && Board.Squares[Position.X + direction, Position.Y].Piece == null)
            {
                set.Add(new Move(this, (Position.X + direction * 2, Position.Y)));
            }
        }

        return set;
    }
}
