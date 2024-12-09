using System.Windows.Media;

namespace ChessWithBot.Game.Pieces;

public static class PieceFactory
{
    public static Piece GetPiece(PieceType pt, (int, int) position, Brush color, Board board) => pt switch
    {
        PieceType.Pawn => new Pawn(color, position, board),
        PieceType.Knight => new Knight(color, position, board),
        PieceType.Bishop => new Bishop(color, position, board),
        PieceType.Queen => new Queen(color, position, board),
        PieceType.Rook => new Rook(color, position,board),
        PieceType.King => new King(color, position, board),
    };
}
