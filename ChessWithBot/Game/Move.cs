using ChessWithBot.Game.Pieces;
using System.Diagnostics.CodeAnalysis;

namespace ChessWithBot.Game
{
    public struct Move
    {
        public Piece Piece { get; init; }
        public (int X, int Y) Coordinates { get; init; }
        public bool IsShortCastling { get; set; } = false;
        public bool IsLongCastling { get; set; } = false;
        public bool IsQueening { get; set; } = false;
        public Piece? TakedPiece { get; set; } = default;
        public bool IsAttacking => TakedPiece != null;

        public Move(Piece piece, (int, int) coordinates)
        {
            Piece = piece;
            Coordinates = coordinates;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Move piece)
            {
                return Piece == piece.Piece && Coordinates == piece.Coordinates;
            }
            return false;
        }

        public override string ToString()
        {
            return "PIECE: " + Piece.Type.ToString() + " WITH COORDS: " + Piece.Position + " MOVES TO " + Coordinates;
        }
    }
}
