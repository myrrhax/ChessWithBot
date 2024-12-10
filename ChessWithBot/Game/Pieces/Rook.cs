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

        public override int Weight => 50;

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

        public override bool Move(Move move, HashSet<Move> possibleMoves)
        {
            return base.Move(move, possibleMoves);
        }

        public void CancelCastling(bool isShorCastling)
        {
            int line = Color == Brushes.White ? Board.BOARD_WIDTH - 1 : 0;
            if (isShorCastling)
            {
                Move move = new Move(this, (line, Board.BOARD_WIDTH - 3));
                UndoMove((line, Board.BOARD_WIDTH - 1), move, null);
            }
            else
            {
                Move move = new Move(this, (line, 3));
                UndoMove((line, 0), move, null);
            }
        }
    }
}
