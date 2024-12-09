using System;
using System.Collections.Generic;
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

    public Pawn(Brush color, (int, int) position, Board board) : base(color, PieceType.Pawn, position, board)
    {
        MovesCount = 0;
    }

    public override HashSet<Move> GeneratePossibleMoves()
    {
        var set = new HashSet<Move>();
        int direction = Color == Brushes.Brown ? 1 : -1;
        if (Position.X > 0 && Position.X < Board.BOARD_HEIGHT - 1)
        {
            if (Board.Squares[Position.X + direction, Position.Y].Piece == null) 
                set.Add(new Move(this, (Position.X + direction, Position.Y)));

            if (Position.Y > 0 && Board.Squares[Position.X + direction, Position.Y - 1].Piece != null 
                && Board.Squares[Position.X + direction, Position.Y - 1].Piece!.Color != Color)
            {
                set.Add(new Move(this, (Position.X + direction, Position.Y - 1))
                {
                    TakedPiece = Board.Squares[Position.X + direction, Position.Y - 1].Piece!
                });
            }
            if (Position.Y < Board.BOARD_WIDTH - 1 && Board.Squares[Position.X + direction, Position.Y + 1].Piece != null
                && Board.Squares[Position.X + direction, Position.Y + 1].Piece!.Color != Color)
            {
                set.Add(new Move(this, (Position.X + direction, Position.Y + 1))
                {
                    TakedPiece = Board.Squares[Position.X + direction, Position.Y + 1].Piece!
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
