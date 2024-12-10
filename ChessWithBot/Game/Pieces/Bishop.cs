using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChessWithBot.Game.Pieces;

public class Bishop : Piece
{
    public Bishop(Brush color, (int, int) position, Board board) : base(color, PieceType.Bishop, position, board)
    {
    }

    public override int Weight => 30;

    protected override double[,] PiecePositionMatrix => new double[,] 
    {
        { -2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0 },
        { -1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -1.0 },
        { -1.0,  0.0,  0.5,  1.0,  1.0,  0.5,  0.0, -1.0 },
        { -1.0,  0.5,  0.5,  1.0,  1.0,  0.5,  0.5, -1.0 },
        { -1.0,  0.0,  1.0,  1.0,  1.0,  1.0,  0.0, -1.0 },
        { -1.0,  1.0,  1.0,  1.0,  1.0,  1.0,  1.0, -1.0 },
        { -1.0,  0.5,  0.0,  0.0,  0.0,  0.0,  0.5, -1.0 },
        { -2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0 }
    };

    public override HashSet<Move> GeneratePossibleMoves()
    {
        var moves = new HashSet<Move>();
        BishopTypeMovements(moves, false);
        return moves;
    }
}
