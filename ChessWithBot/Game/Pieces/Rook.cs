using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChessWithBot.Game.Pieces
{
    public class Rook : Piece, IMoveDependent
    {
        public Rook(Brush color, (int, int) position, Board board) : base(color, PieceType.Rook, position, board)
        {
            MovesCount = 0;
        }

        public int MovesCount { get; set; }

        protected override double[,] PiecePositionMatrix => new double[,]
        {
            {  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0 },
            {  0.5,  1.0,  1.0,  1.0,  1.0,  1.0,  1.0,  0.5 },
            { -0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5 },
            { -0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5 },
            { -0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5 },
            { -0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5 },
            { -0.5,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -0.5 },
            {  0.0,   0.0, 0.0,  0.5,  0.5,  0.0,  0.0,  0.0 }
        };

        public override HashSet<Move> GeneratePossibleMoves()
        {
            var moves = new HashSet<Move>();
            RookTypeMovement(moves, false);
            return moves;
        }

        public override bool Move((int X, int Y) position, HashSet<Move> possibleMoves)
        {
            return base.Move(position, possibleMoves);
        }
    }
}
