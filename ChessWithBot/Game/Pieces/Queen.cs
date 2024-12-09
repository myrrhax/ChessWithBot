using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChessWithBot.Game.Pieces;

public class Queen : Piece
{
    public Queen(Brush color, (int, int) position, Board board) : base(color, PieceType.Queen, position, board)
    {
    }

    protected override double[,] PiecePositionMatrix => new double[,] 
    {
        { -2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0 },
        { -1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -1.0 },
        { -1.0,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -1.0 },
        { -0.5,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -0.5 },
        {  0.0,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -0.5 },
        { -1.0,  0.5,  0.5,  0.5,  0.5,  0.5,  0.0, -1.0 },
        { -1.0,  0.0,  0.5,  0.0,  0.0,  0.0,  0.0, -1.0 },
        { -2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0 }
    };

    public override HashSet<Move> GeneratePossibleMoves()
    {
        var moves = new HashSet<Move>();
        BishopTypeMovements(moves, false);
        RookTypeMovement(moves, false);
        return moves;
    }
}
